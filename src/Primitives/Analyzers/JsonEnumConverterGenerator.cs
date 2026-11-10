using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers;

/// <summary>
///     Registers a camel case <c>JsonStringEnumConverter&lt;TEnum&gt;</c> for every enum a JSON context of the compilation declares or reaches, so
///     LaunchPad writes enums as camel case strings without the reflection based <c>JsonStringEnumConverter</c>, in a JIT application and a native
///     AOT one alike.
/// </summary>
/// <remarks>
///     <para>
///         A JSON context is a class deriving from <c>JsonSerializerContext</c> with <c>[JsonSerializable]</c> attributes. The enums it reaches are
///         those the System.Text.Json source generator writes metadata for: the declared types, their public properties and fields, element and type
///         arguments, and base types. An enum with a <c>[JsonConverter]</c> of its own keeps it.
///     </para>
///     <para>
///         The converters are registered with <c>LaunchPadJsonEnumConverters</c> from a module initializer, and the <c>SystemTextJsonJoint</c> adds
///         them to the options it configures. Nothing is generated when the compilation doesn't reference Rocket.Surgery.LaunchPad.Primitives.
///     </para>
/// </remarks>
[Generator]
public sealed class JsonEnumConverterGenerator : IIncrementalGenerator
{
    private const string Registry = "Rocket.Surgery.LaunchPad.Primitives.LaunchPadJsonEnumConverters";
    private const string Context = "System.Text.Json.Serialization.JsonSerializerContext";
    private const string Serializable = "System.Text.Json.Serialization.JsonSerializableAttribute";
    private const string ConverterAttribute = "System.Text.Json.Serialization.JsonConverterAttribute";

    private static readonly SymbolDisplayFormat FullyQualified = SymbolDisplayFormat.FullyQualifiedFormat;

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enums = context.SyntaxProvider
                           .ForAttributeWithMetadataName(Serializable, static (node, _) => node is ClassDeclarationSyntax, static (ctx, ct) => Enums(ctx, ct))
                           .Collect()
                           .Select(static (all, _) => string.Join("\n", all.SelectMany(e => e.Split('\n')).Where(e => e.Length > 0).Distinct().OrderBy(e => e, StringComparer.Ordinal)));
        var hasRegistry = context.CompilationProvider.Select(static (compilation, _) => compilation.GetTypeByMetadataName(Registry) is { });

        context.RegisterSourceOutput(
            enums.Combine(hasRegistry),
            static (output, pair) =>
            {
                var (names, registry) = pair;
                if (!registry || names.Length == 0) return;
                output.AddSource("Rocket.Surgery.LaunchPad.Primitives.JsonEnumConverters.g.cs", Emit(names.Split('\n')));
            }
        );
    }

    // The enums one context reaches, as fully qualified names, one per line (a string, so the step is cached by value).
    private static string Enums(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.TargetSymbol is not INamedTypeSymbol type || !DerivesFrom(type, Context)) return "";
        var compilation = context.SemanticModel.Compilation;
        var visited = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var pending = new Stack<ITypeSymbol>(
            type.GetAttributes()
                .Where(a => a.AttributeClass?.ToDisplayString() == Serializable)
                .Select(a => a.ConstructorArguments.Length > 0 ? a.ConstructorArguments[0].Value as ITypeSymbol : null)
                .OfType<ITypeSymbol>()
        );
        var enums = new SortedSet<string>(StringComparer.Ordinal);
        while (pending.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var current = pending.Pop().WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            if (!visited.Add(current)) continue;
            if (current.TypeKind == TypeKind.Enum)
            {
                if (compilation.IsSymbolAccessibleWithin(current, compilation.Assembly)
                 && !current.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == ConverterAttribute))
                {
                    enums.Add(current.ToDisplayString(FullyQualified));
                }

                continue;
            }

            foreach (var reached in Reached(current)) pending.Push(reached);
        }

        return string.Join("\n", enums);
    }

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

    private static bool DerivesFrom(INamedTypeSymbol type, string baseType)
    {
        for (var current = type.BaseType; current is { }; current = current.BaseType)
        {
            if (current.ToDisplayString() == baseType) return true;
        }

        return false;
    }

    private static string Emit(IEnumerable<string> enums)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("#pragma warning disable");
        builder.AppendLine("namespace Rocket.Surgery.LaunchPad.Primitives.Generated");
        builder.AppendLine("{");
        builder.AppendLine("    /// <summary>Registers the camel case converters of the enums this assembly's JSON contexts reach.</summary>");
        builder.AppendLine("    [global::System.CodeDom.Compiler.GeneratedCode(\"Rocket.Surgery.LaunchPad.Primitives.Analyzers\", \"generated\")]");
        builder.AppendLine("    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        builder.AppendLine("    internal static class LaunchPadJsonEnumConverterRegistration");
        builder.AppendLine("    {");
        builder.AppendLine("        [global::System.Runtime.CompilerServices.ModuleInitializer]");
        builder.AppendLine("        internal static void Register()");
        builder.AppendLine("        {");
        foreach (var name in enums)
        {
            builder.Append("            global::").Append(Registry).Append(".Register(new global::System.Text.Json.Serialization.JsonStringEnumConverter<")
                   .Append(name)
                   .AppendLine(">(global::System.Text.Json.JsonNamingPolicy.CamelCase));");
        }

        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }
}
