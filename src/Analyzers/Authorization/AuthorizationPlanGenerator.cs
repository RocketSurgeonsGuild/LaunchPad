using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

namespace Rocket.Surgery.LaunchPad.Analyzers.Authorization;

/// <summary>
///     Writes the authorization plan of every request in the project that has a policy attribute (docs/design/authorization.md, section 3):
///     its groups of policies with their scopes and how to run each, and how <c>Unauthorized</c> and <c>Forbidden</c> become its response. The
///     plans are registered from a module initializer, so nothing is looked up at runtime. Reports LPAD1050 to LPAD1054.
/// </summary>
[Generator]
public sealed class AuthorizationPlanGenerator : IIncrementalGenerator
{
    private const string Runtime = "global::Rocket.Surgery.LaunchPad.Authorization";
    private const string StreamRequest = "ZeroAlloc.Mediator.IStreamRequest`1";
    private const string UnaryRequest = "ZeroAlloc.Mediator.IRequest`1";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var required = context.SyntaxProvider.ForAttributeWithMetadataName(PolicyReader.RequirePolicyAttribute, IsType, TypeName);
        var any = context.SyntaxProvider.ForAttributeWithMetadataName(PolicyReader.RequireAnyPolicyAttribute, IsType, TypeName);
        var scoped = context.SyntaxProvider.ForAttributeWithMetadataName(PolicyReader.RequiresScopesAttribute, IsType, TypeName);

        var names = required
                   .Collect()
                   .Combine(any.Collect())
                   .Select(static (pair, _) => pair.Left.Concat(pair.Right).Distinct().OrderBy(n => n, StringComparer.Ordinal).ToEquatableArray())
                   .WithTrackingName("PolicyRequestNames");

        // Read against the current compilation: a policy class or a response union in another file changes the plan.
        var plans = names
                   .Combine(context.CompilationProvider)
                   .Select(
                        static (pair, _) =>
                        {
                            var (requests, compilation) = pair;
                            return  requests.IsEmpty || compilation.GetTypeByMetadataName(PolicyReader.PlanType) is null 
                                ?  EquatableArray<PlanModel>.Empty 
                                :  requests
                                  .Select(name => compilation.GetTypeByMetadataName(name))
                                  .OfType<INamedTypeSymbol>()
                                  .Select(request => Read(request, compilation))
                                  .ToEquatableArray();
                        }
                    )
                   .WithTrackingName("Plans");

        var scopes = scoped
                    .Collect()
                    .Select(static (types, _) => types.Distinct().OrderBy(n => n, StringComparer.Ordinal).ToEquatableArray())
                    .Combine(context.CompilationProvider)
                    .Select(
                         static (pair, _) => pair
                                            .Left.Select(name => pair.Right.GetTypeByMetadataName(name))
                                            .OfType<INamedTypeSymbol>()
                                            .Select(CheckScopes)
                                            .OfType<DiagnosticInfo>()
                                            .ToEquatableArray()
                     );

        context.RegisterSourceOutput(
            plans,
            static (context, plans) =>
            {
                foreach (var diagnostic in plans.SelectMany(p => p.Diagnostics)) context.ReportDiagnostic(diagnostic.ToDiagnostic());
                var emitted = plans.Where(p => p.Emit).ToArray();
                if (emitted.Length > 0) context.AddSource("RequestAuthorizationPlans.g.cs", Emit(emitted));
            }
        );
        context.RegisterSourceOutput(
            scopes,
            static (context, diagnostics) =>
            {
                foreach (var diagnostic in diagnostics) context.ReportDiagnostic(diagnostic.ToDiagnostic());
            }
        );
    }

    private static bool IsType(SyntaxNode node, CancellationToken _) => node is TypeDeclarationSyntax;

    private static string TypeName(GeneratorAttributeSyntaxContext context, CancellationToken _) => ( (INamedTypeSymbol)context.TargetSymbol ).MetadataName();

    private static PlanModel Read(INamedTypeSymbol request, Compilation compilation)
    {
        var diagnostics = new List<DiagnosticInfo>();
        var location = LocationInfo.From(request);
        var display = request.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var policies = PolicyReader.Read(request, compilation);

        var reachable = true;
        if (request.IsGenericType || request.ContainingType is { IsGenericType: true })
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.PolicyRequestNotReachable, location, display, "a generic request has no single plan"));
            reachable = false;
        }
        else if (!IsReachable(request))
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.PolicyRequestNotReachable, location, display, "it is private or protected, where generated code cannot name it"));
            reachable = false;
        }

        policies.Alternatives(out var truncated);
        if (truncated)
        {
            diagnostics.Add(
                DiagnosticInfo.Create(ApiDiagnostics.TooManyScopeAlternatives, location, display, RequestPoliciesModel.MaxAlternatives.ToString(System.Globalization.CultureInfo.InvariantCulture))
            );
        }

        // A stream has no response to answer with: its denial is thrown (AuthorizationDeniedException), whatever it yields.
        var isStream = request.AllInterfaces.Any(i => i.Is(StreamRequest));
        var response = request.AllInterfaces.FirstOrDefault(i => i.Is(UnaryRequest))?.TypeArguments[0];
        string? responseType = null;
        if (response is { })
        {
            if (!UnionResults.TryRead(response, out var union, out var cases, out _))
            {
                diagnostics.Add(
                    DiagnosticInfo.Create(
                        ApiDiagnostics.PolicyWithoutUnauthorized,
                        location,
                        display,
                        $"{response.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} is not a union"
                    )
                );
            }
            else
            {
                var unionDisplay = union.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                var problem = cases.Any(c => c.Is(UnionResults.Problem));
                var unauthorized = problem || cases.Any(c => c.Is(PolicyReader.Unauthorized));
                var forbidden = problem || cases.Any(c => c.Is(PolicyReader.Forbidden));
                if (!unauthorized) diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.PolicyWithoutUnauthorized, location, display, $"{unionDisplay} has none"));
                if (!forbidden) diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.PolicyWithoutForbidden, location, display, unionDisplay));
                // Nullable<U> of a struct union answers through U.
                if (unauthorized && forbidden && !union.IsGenericType) responseType = response.FullyQualified();
            }
        }
        else if (!isStream)
        {
            // Not a ZeroAlloc.Mediator request: no pipeline behavior of LaunchPad's dispatches it, but the plan still serves the edge.
        }

        return new(
            request.FullyQualified(),
            responseType,
            response is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T },
            policies,
            reachable && policies.Groups.All(g => g.Policies.All(p => p is { Type: { }, Interface: { } })),
            diagnostics.ToEquatableArray()
        );
    }

    private static bool IsReachable(INamedTypeSymbol type)
    {
        for (var current = type; current is { }; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility is Accessibility.Private or Accessibility.Protected or Accessibility.ProtectedAndInternal) return false;
        }

        return true;
    }

    private static DiagnosticInfo? CheckScopes(INamedTypeSymbol type)
    {
        var display = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        if (!type.GetAttributes().Any(a => a.AttributeClass.Is(PolicyReader.PolicyAttribute)))
        {
            return DiagnosticInfo.Create(ApiDiagnostics.RequiresScopesInvalid, LocationInfo.From(type), display, "it is not a [Policy], so nothing requires them");
        }

        var declared = type
                      .GetAttributes()
                      .Where(a => a.AttributeClass.Is(PolicyReader.RequiresScopesAttribute))
                      .SelectMany(a => a.ConstructorArguments is [{ Kind: TypedConstantKind.Array, IsNull: false } scopes] ? scopes.Values.Select(v => v.Value as string) : [null])
                      .ToArray();
        return declared.Length == 0 || declared.Any(string.IsNullOrWhiteSpace) || declared.Any(s => s!.Any(char.IsWhiteSpace))
            ? DiagnosticInfo.Create(ApiDiagnostics.RequiresScopesInvalid, LocationInfo.From(type), display, "a scope is missing, blank or has a space in it")
            : null;
    }

    private static string Emit(IReadOnlyList<PlanModel> plans)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("namespace Rocket.Surgery.LaunchPad.Authorization.Generated");
        builder.AppendLine("{");
        builder.AppendLine("    /// <summary>The authorization plans of this assembly's requests, registered when it loads.</summary>");
        builder.AppendLine("    [global::System.CodeDom.Compiler.GeneratedCode(\"Rocket.Surgery.LaunchPad.Authorization\", \"1.0\")]");
        builder.AppendLine("    file static class RequestAuthorizationPlans");
        builder.AppendLine("    {");
        builder.AppendLine("        [global::System.Runtime.CompilerServices.ModuleInitializer]");
        builder.AppendLine("        internal static void Register()");
        builder.AppendLine("        {");
        foreach (var plan in plans)
        {
            builder.Append("            ").Append(Runtime).Append(".RequestAuthorizationPlan.Register<").Append(plan.Request).AppendLine(">(");
            for (var g = 0; g < plan.Policies.Groups.Length; g++)
            {
                var group = plan.Policies.Groups[g];
                builder.Append("                new ").Append(Runtime).AppendLine(".PolicyGroup(");
                for (var p = 0; p < group.Policies.Length; p++)
                {
                    var policy = group.Policies[p];
                    builder
                       .Append("                    new ")
                       .Append(Runtime)
                       .Append(".PolicyRequirement(")
                       .Append(Literal(policy.Name))
                       .Append(", ")
                       .Append(Literal(policy.ArgumentsKey))
                       .Append(", typeof(")
                       .Append(policy.Type)
                       .Append("), new string[] { ")
                       .Append(Scopes(policy))
                       .Append(" }, static (services, context, cancellationToken) => ((")
                       .Append(policy.Interface)
                       .Append(')')
                       .Append(Runtime)
                       .Append(".PolicyResolver.Resolve<")
                       .Append(policy.Type)
                       .Append(">(services)).EvaluateAsync(context")
                       .Append(Arguments(policy))
                       .Append(", cancellationToken))")
                       .AppendLine(p == group.Policies.Length - 1 ? string.Empty : ",");
                }

                builder.Append("                )").AppendLine(g == plan.Policies.Groups.Length - 1 ? string.Empty : ",");
            }

            builder.AppendLine("            );");
        }

        foreach (var response in plans.Where(p => p.Response is { }).Select(p => p.Response!).Distinct(StringComparer.Ordinal))
        {
            builder
               .Append("            ")
               .Append(Runtime)
               .Append(".AuthorizationResponse<")
               .Append(response)
               .Append(">.Register(static problem => new ")
               .Append(response)
               .Append("(problem), static problem => new ")
               .Append(response)
               .AppendLine("(problem));");
        }

        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string Arguments(PolicyModel policy) => string.Concat(policy.Arguments.Select(a => ", " + a));

    private static string Scopes(PolicyModel policy) => string.Join(", ", policy.Scopes.Select(Literal));

    private static string Literal(string value) => SymbolDisplay.FormatLiteral(value, true);

    /// <summary>A request's plan, as emitted.</summary>
    /// <param name="Request">The request, fully qualified.</param>
    /// <param name="Response">The response union the denial cases convert to; null when they can't (a diagnostic says why) or for a stream.</param>
    /// <param name="Emit">Whether the plan can be written: the request can be named and every policy was found.</param>
    private sealed record PlanModel(
        string Request,
        string? Response,
        bool ResponseIsNullable,
        RequestPoliciesModel Policies,
        bool Emit,
        EquatableArray<DiagnosticInfo> Diagnostics
    );
}
