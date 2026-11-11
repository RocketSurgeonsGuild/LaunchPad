using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers;

/// <summary>
///     Writes the strong type registry of an application: <c>LaunchPadStrongTypes.All</c>, a <c>StrongTypeDescription</c> for every strong type
///     (a class implementing SuperStrong's <c>IStrongType&lt;TSelf, TPrimitive&gt;</c>) the application declares or references, each built
///     with closed generic arguments so nothing reflects over the types; the representations the application chose with
///     <c>LaunchPadStrongTypeOpenApiRepresentation</c> and <c>LaunchPadStrongTypeGraphQLRepresentation</c>; and, when the EF Core template
///     declares <c>ForEachDbContext</c>, a call per <c>DbContext</c> the application declares or references.
/// </summary>
/// <remarks>
///     It writes nothing unless the project declares <c>LaunchPadStrongTypes</c> in source, which Primitives' templates do: a library that
///     references Primitives gets no registry.
/// </remarks>
[Generator]
public sealed class StrongTypeRegistryGenerator : IIncrementalGenerator
{
    private const string RegistryNamespace = "Rocket.Surgery.LaunchPad.StrongTypes.Generated";
    private const string RegistryName = RegistryNamespace + ".LaunchPadStrongTypes";
    private const string ForEachDbContext = "ForEachDbContext";
    private const string StrongTypeInterface = "SuperStrong.Types.IStrongType`2";
    private const string FormatAttribute = "Rocket.Surgery.LaunchPad.StrongTypes.StrongTypeFormatAttribute";
    private const string DbContext = "Microsoft.EntityFrameworkCore.DbContext";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var representations = context.AnalyzerConfigOptionsProvider.Select(
            static (provider, _) => (
                OpenApi: Option(provider.GlobalOptions, "LaunchPadStrongTypeOpenApiRepresentation", "Component"),
                GraphQL: Option(provider.GlobalOptions, "LaunchPadStrongTypeGraphQLRepresentation", "Value")
            )
        );

        var registry = context.CompilationProvider.Select(static (compilation, cancellationToken) => Read(compilation, cancellationToken));

        context.RegisterSourceOutput(
            registry.Combine(representations),
            static (production, input) =>
            {
                if (input.Left is not { } model) return;
                production.AddSource("LaunchPadStrongTypes.g.cs", SourceText.From(Write(model, input.Right.OpenApi, input.Right.GraphQL), Encoding.UTF8));
            }
        );
    }

    private static string Option(AnalyzerConfigOptions options, string name, string fallback) =>
        options.TryGetValue($"build_property.{name}", out var value) && !string.IsNullOrWhiteSpace(value) ? value.Trim() : fallback;

    private static RegistryModel? Read(Compilation compilation, CancellationToken cancellationToken)
    {
        if (compilation.GetTypeByMetadataName(RegistryName) is not { DeclaringSyntaxReferences.Length: > 0 } registry) return null;
        if (compilation.GetTypeByMetadataName(StrongTypeInterface) is not { } strongTypeInterface) return null;

        var numberBase = compilation.GetTypeByMetadataName("System.Numerics.INumberBase`1");
        var comparable = compilation.GetTypeByMetadataName("System.IComparable`1");
        var strongTypes = new List<StrongTypeModel>();
        foreach (var type in Types(compilation, "SuperStrong.Types", cancellationToken))
        {
            if (type is not { TypeKind: TypeKind.Class, IsAbstract: false, IsGenericType: false, IsFileLocal: false }) continue;
            if (!compilation.IsSymbolAccessibleWithin(type, compilation.Assembly)) continue;
            var implemented = type.AllInterfaces.FirstOrDefault(
                z => SymbolEqualityComparer.Default.Equals(z.OriginalDefinition, strongTypeInterface)
                 && SymbolEqualityComparer.Default.Equals(z.TypeArguments[0], type)
            );
            if (implemented is null) continue;

            var primitive = implemented.TypeArguments[1];
            var isNumber = numberBase is { } && comparable is { } && Implements(primitive, numberBase) && Implements(primitive, comparable);
            strongTypes.Add(new(Name(type), Name(primitive), isNumber, Format(type)));
        }

        // The DbContexts, only for the EF Core template, which declares the partial method.
        var dbContexts = new List<string>();
        string? dbContextParameter = null;
        if (registry.GetMembers(ForEachDbContext).OfType<IMethodSymbol>().FirstOrDefault() is { IsPartialDefinition: true, Parameters: [var parameter] }
         && compilation.GetTypeByMetadataName(DbContext) is { } dbContext)
        {
            dbContextParameter = Name(parameter.Type);
            foreach (var type in Types(compilation, "Microsoft.EntityFrameworkCore", cancellationToken))
            {
                if (type is not { TypeKind: TypeKind.Class, IsAbstract: false, IsGenericType: false, IsFileLocal: false }) continue;
                if (!compilation.IsSymbolAccessibleWithin(type, compilation.Assembly)) continue;
                if (!DerivesFrom(type, dbContext)) continue;
                dbContexts.Add(Name(type));
            }
        }

        return new(
            strongTypes.OrderBy(static z => z.Type, StringComparer.Ordinal).ToImmutableArray(),
            dbContexts.OrderBy(static z => z, StringComparer.Ordinal).ToImmutableArray(),
            dbContextParameter
        );
    }

    // The types of the compilation, and of every referenced assembly that references the library (by its assembly name).
    private static IEnumerable<INamedTypeSymbol> Types(Compilation compilation, string library, CancellationToken cancellationToken)
    {
        foreach (var type in Types(compilation.Assembly.GlobalNamespace, cancellationToken)) yield return type;

        foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (assembly.Name != library && !assembly.Modules.Any(module => module.ReferencedAssemblies.Any(z => z.Name == library))) continue;
            foreach (var type in Types(assembly.GlobalNamespace, cancellationToken)) yield return type;
        }
    }

    private static IEnumerable<INamedTypeSymbol> Types(INamespaceOrTypeSymbol container, CancellationToken cancellationToken)
    {
        foreach (var member in container.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (member is INamespaceSymbol @namespace)
            {
                foreach (var type in Types(@namespace, cancellationToken)) yield return type;
            }
            else if (member is INamedTypeSymbol type)
            {
                yield return type;
                foreach (var nested in Types(type, cancellationToken)) yield return nested;
            }
        }
    }

    private static bool Implements(ITypeSymbol type, INamedTypeSymbol openInterface) =>
        type.AllInterfaces.Any(
            z => SymbolEqualityComparer.Default.Equals(z.OriginalDefinition, openInterface) && SymbolEqualityComparer.Default.Equals(z.TypeArguments[0], type)
        );

    private static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var current = type.BaseType; current is { }; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType)) return true;
        }

        return false;
    }

    // [StrongTypeFormat] on the strong type, or on the rules of its [StrongType<TPrimitive, TRules>].
    private static string? Format(INamedTypeSymbol type)
    {
        if (FormatOf(type) is { } format) return format;
        foreach (var attribute in type.GetAttributes())
        {
            if (attribute.AttributeClass is { Name: "StrongTypeAttribute", TypeArguments: [_, INamedTypeSymbol rules] } attributeClass
             && attributeClass.ContainingNamespace.ToDisplayString() == "SuperStrong.Types"
             && FormatOf(rules) is { } rulesFormat)
            {
                return rulesFormat;
            }
        }

        return null;

        static string? FormatOf(INamedTypeSymbol symbol) =>
            symbol
               .GetAttributes()
               .FirstOrDefault(z => z.AttributeClass?.ToDisplayString() == FormatAttribute)
              ?.ConstructorArguments.FirstOrDefault()
               .Value as string;
    }

    private static string Name(ITypeSymbol type) => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    private static string Write(RegistryModel model, string openApi, string graphQL)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine($"namespace {RegistryNamespace};");
        builder.AppendLine();
        builder.AppendLine("internal static partial class LaunchPadStrongTypes");
        builder.AppendLine("{");
        builder.AppendLine("    /// <summary>LaunchPadStrongTypeOpenApiRepresentation: Component (a named schema) or Inline (the value's schema in place).</summary>");
        builder.AppendLine($"    public const string OpenApiRepresentation = {Literal(openApi)};");
        builder.AppendLine("    /// <summary>LaunchPadStrongTypeGraphQLRepresentation: Value (the scalar of the value) or Named (a scalar named after the strong type).</summary>");
        builder.AppendLine($"    public const string GraphQLRepresentation = {Literal(graphQL)};");
        builder.AppendLine();
        builder.AppendLine("    /// <summary>Every strong type the application declares or references.</summary>");
        builder.AppendLine("    public static global::System.Collections.Generic.IReadOnlyList<global::Rocket.Surgery.LaunchPad.StrongTypes.StrongTypeDescription> All { get; } =");
        builder.AppendLine("    [");
        foreach (var strongType in model.StrongTypes)
        {
            var factory = strongType.IsNumber ? "CreateNumber" : "Create";
            builder.AppendLine(
                $"        global::Rocket.Surgery.LaunchPad.StrongTypes.StrongTypeDescription.{factory}<{strongType.Type}, {strongType.Primitive}>({( strongType.Format is { } format ? Literal(format) : "null" )}),"
            );
        }

        builder.AppendLine("    ];");
        if (model.DbContextParameter is { } parameter)
        {
            builder.AppendLine();
            builder.AppendLine($"    static partial void {ForEachDbContext}({parameter} visitor)");
            builder.AppendLine("    {");
            foreach (var dbContext in model.DbContexts)
            {
                builder.AppendLine($"        visitor.Visit<{dbContext}>();");
            }

            builder.AppendLine("    }");
        }

        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string Literal(string value) => "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

    private sealed record StrongTypeModel(string Type, string Primitive, bool IsNumber, string? Format);

    private sealed record RegistryModel(EquatableArray<StrongTypeModel> StrongTypes, EquatableArray<string> DbContexts, string? DbContextParameter);
}

/// <summary>An immutable array that compares by its items, so the incremental pipeline can tell when a model really changed.</summary>
internal readonly struct EquatableArray<T>(ImmutableArray<T> items) : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    private readonly ImmutableArray<T> _items = items;

    public static implicit operator EquatableArray<T>(ImmutableArray<T> items) => new(items);

    public bool Equals(EquatableArray<T> other) => _items.AsSpan().SequenceEqual(other._items.AsSpan());

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        var hash = 17;
        foreach (var item in _items) hash = hash * 31 + ( item?.GetHashCode() ?? 0 );
        return hash;
    }

    public IEnumerator<T> GetEnumerator() => ( (IEnumerable<T>)_items ).GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
