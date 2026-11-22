using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

namespace Rocket.Surgery.LaunchPad.Analyzers.Authorization;

/// <summary>
///     Writes the <c>EvaluateAsync</c> of a policy class that has <c>[RequireScope]</c> or <c>[RequireAnyScope]</c>
///     (docs/design/authorization.md, section 1): such a policy is what its scope attributes say, so its check of the caller's scopes is
///     generated, and runs wherever the policy does. Reports LPAD1053 for a class the method cannot be written for.
/// </summary>
[Generator]
public sealed class ScopePolicyGenerator : IIncrementalGenerator
{
    private const string Runtime = "global::Rocket.Surgery.LaunchPad.Authorization";
    private const string EvaluateAsync = "EvaluateAsync";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var all = context.SyntaxProvider.ForAttributeWithMetadataName(PolicyReader.RequireScopeAttribute, IsClass, Read);
        var any = context.SyntaxProvider.ForAttributeWithMetadataName(PolicyReader.RequireAnyScopeAttribute, IsClass, Read);

        // A class with both kinds of attribute is found twice, and written once.
        var policies = all
                      .Collect()
                      .Combine(any.Collect())
                      .SelectMany(static (pair, _) => pair.Left.Concat(pair.Right).GroupBy(p => p.HintName, StringComparer.Ordinal).Select(g => g.First()));
        context.RegisterSourceOutput(
            policies,
            static (context, policy) =>
            {
                if (policy.Diagnostic is { } diagnostic)
                {
                    context.ReportDiagnostic(diagnostic.ToDiagnostic());
                    return;
                }

                context.AddSource(policy.HintName, Emit(policy));
            }
        );
    }

    private static bool IsClass(SyntaxNode node, CancellationToken _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax;

    private static ScopePolicyModel Read(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        var type = (INamedTypeSymbol)context.TargetSymbol;
        var display = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var requirements = PolicyReader.ScopeRequirements(type);
        var hintName = type.MetadataName().Replace('+', '.').Replace('`', '_') + ".ScopePolicy.g.cs";

        ScopePolicyModel Invalid(string reason) =>
            new(hintName, null, default, default, default, DiagnosticInfo.Create(ApiDiagnostics.ScopePolicyInvalid, LocationInfo.From(type), display, reason));

        if (!type.GetAttributes().Any(a => a.AttributeClass.Is(PolicyReader.PolicyAttribute)))
        {
            return Invalid("it is not a [Policy], so nothing requires the scopes");
        }

        if (requirements.Any(r => r.Scopes.IsEmpty || r.Scopes.Any(s => string.IsNullOrWhiteSpace(s) || s.Any(char.IsWhiteSpace))))
        {
            return Invalid("a scope is missing, blank or has a space in it");
        }

        if (!type.Interfaces.Any(i => i is { TypeArguments.Length: 0 } && i.Is(PolicyReader.PolicyInterface)))
        {
            return Invalid("it does not list IAuthorizationPolicy, which the generated EvaluateAsync implements; a policy with arguments is not a matter of scopes alone");
        }

        if (type.GetMembers(EvaluateAsync).OfType<IMethodSymbol>().Any(m => !m.IsImplicitlyDeclared)
         || type.GetMembers().OfType<IMethodSymbol>().Any(m => m.ExplicitInterfaceImplementations.Any(i => i.Name == EvaluateAsync)))
        {
            return Invalid("it declares EvaluateAsync itself: a policy with scope attributes is those scopes and nothing else, and its EvaluateAsync is generated");
        }

        var containers = new List<INamedTypeSymbol>();
        for (var current = type; current is { }; current = current.ContainingType) containers.Insert(0, current);
        return  type.IsGenericType || containers.Any(c => c.IsGenericType) || containers.Any(c => !IsPartial(c, cancellationToken)) 
            ?  Invalid("it, or a type it is nested in, is generic or not partial, so its EvaluateAsync cannot be generated into it") 
            :   new(
            hintName,
            type.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : null,
            containers.Select(Declaration).ToEquatableArray(),
            requirements,
            display,
            null
        );
    }

    private static bool IsPartial(INamedTypeSymbol type, CancellationToken cancellationToken) =>
        type.DeclaringSyntaxReferences.All(r => r.GetSyntax(cancellationToken) is TypeDeclarationSyntax declaration && declaration.Modifiers.Any(SyntaxKind.PartialKeyword));

    // "partial class Name", as the type is declared: a record is a record, a static container is static.
    private static string Declaration(INamedTypeSymbol type) =>
        ( type.IsStatic ? "static " : "" )
      + "partial "
      + ( type is { IsRecord: true, IsValueType: true } ? "record struct" : type.IsRecord ? "record" : type.IsValueType ? "struct" : type.TypeKind == TypeKind.Interface ? "interface" : "class" )
      + " "
      + type.Name;

    private static string Emit(ScopePolicyModel policy)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("#nullable enable");
        if (policy.Namespace is { } ns) builder.Append("namespace ").Append(ns).AppendLine(";").AppendLine();
        for (var depth = 0; depth < policy.Declarations.Length; depth++)
        {
            builder.Append(' ', depth * 4).AppendLine(policy.Declarations[depth]);
            builder.Append(' ', depth * 4).AppendLine("{");
        }

        var indent = new string(' ', policy.Declarations.Length * 4);
        builder.Append(indent).Append("private static readonly ").Append(Runtime).AppendLine(".ScopeRequirement[] __scopes =");
        builder.Append(indent).AppendLine("{");
        foreach (var requirement in policy.Requirements)
        {
            builder
               .Append(indent)
               .Append("    new ")
               .Append(Runtime)
               .Append(".ScopeRequirement(")
               .Append(requirement.Any ? "true" : "false")
               .Append(Scopes(requirement))
               .AppendLine("),");
        }

        builder.Append(indent).AppendLine("};");
        builder.AppendLine();
        builder.Append(indent).AppendLine("/// <summary>Checks that the caller has the scopes this policy requires with its scope attributes.</summary>");
        builder.Append(indent).AppendLine("/// <param name=\"ctx\">The caller.</param>");
        builder.Append(indent).AppendLine("/// <param name=\"ct\">The cancellation token of the call.</param>");
        builder.Append(indent).AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"Rocket.Surgery.LaunchPad.Authorization\", \"1.0\")]");
        builder
           .Append(indent)
           .AppendLine(
                "public global::System.Threading.Tasks.ValueTask<global::ZeroAlloc.Results.UnitResult<global::ZeroAlloc.Authorization.AuthorizationFailure>> EvaluateAsync(global::ZeroAlloc.Authorization.ISecurityContext ctx, global::System.Threading.CancellationToken ct = default) =>"
            );
        builder.Append(indent).Append("    new(").Append(Runtime).AppendLine(".ScopeRequirements.Evaluate(ctx, __scopes));");
        for (var depth = policy.Declarations.Length - 1; depth >= 0; depth--)
        {
            builder.Append(' ', depth * 4).AppendLine("}");
        }

        return builder.ToString();
    }

    private static string Scopes(ScopeRequirementModel requirement) => string.Concat(requirement.Scopes.Select(scope => ", " + SymbolDisplay.FormatLiteral(scope, true)));

    private sealed record ScopePolicyModel(
        string HintName,
        string? Namespace,
        EquatableArray<string> Declarations,
        EquatableArray<ScopeRequirementModel> Requirements,
        string? Display,
        DiagnosticInfo? Diagnostic
    );
}
