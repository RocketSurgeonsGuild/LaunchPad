using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Emit;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Registry;

/// <summary>
///     Writes what an application needs to know about every domain primitive it declares or references:
///     <list type="bullet">
///         <item>
///             the registry the Joints Primitives writes into an application as templates read (<c>LaunchPadPrimitives</c>): the description
///             of every domain primitive and value object, the representations the application chose, and when the EF Core template asks
///             for them, the DbContexts and a value converter per domain primitive. Nothing is written unless the project declares
///             <c>LaunchPadPrimitives</c> in source, which the templates do;
///         </item>
///         <item>a Dapper type handler per domain primitive, registered by a module initializer, when the project references Dapper;</item>
///         <item><c>AddLaunchPadPrimitivesInstrumentation()</c> for OpenTelemetry's builders, when the project references OpenTelemetry.</item>
///     </list>
/// </summary>
/// <remarks>
///     A domain primitive of the project itself is found by its primitive attribute (the domain primitive generator completes it in the same
///     run, so its interfaces aren't there yet); one of a referenced assembly by the <c>IDomainPrimitive</c> interface it implements.
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class PrimitiveRegistryGenerator : IIncrementalGenerator
{
    private const string RegistryNamespace = "Rocket.Surgery.LaunchPad.Primitives.Generated";
    private const string RegistryName = RegistryNamespace + ".LaunchPadPrimitives";
    private const string Library = "Rocket.Surgery.LaunchPad.Primitives";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var representations = context.AnalyzerConfigOptionsProvider.Select(
            static (provider, _) => (
                OpenApi: Option(provider.GlobalOptions, "LaunchPadPrimitivesOpenApiRepresentation", "Component"),
                GraphQL: Option(provider.GlobalOptions, "LaunchPadPrimitivesGraphQLRepresentation", "Value")
            )
        );

        var model = context.CompilationProvider.Select(static (compilation, cancellationToken) => Read(compilation, cancellationToken));
        context.RegisterSourceOutput(
            model.Combine(representations),
            static (production, input) =>
            {
                var (registry, (openApi, graphQL)) = input;
                if (registry.HasRegistry) production.AddSource("LaunchPadPrimitives.g.cs", Text(WriteRegistry(registry, openApi, graphQL)));
                if (registry.HasDapper) production.AddSource("LaunchPadPrimitivesDapper.g.cs", Text(WriteDapper(registry)));
                if (registry.HasMetrics || registry.HasTracing) production.AddSource("LaunchPadPrimitivesInstrumentation.g.cs", Text(WriteInstrumentation(registry)));
            }
        );
    }

    private static SourceText Text(string source) => SourceText.From(source, Encoding.UTF8);

    private static string Option(AnalyzerConfigOptions options, string name, string fallback) =>
        options.TryGetValue($"build_property.{name}", out var value) && !string.IsNullOrWhiteSpace(value) ? value.Trim() : fallback;

    private sealed record PrimitiveEntry(string Type, string Value, bool IsGuidStrongId, bool IsStrongId);

    private sealed record RegistryModel(
        bool HasRegistry,
        bool HasDapper,
        bool HasMetrics,
        bool HasTracing,
        EquatableArray<PrimitiveEntry> Primitives,
        EquatableArray<string> ValueObjects,
        EquatableArray<string> DbContexts,
        string? DbContextParameter,
        string? ConverterParameter
    );

    private static RegistryModel Read(Compilation compilation, CancellationToken cancellationToken)
    {
        var registry = compilation.GetTypeByMetadataName(RegistryName) is { DeclaringSyntaxReferences.Length: > 0 } declared ? declared : null;
        var primitivesReferenced = compilation.GetTypeByMetadataName(Library + ".IDomainPrimitive`2") is not null;
        var hasDapper = primitivesReferenced && compilation.GetTypeByMetadataName("Dapper.SqlMapper") is not null;
        var hasMetrics = primitivesReferenced && compilation.GetTypeByMetadataName("OpenTelemetry.Metrics.MeterProviderBuilder") is not null;
        var hasTracing = primitivesReferenced && compilation.GetTypeByMetadataName("OpenTelemetry.Trace.TracerProviderBuilder") is not null;
        if (!primitivesReferenced || registry is null && !hasDapper)
            return new(false, false, hasMetrics, hasTracing, EquatableArray<PrimitiveEntry>.Empty, EquatableArray<string>.Empty, EquatableArray<string>.Empty, null, null);

        var primitives = new List<PrimitiveEntry>();
        var valueObjects = new List<string>();
        foreach (var type in Types(compilation, Library, cancellationToken))
        {
            // A type of this compilation is a domain primitive only when it is a record (the generator leaves anything else alone). A record
            // struct of a referenced assembly isn't a record in metadata (only a record class has a <Clone>$ to tell), so there the interfaces decide.
            if (type is not { IsGenericType: false, IsFileLocal: false, TypeKind: TypeKind.Class or TypeKind.Struct }) continue;
            if (!type.IsRecord && SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, compilation.Assembly)) continue;
            if (!compilation.IsSymbolAccessibleWithin(type, compilation.Assembly)) continue;
            var name = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (IsValueObject(type))
            {
                valueObjects.Add(name);
                continue;
            }

            if (PrimitiveReader.PrimitiveValueOf(type, compilation) is not { } value) continue;
            var isStrongId = IsStrongId(type);
            primitives.Add(new(name, ValueTypeInfo.From(value, compilation).Display, isStrongId && value.MetadataName == "Guid", isStrongId));
        }

        var dbContexts = new List<string>();
        string? dbContextParameter = null;
        string? converterParameter = null;
        if (registry is not null
         && registry.GetMembers("ForEachDbContext").OfType<IMethodSymbol>().FirstOrDefault() is { IsPartialDefinition: true, Parameters: [var dbContextVisitor] }
         && compilation.GetTypeByMetadataName("Microsoft.EntityFrameworkCore.DbContext") is { } dbContext)
        {
            dbContextParameter = dbContextVisitor.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            foreach (var type in Types(compilation, "Microsoft.EntityFrameworkCore", cancellationToken))
            {
                if (type is not { TypeKind: TypeKind.Class, IsAbstract: false, IsGenericType: false, IsFileLocal: false }) continue;
                if (!compilation.IsSymbolAccessibleWithin(type, compilation.Assembly) || !DerivesFrom(type, dbContext)) continue;
                dbContexts.Add(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            }
        }

        if (registry?.GetMembers("ForEachConverter").OfType<IMethodSymbol>().FirstOrDefault() is { IsPartialDefinition: true, Parameters: [var converterVisitor] })
            converterParameter = converterVisitor.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return new(
            registry is not null,
            hasDapper,
            hasMetrics,
            hasTracing,
            primitives.OrderBy(static z => z.Type, StringComparer.Ordinal).ToEquatableArray(),
            valueObjects.OrderBy(static z => z, StringComparer.Ordinal).ToEquatableArray(),
            dbContexts.OrderBy(static z => z, StringComparer.Ordinal).ToEquatableArray(),
            dbContextParameter,
            converterParameter
        );
    }

    private static bool IsValueObject(INamedTypeSymbol type)
    {
        foreach (var candidate in type.AllInterfaces)
        {
            if (candidate is { Name: "IValueObject", TypeArguments.Length: 1 } && candidate.ContainingNamespace?.ToDisplayString() == Library) return true;
        }

        var kind = PrimitiveKind(type);
        return kind is "ValueObject" or "Money" or "DateRange" or "TimeRange";
    }

    private static bool IsStrongId(INamedTypeSymbol type)
    {
        foreach (var candidate in type.AllInterfaces)
        {
            if (candidate is { Name: "IStrongId", TypeArguments.Length: 2 } && candidate.ContainingNamespace?.ToDisplayString() == Library) return true;
        }

        return PrimitiveKind(type) == "StrongId";
    }

    // The kind of a domain primitive of this compilation, from its primitive attribute.
    private static string? PrimitiveKind(INamedTypeSymbol type)
    {
        foreach (var attribute in type.GetAttributes())
        {
            if (PrimitiveReader.IsPrimitiveAttribute(attribute.AttributeClass)) return PrimitiveReader.KindOf(attribute.AttributeClass!);
        }

        return null;
    }

    // The types of the compilation, and of every referenced assembly that references the library (by its assembly name).
    private static IEnumerable<INamedTypeSymbol> Types(Compilation compilation, string library, CancellationToken cancellationToken)
    {
        foreach (var type in Types(compilation.Assembly.GlobalNamespace, cancellationToken))
        {
            yield return type;
        }

        foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (assembly.Name != library && !References(assembly, library)) continue;
            foreach (var type in Types(assembly.GlobalNamespace, cancellationToken))
            {
                yield return type;
            }
        }
    }

    private static bool References(IAssemblySymbol assembly, string library)
    {
        foreach (var module in assembly.Modules)
        {
            foreach (var reference in module.ReferencedAssemblies)
            {
                if (reference.Name == library) return true;
            }
        }

        return false;
    }

    private static IEnumerable<INamedTypeSymbol> Types(INamespaceOrTypeSymbol container, CancellationToken cancellationToken)
    {
        foreach (var member in container.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (member is INamespaceSymbol @namespace)
            {
                foreach (var type in Types(@namespace, cancellationToken))
                {
                    yield return type;
                }
            }
            else if (member is INamedTypeSymbol type)
            {
                yield return type;
                foreach (var nested in Types(type, cancellationToken))
                {
                    yield return nested;
                }
            }
        }
    }

    private static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType)) return true;
        }

        return false;
    }

    private static string ConverterName(string type) =>
        new string([.. type.Replace("global::", "").Select(static c => char.IsLetterOrDigit(c) ? c : '_')]) + "ValueConverter";

    private static string WriteRegistry(RegistryModel model, string openApi, string graphQL)
    {
        var writer = new SourceWriter();
        writer.Line("// <auto-generated/>");
        writer.Line("#nullable enable");
        writer.Open($"namespace {RegistryNamespace}");
        writer.Open("internal static partial class LaunchPadPrimitives");
        writer.Line("/// <summary>LaunchPadPrimitivesOpenApiRepresentation: Component (a named schema) or Inline (the value's schema in place).</summary>");
        writer.Line($"public const string OpenApiRepresentation = {Names.Literal(openApi)};");
        writer.Line();
        writer.Line("/// <summary>LaunchPadPrimitivesGraphQLRepresentation: Value (the scalar of the value, ID for a strong ID) or Named (a scalar named after the domain primitive).</summary>");
        writer.Line($"public const string GraphQLRepresentation = {Names.Literal(graphQL)};");
        writer.Line();
        writer.Line("/// <summary>Every domain primitive with one value the application declares or references.</summary>");
        writer.Line($"public static global::System.Collections.Generic.IReadOnlyList<{Names.Description}> All {{ get; }} =");
        writer.Line("[");
        foreach (var primitive in model.Primitives)
        {
            writer.Line($"    {primitive.Type}.Description,");
        }

        writer.Line("];");
        writer.Line();
        writer.Line("/// <summary>Every value object the application declares or references.</summary>");
        writer.Line($"public static global::System.Collections.Generic.IReadOnlyList<{Names.ValueObjectDescription}> ValueObjects {{ get; }} =");
        writer.Line("[");
        foreach (var valueObject in model.ValueObjects)
        {
            writer.Line($"    {valueObject}.Description,");
        }

        writer.Line("];");
        if (model.DbContextParameter is { } dbContextParameter)
        {
            writer.Line();
            writer.Open($"static partial void ForEachDbContext({dbContextParameter} visitor)");
            foreach (var dbContext in model.DbContexts)
            {
                writer.Line($"visitor.Visit<{dbContext}>();");
            }

            writer.Close();
        }

        if (model.ConverterParameter is { } converterParameter)
        {
            writer.Line();
            writer.Open($"static partial void ForEachConverter({converterParameter} visitor)");
            foreach (var primitive in model.Primitives)
            {
                writer.Line(
                    primitive.IsGuidStrongId
                        ? $"visitor.VisitGuidStrongId<{primitive.Type}, LaunchPadPrimitivesValueConverters.{ConverterName(primitive.Type)}>();"
                        : $"visitor.Visit<{primitive.Type}, {primitive.Value}, LaunchPadPrimitivesValueConverters.{ConverterName(primitive.Type)}>();"
                );
            }

            writer.Close();
        }

        writer.Close();
        if (model.ConverterParameter is not null)
        {
            writer.Line();
            writer.Line("/// <summary>A value converter per domain primitive: an expression tree can't call a static abstract member, so each names its type.</summary>");
            writer.Open("internal static class LaunchPadPrimitivesValueConverters");
            foreach (var primitive in model.Primitives)
            {
                var name = ConverterName(primitive.Type);
                writer.Line(
                    $"public sealed class {name}() : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<{primitive.Type}, {primitive.Value}>(value => value.Value, value => {primitive.Type}.Create(value), LaunchPadPrimitivesEntityFramework.Hints({primitive.Type}.Description));"
                );
            }

            writer.Close();
        }

        writer.Close();
        return writer.ToString();
    }

    private static string WriteDapper(RegistryModel model)
    {
        var writer = new SourceWriter();
        writer.Line("// <auto-generated/>");
        writer.Line("#nullable enable");
        writer.Open("namespace Rocket.Surgery.LaunchPad.Primitives.Generated");
        writer.Line("/// <summary>Registers a Dapper type handler for every domain primitive the assembly declares or references, when it loads.</summary>");
        writer.Line($"[{Names.GeneratedCode}, global::Microsoft.CodeAnalysis.Embedded, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        writer.Open("internal static class LaunchPadPrimitivesDapper");
        writer.Line("[global::System.Runtime.CompilerServices.ModuleInitializer]");
        writer.Open("internal static void Register()");
        foreach (var primitive in model.Primitives)
        {
            writer.Line($"global::Dapper.SqlMapper.AddTypeHandler(new DomainPrimitiveTypeHandler<{primitive.Type}, {primitive.Value}>());");
        }

        writer.Close();
        writer.Close();
        writer.Line();
        writer.Line("/// <summary>Stores a domain primitive as its value, and reads it back through its rules.</summary>");
        writer.Line($"[{Names.GeneratedCode}, global::Microsoft.CodeAnalysis.Embedded, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        writer.Line("internal sealed class DomainPrimitiveTypeHandler<TPrimitive, TValue> : global::Dapper.SqlMapper.TypeHandler<TPrimitive>");
        writer.Line($"    where TPrimitive : {Names.Primitives}.IDomainPrimitive<TPrimitive, TValue>");
        writer.Line("    where TValue : notnull");
        writer.Open();
        writer.Line("public override void SetValue(global::System.Data.IDbDataParameter parameter, TPrimitive? value) =>");
        writer.Line("    parameter.Value = value is null || value.IsDefault ? global::System.DBNull.Value : value.Value;");
        writer.Line();
        writer.Open("public override TPrimitive? Parse(object value)");
        writer.Line("if (value is null or global::System.DBNull) return default;");
        writer.Line("var typed = value switch");
        writer.Open();
        writer.Line("TValue same => same,");
        writer.Line("string text when typeof(TValue) == typeof(global::System.Guid) => (TValue)(object)global::System.Guid.Parse(text),");
        writer.Line("byte[] bytes when typeof(TValue) == typeof(global::System.Guid) => (TValue)(object)new global::System.Guid(bytes),");
        writer.Line("global::System.DateTime date when typeof(TValue) == typeof(global::System.DateOnly) => (TValue)(object)global::System.DateOnly.FromDateTime(date),");
        writer.Line("global::System.DateTime time when typeof(TValue) == typeof(global::System.TimeOnly) => (TValue)(object)global::System.TimeOnly.FromDateTime(time),");
        writer.Line("global::System.TimeSpan span when typeof(TValue) == typeof(global::System.TimeOnly) => (TValue)(object)global::System.TimeOnly.FromTimeSpan(span),");
        // A provider that stores dates and times as text (SQLite) reads them back as strings.
        writer.Line("string text when typeof(TValue) == typeof(global::System.DateOnly) => (TValue)(object)global::System.DateOnly.Parse(text, global::System.Globalization.CultureInfo.InvariantCulture),");
        writer.Line("string text when typeof(TValue) == typeof(global::System.TimeOnly) => (TValue)(object)global::System.TimeOnly.Parse(text, global::System.Globalization.CultureInfo.InvariantCulture),");
        writer.Line("global::System.DateTime moment when typeof(TValue) == typeof(global::System.DateTimeOffset) => (TValue)(object)new global::System.DateTimeOffset(global::System.DateTime.SpecifyKind(moment, global::System.DateTimeKind.Utc)),");
        writer.Line("string text when typeof(TValue) == typeof(global::System.DateTimeOffset) => (TValue)(object)global::System.DateTimeOffset.Parse(text, global::System.Globalization.CultureInfo.InvariantCulture),");
        writer.Line("global::System.IConvertible convertible => (TValue)convertible.ToType(typeof(TValue), global::System.Globalization.CultureInfo.InvariantCulture),");
        writer.Line("_ => throw new global::System.Data.DataException($\"A {value.GetType()} can't be read as a {typeof(TPrimitive).Name}.\"),");
        writer.Close(";");
        writer.Line($"return TPrimitive.TryCreate(typed, out var result, out var error) ? result : throw new global::System.Data.DataException($\"[{{error.Code}}] {{error.Message}}\");");
        writer.Close();
        writer.Close();
        writer.Close();
        return writer.ToString();
    }

    private static string WriteInstrumentation(RegistryModel model)
    {
        var writer = new SourceWriter();
        writer.Line("// <auto-generated/>");
        writer.Line("#nullable enable");
        if (model.HasMetrics)
        {
            writer.Open("namespace OpenTelemetry.Metrics");
            writer.Line("/// <summary>The metrics of Rocket.Surgery.LaunchPad.Primitives.</summary>");
            writer.Line($"[{Names.GeneratedCode}, global::Microsoft.CodeAnalysis.Embedded, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
            writer.Open("internal static class LaunchPadPrimitivesMeterProviderBuilderExtensions");
            writer.Line("/// <summary>Collects the validation and creation counters of domain primitives.</summary>");
            writer.Line("public static global::OpenTelemetry.Metrics.MeterProviderBuilder AddLaunchPadPrimitivesInstrumentation(this global::OpenTelemetry.Metrics.MeterProviderBuilder builder) =>");
            writer.Line($"    builder.AddMeter({Names.Instrumentation}.Name);");
            writer.Close();
            writer.Close();
        }

        if (model.HasTracing)
        {
            writer.Open("namespace OpenTelemetry.Trace");
            writer.Line("/// <summary>The traces of Rocket.Surgery.LaunchPad.Primitives.</summary>");
            writer.Line($"[{Names.GeneratedCode}, global::Microsoft.CodeAnalysis.Embedded, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
            writer.Open("internal static class LaunchPadPrimitivesTracerProviderBuilderExtensions");
            writer.Line("/// <summary>Adds an event to the current activity for every value a domain primitive rejects.</summary>");
            writer.Line("public static global::OpenTelemetry.Trace.TracerProviderBuilder AddLaunchPadPrimitivesInstrumentation(this global::OpenTelemetry.Trace.TracerProviderBuilder builder) =>");
            writer.Line($"    builder.AddSource({Names.Instrumentation}.Name);");
            writer.Close();
            writer.Close();
        }

        return writer.ToString();
    }
}
