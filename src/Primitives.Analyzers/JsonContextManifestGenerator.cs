using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.LaunchPad.Mediators;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers;

/// <summary>
///     Writes the JSON context manifest of an assembly: an <c>[assembly: JsonContextType(typeof(X))]</c> for every type of it that crosses the
///     wire as JSON. An application's build reads the manifests of its referenced assemblies and generates the JSON context that declares them.
/// </summary>
/// <remarks>
///     <para>
///         Listed: every mediator request (recognised with the mediator libraries LaunchPad supports, or <c>[assembly: MediatorRequest]</c>) and its
///         response or stream item type, every <c>IPropertyTracking&lt;T&gt;</c> type with its target, and every type marked
///         <c>[IncludeInJsonContext]</c>. Only public types are listed, since the application's context names them.
///     </para>
///     <para>Nothing is written when the compilation doesn't reference Rocket.Surgery.LaunchPad.Primitives.</para>
/// </remarks>
[Generator]
public sealed class JsonContextManifestGenerator : IIncrementalGenerator
{
    private const string ManifestAttribute = "Rocket.Surgery.LaunchPad.Primitives.JsonContextTypeAttribute";
    private const string IncludeAttribute = "Rocket.Surgery.LaunchPad.Primitives.IncludeInJsonContextAttribute";
    private const string PropertyTracking = "Rocket.Surgery.LaunchPad.Foundation.IPropertyTracking`1";
    private const string CustomRequestAttribute = "MediatorRequestAttribute";

    private static readonly SymbolDisplayFormat FullyQualified = SymbolDisplayFormat.FullyQualifiedFormat;

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var types = context.SyntaxProvider
                           .CreateSyntaxProvider(
                                static (node, _) => node is TypeDeclarationSyntax declaration
                                 && ( declaration.BaseList is { } || declaration.AttributeLists.Count > 0 ),
                                static (ctx, ct) => Listed(ctx, ct)
                            )
                           .Where(static listed => listed.Length > 0)
                           .Collect()
                           .Select(
                                static (all, _) => string.Join(
                                    "\n",
                                    all.SelectMany(e => e.Split('\n')).Where(e => e.Length > 0).Distinct().OrderBy(e => e, StringComparer.Ordinal)
                                )
                            );
        var hasManifest = context.CompilationProvider.Select(static (compilation, _) => compilation.GetTypeByMetadataName(ManifestAttribute) is { });

        context.RegisterSourceOutput(
            types.Combine(hasManifest),
            static (output, pair) =>
            {
                var (names, manifest) = pair;
                if (!manifest || names.Length == 0) return;
                output.AddSource("Rocket.Surgery.LaunchPad.Primitives.JsonContextManifest.g.cs", Emit(names.Split('\n')));
            }
        );
    }

    // The types one declaration lists, as fully qualified names, one per line (a string, so the step is cached by value).
    private static string Listed(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol type) return "";
        if (type.IsGenericType || type.IsStatic || type.TypeKind is not ( TypeKind.Class or TypeKind.Struct or TypeKind.Enum or TypeKind.Interface )) return "";

        var compilation = context.SemanticModel.Compilation;
        var listed = new SortedSet<string>(StringComparer.Ordinal);
        void Add(ITypeSymbol candidate)
        {
            var unwrapped = Unwrap(candidate);
            if (IsListable(unwrapped)) listed.Add(unwrapped.ToDisplayString(FullyQualified));
        }

        if (type.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == IncludeAttribute)) Add(type);

        foreach (var implemented in type.AllInterfaces)
        {
            var name = MetadataName(implemented.OriginalDefinition);
            if (name == PropertyTracking)
            {
                Add(type);
                Add(implemented.TypeArguments[0]);
                continue;
            }

            if (RequestInterface(compilation, implemented.OriginalDefinition, name) is not { } request) continue;
            Add(type);
            if (request.Shape is MediatorRequestShape.Void || implemented.TypeArguments.Length <= request.ResponseTypeArgumentIndex) continue;
            var response = implemented.TypeArguments[request.ResponseTypeArgumentIndex];
            if (!IsUnit(response)) Add(response);
        }

        return string.Join("\n", listed);
    }

    private static MediatorRequestInterface? RequestInterface(Compilation compilation, INamedTypeSymbol definition, string name)
    {
        foreach (var library in MediatorLibraries.All)
        {
            foreach (var request in library.Requests)
            {
                if (request.MetadataName == name) return request;
            }
        }

        // A custom profile names its request interfaces with [assembly: MediatorRequest(typeof(IMyRequest<>), ...)].
        foreach (var attribute in compilation.Assembly.GetAttributes())
        {
            if (attribute.AttributeClass?.Name != CustomRequestAttribute) continue;
            if (attribute.ConstructorArguments is not [{ Value: INamedTypeSymbol custom }, ..] || !SymbolEqualityComparer.Default.Equals(custom.OriginalDefinition, definition)) continue;
            var index = attribute.NamedArguments.FirstOrDefault(a => a.Key == "ResponseTypeArgumentIndex").Value.Value is int i ? i : 0;
            var kind = attribute.NamedArguments.FirstOrDefault(a => a.Key == "Kind").Value.Value is int k ? (MediatorRequestShape)k : MediatorRequestShape.Unary;
            return new(name, kind == 0 ? MediatorRequestShape.Unary : kind, index, null, "Handle");
        }

        return null;
    }

    private static ITypeSymbol Unwrap(ITypeSymbol type) =>
        type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T, TypeArguments: [var underlying] }
            ? underlying
            : type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

    private static bool IsUnit(ITypeSymbol type) => MediatorLibraries.UnitTypes.Contains(MetadataName(type.OriginalDefinition));

    // The application's context names the type, so it must be public all the way down; primitives need no listing.
    private static bool IsListable(ITypeSymbol type) =>
        type switch
        {
            { SpecialType: not SpecialType.None } => false,
            ITypeParameterSymbol => false,
            IArrayTypeSymbol array => IsListable(array.ElementType) || IsPublic(array.ElementType),
            INamedTypeSymbol named => IsPublic(named) && named.TypeArguments.All(a => a is not ITypeParameterSymbol && ( a.SpecialType != SpecialType.None || IsPublic(a) )),
            _ => false,
        };

    private static bool IsPublic(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol array) return IsPublic(array.ElementType);
        for (ISymbol? current = type; current is INamedTypeSymbol; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility != Accessibility.Public) return false;
        }

        return type is INamedTypeSymbol named ? named.TypeArguments.All(a => a.SpecialType != SpecialType.None || IsPublic(a)) : type.SpecialType != SpecialType.None;
    }

    private static string MetadataName(ITypeSymbol type) =>
        type.ContainingType is { } containing
            ? MetadataName(containing) + "+" + type.MetadataName
            : type.ContainingNamespace is { IsGlobalNamespace: false } ns
                ? ns.ToDisplayString() + "." + type.MetadataName
                : type.MetadataName;

    private static string Emit(IEnumerable<string> types)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("// The types of this assembly that cross the wire as JSON: the applications that reference it declare them in their JSON context.");
        builder.AppendLine("#pragma warning disable");
        foreach (var type in types)
        {
            builder.Append("[assembly: global::").Append(ManifestAttribute).Append("(typeof(").Append(type).AppendLine("))]");
        }

        return builder.ToString();
    }
}
