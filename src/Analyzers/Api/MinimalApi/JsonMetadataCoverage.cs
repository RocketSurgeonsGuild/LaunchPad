using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>The types the source generated JSON metadata of a compilation covers, when the project needs it.</summary>
internal sealed record JsonCoverage(bool Required, EquatableArray<string> Types)
{
    public static JsonCoverage NotRequired { get; } = new(false, EquatableArray<string>.Empty);
}

/// <summary>
///     Warns when a trimmed or native AOT project has a Minimal API operation whose body or response type no <c>JsonSerializerContext</c> declares.
/// </summary>
/// <remarks>
///     <para>
///         The generated request delegates read and write JSON with the <c>JsonTypeInfo</c> the application's HTTP JSON options resolve. Generators
///         can't see each other's output, so these generators can't declare a context for the System.Text.Json source generator to fill in: the
///         application declares one, and this check names the types it is missing. Without them the endpoint throws
///         <c>NotSupportedException</c> at run time once reflection is off.
///     </para>
///     <para>
///         The check runs when the project sets <c>PublishAot</c>, <c>IsAotCompatible</c>, <c>PublishTrimmed</c> or <c>IsTrimmable</c>, or
///         <c>LaunchPadApiRequireJsonContext</c>, which also turns it off with <c>false</c>. A type counts as declared when a context in the
///         compilation or a referenced assembly declares it, or one of the types it reaches through public properties, collections or base types.
///         The primitive types LaunchPad's own context declares (<c>SystemTextJsonJoint</c>) need no declaration.
///     </para>
/// </remarks>
internal static class JsonMetadataCoverage
{
    public const string RequireProperty = "build_property.LaunchPadApiRequireJsonContext";

    private static readonly string[] TrimmedProperties =
        ["build_property.PublishAot", "build_property.IsAotCompatible", "build_property.PublishTrimmed", "build_property.IsTrimmable"];

    // What LaunchPad's own context (Rocket.Surgery.LaunchPad.Foundation.LaunchPadJsonSerializerContext) declares, as FullyQualified() writes it.
    private static readonly HashSet<string> LaunchPadTypes =
    [
        with(StringComparer.Ordinal),
        "object", "string", "bool", "byte", "short", "int", "long", "float", "double", "decimal", "char", "string[]", "object[]", "int[]",
        "global::System.Guid", "global::System.DateTime", "global::System.DateTimeOffset", "global::System.DateOnly", "global::System.TimeOnly",
        "global::System.TimeSpan", "global::System.Uri", "global::System.Text.Json.JsonElement", "global::System.Text.Json.JsonDocument",
        "global::System.Text.Json.Nodes.JsonNode", "global::System.Text.Json.Nodes.JsonObject", "global::System.Text.Json.Nodes.JsonArray",
    ];

    public static IncrementalValueProvider<JsonCoverage> Provider(IncrementalGeneratorInitializationContext context) =>
        context.AnalyzerConfigOptionsProvider
               .Select(static (options, _) => IsRequired(options.GlobalOptions))
               .Combine(context.CompilationProvider)
               .Select(static (pair, cancellationToken) => pair.Left ? new(true, Covered(pair.Right, cancellationToken)) : JsonCoverage.NotRequired)
               .WithTrackingName("JsonCoverage");

    public static void Report(SourceProductionContext output, EquatableArray<HostModel> hosts, JsonCoverage coverage)
    {
        if (!coverage.Required) return;
        var covered = new HashSet<string>(coverage.Types, StringComparer.Ordinal);
        var reported = new HashSet<string>(StringComparer.Ordinal);
        foreach (var host in hosts)
        {
            if (host.Profile is null) continue;
            foreach (var operation in host.Operations)
            {
                foreach (var type in JsonTypes(operation))
                {
                    if (LaunchPadTypes.Contains(type) || covered.Contains(type) || !reported.Add(type)) continue;
                    var display = type.Replace("global::", "");
                    output.ReportDiagnostic(
                        DiagnosticInfo.Create(ApiDiagnostics.JsonMetadataMissing, operation.Location, display, host.Name, operation.Name).ToDiagnostic()
                    );
                }
            }
        }
    }

    // The body the request delegate reads and the response it writes: the item type of a stream, nothing for a void operation.
    private static IEnumerable<string> JsonTypes(OperationModel operation)
    {
        if (operation.HasBody) yield return operation.WireType;
        if (operation.ResponseTypeNotNull is not { } response) yield break;
        if (operation.Kind == RequestKind.Stream || ( operation.Kind != RequestKind.Void && !operation.ResponseIsUnit )) yield return response;
    }

    private static bool IsRequired(AnalyzerConfigOptions options)
    {
        return options.TryGetValue(RequireProperty, out var explicitValue) && bool.TryParse(explicitValue, out var required)
            ? required
            : TrimmedProperties.Any(name => options.TryGetValue(name, out var value) && string.Equals(value, "true", StringComparison.OrdinalIgnoreCase));
    }

    private static EquatableArray<string> Covered(Compilation compilation, CancellationToken cancellationToken)
    {
        if (compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonSerializerContext") is not { } context
         || compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonSerializableAttribute") is not { } serializable)
        {
            return EquatableArray<string>.Empty;
        }

        var roots = new List<ITypeSymbol>();
        foreach (var assembly in new[] { compilation.Assembly }.Concat(compilation.SourceModule.ReferencedAssemblySymbols.Where(MayDeclareContexts)))
        {
            foreach (var type in Types(assembly.GlobalNamespace, cancellationToken))
            {
                if (!DerivesFrom(type, context)) continue;
                roots.AddRange(
                    type.GetAttributes()
                        .Where(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, serializable))
                        .Select(a => a.ConstructorArguments is [{ Value: ITypeSymbol declared }, ..] ? declared : null)
                        .OfType<ITypeSymbol>()
                );
            }
        }

        var covered = new HashSet<string>(StringComparer.Ordinal);
        var visited = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var pending = new Stack<ITypeSymbol>(roots);
        while (pending.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var type = pending.Pop().WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            if (!visited.Add(type)) continue;
            covered.Add(type.FullyQualified());
            foreach (var reached in Reached(type)) pending.Push(reached);
        }

        return covered.OrderBy(t => t, StringComparer.Ordinal).ToEquatableArray();
    }

    // The source generator writes metadata for every type a declared type reaches: element and type arguments, base types, and the public
    // properties and fields it serializes. Framework types are leaves, apart from their type arguments.
    private static IEnumerable<ITypeSymbol> Reached(ITypeSymbol type)
    {
        switch (type)
        {
            case IArrayTypeSymbol array:
                yield return array.ElementType;
                yield break;
            case INamedTypeSymbol named:
                foreach (var argument in named.TypeArguments) yield return argument;
                if (IsFramework(named)) yield break;
                if (named.BaseType is { SpecialType: SpecialType.None } baseType && !IsFramework(baseType)) yield return baseType;
                foreach (var member in named.GetMembers())
                {
                    if (member.IsStatic || member.DeclaredAccessibility != Accessibility.Public) continue;
                    if (member is IPropertySymbol { IsIndexer: false } property) yield return property.Type;
                    else if (member is IFieldSymbol { IsImplicitlyDeclared: false } field) yield return field.Type;
                }

                yield break;
            default:
                break;
        }
    }

    private static bool IsFramework(INamedTypeSymbol type) =>
        type.ContainingNamespace?.ToDisplayString() is { } ns && ( ns == "System" || ns.StartsWith("System.", StringComparison.Ordinal) || ns.StartsWith("Microsoft.", StringComparison.Ordinal) );

    private static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var current = type.BaseType; current is { }; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType)) return true;
        }

        return false;
    }

    // Only assemblies that use System.Text.Json can declare a context; the framework's own contexts serialize the framework's types.
    private static bool MayDeclareContexts(IAssemblySymbol assembly) =>
        assembly.Name is not ( "netstandard" or "mscorlib" or "System.Text.Json" )
     && !assembly.Name.StartsWith("System.", StringComparison.Ordinal)
     && !assembly.Name.StartsWith("Microsoft.", StringComparison.Ordinal)
     && assembly.Modules.Any(module => module.ReferencedAssemblies.Any(reference => reference.Name == "System.Text.Json"));

    private static IEnumerable<INamedTypeSymbol> Types(INamespaceSymbol ns, CancellationToken cancellationToken)
    {
        foreach (var type in ns.GetTypeMembers())
        {
            foreach (var nested in WithNested(type)) yield return nested;
        }

        foreach (var child in ns.GetNamespaceMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var type in Types(child, cancellationToken)) yield return type;
        }
    }

    private static IEnumerable<INamedTypeSymbol> WithNested(INamedTypeSymbol type)
    {
        yield return type;
        foreach (var nested in type.GetTypeMembers().SelectMany(WithNested)) yield return nested;
    }
}
