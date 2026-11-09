using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Writes the handler of each patch operation: a partial class that declares the mediator's handler interface for a request that tracks the
///     changes to another operation's request (<c>IPropertyTracking&lt;TRequest&gt;</c>), without implementing it.
/// </summary>
/// <remarks>
///     <para>
///         The application declares the class, the handler interface and the services it needs, because a mediator's own generator only sees
///         handlers in source the application wrote. This generator adds the handle method and declares a partial
///         <c>GetRequest(TPatch, CancellationToken)</c> the application implements to load the current request. The handle method applies the patch
///         to that request and sends the result through the mediator, which validates it like any other request.
///     </para>
///     <para>
///         The handler interface comes from the mediator profile, and the method's shape (return type, cancellation token) is read from that
///         interface, so it follows whatever the mediator declares. The sender is the constructor parameter, field or property of the profile's
///         sender type.
///     </para>
/// </remarks>
[Generator]
public sealed class PatchHandlerGenerator : IIncrementalGenerator
{
    internal const string PropertyTracking = "Rocket.Surgery.LaunchPad.Foundation.IPropertyTracking`1";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var candidates = context.SyntaxProvider
                                .CreateSyntaxProvider(
                                     static (node, _) => node is ClassDeclarationSyntax { BaseList: { } } declaration
                                      && declaration.Modifiers.Any(m => m.Text == "partial"),
                                     static (syntax, cancellationToken) =>
                                         syntax.SemanticModel.GetDeclaredSymbol(syntax.Node, cancellationToken) is INamedTypeSymbol type
                                      && type.Interfaces.Any(i => i.TypeArguments.Length > 0)
                                             ? type.MetadataName()
                                             : null
                                 )
                                .Where(static name => name is { })
                                .Select(static (name, _) => name!)
                                .Collect()
                                .Select(static (names, _) => names.Distinct().OrderBy(n => n, StringComparer.Ordinal).ToEquatableArray())
                                .WithTrackingName("PatchHandlerCandidates");

        var handlers = candidates
                      .Combine(context.CompilationProvider)
                      .Select(
                           static (pair, cancellationToken) =>
                           {
                               var (names, compilation) = pair;
                               if (names.IsEmpty) return EquatableArray<PatchHandlerModel>.Empty;
                               if (ProfileResolver.Resolve(compilation).Profile is not { } profile) return EquatableArray<PatchHandlerModel>.Empty;
                               var propertyTracking = compilation.GetTypeByMetadataName(PropertyTracking);
                               return  propertyTracking is null 
                                   ?  EquatableArray<PatchHandlerModel>.Empty 
                                   :  names
                                     .Select(name => compilation.GetTypeByMetadataName(name))
                                     .OfType<INamedTypeSymbol>()
                                     .SelectMany(type => Read(type, profile, propertyTracking, compilation, cancellationToken))
                                     .ToEquatableArray();
                           }
                       )
                      .WithTrackingName("PatchHandlers");

        context.RegisterSourceOutput(handlers, static (output, models) => Emit(output, models));
    }

    private static IEnumerable<PatchHandlerModel> Read(
        INamedTypeSymbol type,
        ProfileModel profile,
        INamedTypeSymbol propertyTracking,
        Compilation compilation,
        CancellationToken cancellationToken
    )
    {
        foreach (var handlerInterface in type.Interfaces)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var kind = profile.Kinds.FirstOrDefault(
                k => k.Kind != RequestKind.Stream && k.HandlerInterface is { } name && handlerInterface.Is(name)
            );
            if (kind is null || handlerInterface.TypeArguments.Length == 0) continue;
            if (handlerInterface.TypeArguments[0] is not INamedTypeSymbol patch) continue;
            var tracking = patch.AllInterfaces.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, propertyTracking));
            if (tracking?.TypeArguments[0] is not INamedTypeSymbol request) continue;

            var handle = handlerInterface.GetMembers(kind.HandleMethod).OfType<IMethodSymbol>().FirstOrDefault();
            if (handle is null) continue;
            // Already implemented by the application: nothing to write.
            if (type.FindImplementationForInterfaceMember(handle) is { }) continue;

            var location = LocationInfo.From(type);
            var diagnostics = new List<DiagnosticInfo>();
            var nonPartial = Containers(type).FirstOrDefault(c => !IsPartial(c));
            if (nonPartial is { })
            {
                diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.PatchHandlerNotPartial, location, type.Name, nonPartial.Name));
            }

            var sender = profile.SenderType is { } senderType ? FindSender(type, senderType.Replace("global::", ""), compilation) : null;
            if (sender is null)
            {
                diagnostics.Add(
                    DiagnosticInfo.Create(
                        ApiDiagnostics.PatchHandlerWithoutSender,
                        location,
                        type.Name,
                        patch.Name,
                        profile.SenderType ?? "a sender (the " + profile.Name + " mediator profile has none)"
                    )
                );
            }

            var asyncShape = AsyncShape(handle.ReturnType);
            if (asyncShape is null)
            {
                diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.PatchHandlerNotAsync, location, type.Name, handle.ReturnType.ToDisplayString()));
            }

            var parameters = handle.Parameters
                                   .Select(p => new PatchParameter(p.Type.FullyQualifiedNullable(), p.Name, p.Type.Is("System.Threading.CancellationToken")))
                                   .ToEquatableArray();

            yield return new(
                type.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : null,
                Containers(type).Reverse().Select(Declaration).ToEquatableArray(),
                type.MetadataName(),
                handlerInterface.FullyQualified(),
                handle.Name,
                handle.ReturnType.FullyQualified(),
                asyncShape ?? "",
                patch.FullyQualified(),
                request.FullyQualified(),
                ( (INamedTypeSymbol)handle.ReturnType ).TypeArguments.FirstOrDefault()?.FullyQualified() ?? "",
                parameters,
                sender ?? "",
                profile.SendMethod,
                profile.SendNeedsResponseTypeArgument,
                diagnostics.ToEquatableArray()
            );
        }
    }

    // Task<T> or ValueTask<T>; the generated method awaits the load and the send.
    private static string? AsyncShape(ITypeSymbol returnType) =>
        returnType is INamedTypeSymbol { TypeArguments.Length: 1 } named
            ? named.Is("System.Threading.Tasks.Task`1")
                ? "global::System.Threading.Tasks.Task"
                : named.Is("System.Threading.Tasks.ValueTask`1")
                    ? "global::System.Threading.Tasks.ValueTask"
                    : null
            : null;

    private static string? FindSender(INamedTypeSymbol type, string senderType, Compilation compilation)
    {
        // A sender a mediator generates (ZeroAlloc's IMediator) isn't visible to this generator: it is still an error type here, so its simple name
        // is all there is to match.
        var simpleName = senderType[( senderType.LastIndexOf('.') + 1 )..];
        bool IsSender(ITypeSymbol candidate) =>
            candidate.Is(senderType)
         || candidate.AllInterfaces.Any(i => i.Is(senderType))
         || ( candidate.TypeKind == TypeKind.Error && candidate.Name == simpleName );

        // A primary constructor's parameters are in scope in every part of the class.
        var primary = type.InstanceConstructors.FirstOrDefault(
            c => c.DeclaringSyntaxReferences.Any(r => r.GetSyntax() is TypeDeclarationSyntax)
        );
        if (primary?.Parameters.FirstOrDefault(p => IsSender(p.Type)) is { } parameter) return parameter.Name;

        var member = type.GetMembers()
                         .FirstOrDefault(
                              m => m switch
                              {
                                  IFieldSymbol { IsImplicitlyDeclared: false, IsStatic: false } f => IsSender(f.Type),
                                  IPropertySymbol { IsStatic: false, GetMethod: { } } p => IsSender(p.Type),
                                  _ => false,
                              }
                          );
        return member?.Name;
    }

    private static IEnumerable<INamedTypeSymbol> Containers(INamedTypeSymbol type)
    {
        for (var current = type; current is { }; current = current.ContainingType)
        {
            yield return current;
        }
    }

    private static bool IsPartial(INamedTypeSymbol type) =>
        type.DeclaringSyntaxReferences.Any(r => r.GetSyntax() is TypeDeclarationSyntax declaration && declaration.Modifiers.Any(m => m.Text == "partial"));

    private static string Declaration(INamedTypeSymbol type)
    {
        var keyword = type switch
        {
            { IsRecord: true, TypeKind: TypeKind.Struct } => "record struct",
            { IsRecord: true } => "record",
            { TypeKind: TypeKind.Struct } => "struct",
            { TypeKind: TypeKind.Interface } => "interface",
            _ => "class",
        };
        var typeParameters = type.TypeParameters.Length > 0 ? "<" + string.Join(", ", type.TypeParameters.Select(t => t.Name)) + ">" : "";
        return ( type.IsStatic ? "static " : "" ) + "partial " + keyword + " " + type.Name + typeParameters;
    }

    private static void Emit(SourceProductionContext output, EquatableArray<PatchHandlerModel> handlers)
    {
        foreach (var handler in handlers)
        {
            foreach (var diagnostic in handler.Diagnostics) output.ReportDiagnostic(diagnostic.ToDiagnostic());
            if (!handler.Diagnostics.IsEmpty) continue;
            output.AddSource(handler.MetadataName.Replace('+', '.').Replace('`', '_') + "." + Hint(handler.Patch) + ".Patch.g.cs", Write(handler));
        }
    }

    private static string Hint(string fullyQualified) =>
        new([.. fullyQualified.Replace("global::", "").Select(c => char.IsLetterOrDigit(c) || c == '.' ? c : '_')]);

    private static string Write(PatchHandlerModel handler)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("#pragma warning disable CS1591");
        if (handler.Namespace is { } ns) sb.Append("namespace ").Append(ns).AppendLine(";");
        sb.AppendLine();

        var indent = "";
        foreach (var container in handler.Containers)
        {
            sb.Append(indent).AppendLine(container);
            sb.Append(indent).AppendLine("{");
            indent += "    ";
        }

        var cancellationToken = handler.Parameters.FirstOrDefault(p => p.IsCancellationToken)?.Name;
        var patch = handler.Parameters.First().Name;
        var tokenArgument = cancellationToken is { } ? ", " + cancellationToken : "";

        sb.Append(indent).AppendLine("/// <summary>");
        sb.Append(indent).Append("///     Loads the current <see cref=\"").Append(handler.Request.Replace("global::", "")).Append("\" /> the patch is applied to.").AppendLine();
        sb.Append(indent).AppendLine("/// </summary>");
        sb.Append(indent)
          .Append("private partial ")
          .Append(handler.AsyncShape)
          .Append('<')
          .Append(handler.Request)
          .Append("> GetRequest(")
          .Append(handler.Patch)
          // Fixed names, so the application's implementation doesn't depend on what the mediator calls its parameters.
          .Append(" request")
          .Append(cancellationToken is { } ? ", global::System.Threading.CancellationToken cancellationToken" : "")
          .AppendLine(");");
        sb.AppendLine();

        sb.Append(indent).AppendLine("/// <summary>");
        sb.Append(indent).AppendLine("///     Applies the patch to the current request and sends the result through the mediator, which validates it.");
        sb.Append(indent).AppendLine("/// </summary>");
        sb.Append(indent).Append("[global::System.CodeDom.Compiler.GeneratedCode(\"Rocket.Surgery.LaunchPad.Api.Patch\", \"")
          .Append(typeof(PatchHandlerGenerator).Assembly.GetName().Version)
          .AppendLine("\")]");
        sb.Append(indent)
          .Append("public async ")
          .Append(handler.ReturnType)
          .Append(' ')
          .Append(handler.HandleMethod)
          .Append('(')
          .Append(string.Join(", ", handler.Parameters.Select(p => p.Type + " " + p.Name)))
          .AppendLine(")");
        sb.Append(indent).AppendLine("{");
        sb.Append(indent).Append("    var current = await GetRequest(").Append(patch).Append(tokenArgument).AppendLine(").ConfigureAwait(false);");
        sb.Append(indent)
          .Append("    return await ")
          .Append(handler.Sender)
          .Append('.')
          .Append(handler.SendMethod)
          .Append(handler.SendNeedsResponseTypeArgument ? "<" + handler.Response + ">" : "")
          .Append('(')
          .Append(patch)
          .Append(".ApplyChanges(current)")
          .Append(tokenArgument)
          .AppendLine(").ConfigureAwait(false);");
        sb.Append(indent).AppendLine("}");

        foreach (var _ in handler.Containers)
        {
            indent = indent[4..];
            sb.Append(indent).AppendLine("}");
        }

        return sb.ToString();
    }
}

internal sealed record PatchParameter(string Type, string Name, bool IsCancellationToken);

internal sealed record PatchHandlerModel(
    string? Namespace,
    EquatableArray<string> Containers,
    string MetadataName,
    string HandlerInterface,
    string HandleMethod,
    string ReturnType,
    string AsyncShape,
    string Patch,
    string Request,
    string Response,
    EquatableArray<PatchParameter> Parameters,
    string Sender,
    string SendMethod,
    bool SendNeedsResponseTypeArgument,
    EquatableArray<DiagnosticInfo> Diagnostics
);
