using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     Writes the mutable view model of each type marked <c>[Mutable]</c>: a class next to the type with a settable property for each of its
///     properties, <c>From(value)</c> and <c>ToImmutable()</c>. The type can be a class, a record, a record struct or a readonly record struct.
/// </summary>
[Generator]
public class MutableGenerator : IIncrementalGenerator
{
    private const string MutableAttribute = "Rocket.Surgery.LaunchPad.Foundation.MutableAttribute";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context
                    .SyntaxProvider
                    .ForAttributeWithMetadataName(
                         MutableAttribute,
                         static (node, _) => node is TypeDeclarationSyntax,
                         static (syntaxContext, cancellationToken) => Read(syntaxContext, cancellationToken)
                     )
                    .WithTrackingName("Mutable");

        context.RegisterSourceOutput(models, static (output, models) => Emit(output, models));
    }

    private static EquatableArray<MutableModel> Read(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.TargetSymbol is not INamedTypeSymbol type) return EquatableArray<MutableModel>.Empty;
        var compilation = context.SemanticModel.Compilation;
        var nonPartial = FirstNonPartialContainer(type);
        var properties = Properties(type, compilation);
        var constructor = Constructor(type, properties, compilation);
        var arguments = ImmutableArray.CreateBuilder<string>();
        if (constructor is { })
        {
            foreach (var parameter in constructor.Parameters)
            {
                arguments.Add(FindProperty(properties, parameter.Name)!.Name);
            }
        }

        var members = ImmutableArray.CreateBuilder<MutableProperty>(properties.Count);
        foreach (var property in properties)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var inConstructor = arguments.Contains(property.Name);
            var settable = property.SetMethod is { } setter && compilation.IsSymbolAccessibleWithin(setter, compilation.Assembly);
            // a property the type can't be made with (no constructor parameter, no setter) can't come back from the view model
            if (!inConstructor && !settable) continue;
            members.Add(new(property.Name, property.Type.FullyQualifiedNullable(), inConstructor));
        }

        var models = ImmutableArray.CreateBuilder<MutableModel>();
        foreach (var attribute in context.Attributes)
        {
            var prefix = attribute.ConstructorArguments.Length > 0 ? attribute.ConstructorArguments[0].Value as string ?? "" : "";
            var suffix = attribute.ConstructorArguments.Length > 1 ? attribute.ConstructorArguments[1].Value as string ?? "ViewModel" : "ViewModel";
            models.Add(
                new(
                    type.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : null,
                    Containers(type),
                    Accessibility(type.DeclaredAccessibility),
                    prefix + type.Name + suffix,
                    type.FullyQualified(),
                    Hint(type),
                    new(members.ToImmutable()),
                    new(arguments.ToImmutable()),
                    nonPartial?.Name,
                    nonPartial is { } ? LocationInfo.From(nonPartial) : null
                )
            );
        }

        return new(models.ToImmutable());
    }

    // The public instance properties of the type and its base types, the type's own first; a property a derived type hides is read once.
    private static List<IPropertySymbol> Properties(INamedTypeSymbol type, Compilation compilation)
    {
        var properties = new List<IPropertySymbol>();
        for (var current = type; current is { SpecialType: not SpecialType.System_Object and not SpecialType.System_ValueType }; current = current.BaseType)
        {
            foreach (var member in current.GetMembers())
            {
                if (member is not IPropertySymbol { IsStatic: false, IsIndexer: false, DeclaredAccessibility: Microsoft.CodeAnalysis.Accessibility.Public, GetMethod: { } } property) continue;
                if (!compilation.IsSymbolAccessibleWithin(property.GetMethod, compilation.Assembly)) continue;
                if (FindProperty(properties, property.Name) is { }) continue;
                properties.Add(property);
            }
        }

        return properties;
    }

    // The accessible constructor with the most parameters that each name a property: a positional record's primary constructor, say. A copy
    // constructor is left out.
    private static IMethodSymbol? Constructor(INamedTypeSymbol type, List<IPropertySymbol> properties, Compilation compilation)
    {
        IMethodSymbol? best = null;
        foreach (var constructor in type.InstanceConstructors)
        {
            if (!compilation.IsSymbolAccessibleWithin(constructor, compilation.Assembly)) continue;
            if (best is { } && constructor.Parameters.Length <= best.Parameters.Length) continue;
            var matches = true;
            foreach (var parameter in constructor.Parameters)
            {
                if (SymbolEqualityComparer.Default.Equals(parameter.Type, type) || FindProperty(properties, parameter.Name) is null)
                {
                    matches = false;
                    break;
                }
            }

            if (matches) best = constructor;
        }

        return best;
    }

    private static IPropertySymbol? FindProperty(List<IPropertySymbol> properties, string name)
    {
        foreach (var property in properties)
        {
            if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase)) return property;
        }

        return null;
    }

    private static INamedTypeSymbol? FirstNonPartialContainer(INamedTypeSymbol type)
    {
        for (var current = type.ContainingType; current is { }; current = current.ContainingType)
        {
            if (!IsPartial(current)) return current;
        }

        return null;
    }

    private static bool IsPartial(INamedTypeSymbol type)
    {
        foreach (var reference in type.DeclaringSyntaxReferences)
        {
            if (reference.GetSyntax() is not TypeDeclarationSyntax declaration) continue;
            foreach (var modifier in declaration.Modifiers)
            {
                if (modifier.Text == "partial") return true;
            }
        }

        return false;
    }

    // The declarations of the types that contain the type, outermost first.
    private static EquatableArray<string> Containers(INamedTypeSymbol type)
    {
        var containers = new List<string>();
        for (var current = type.ContainingType; current is { }; current = current.ContainingType)
        {
            containers.Add(Declaration(current));
        }

        containers.Reverse();
        return containers.ToEquatableArray();
    }

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
        var declaration = new StringBuilder();
        if (type.IsStatic) declaration.Append("static ");
        declaration.Append("partial ").Append(keyword).Append(' ').Append(type.Name);
        if (type.TypeParameters.Length > 0)
        {
            declaration.Append('<');
            for (var index = 0; index < type.TypeParameters.Length; index++)
            {
                if (index > 0) declaration.Append(", ");
                declaration.Append(type.TypeParameters[index].Name);
            }

            declaration.Append('>');
        }

        return declaration.ToString();
    }

    // The file name part for the type: its full name, which two types of the same name in different containers don't share.
    private static string Hint(INamedTypeSymbol type)
    {
        var name = new StringBuilder(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)));
        for (var index = 0; index < name.Length; index++)
        {
            if (!char.IsLetterOrDigit(name[index]) && name[index] != '.' && name[index] != '_') name[index] = '_';
        }

        return name.ToString();
    }

    private static string Accessibility(Accessibility accessibility) => accessibility switch
    {
        Microsoft.CodeAnalysis.Accessibility.Public => "public",
        Microsoft.CodeAnalysis.Accessibility.Protected => "protected",
        Microsoft.CodeAnalysis.Accessibility.ProtectedOrInternal => "protected internal",
        Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal => "private protected",
        Microsoft.CodeAnalysis.Accessibility.Private => "private",
        Microsoft.CodeAnalysis.Accessibility.NotApplicable => throw new NotImplementedException(),
        Microsoft.CodeAnalysis.Accessibility.Internal => throw new NotImplementedException(),
        _ => "internal",
    };

    private static void Emit(SourceProductionContext output, EquatableArray<MutableModel> models)
    {
        foreach (var model in models)
        {
            if (model.NonPartialContainer is { } container)
            {
                output.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MustBePartial, model.NonPartialLocation?.ToLocation() ?? Location.None, container));
                continue;
            }

            output.AddSource(model.Hint + "." + model.Name + ".Mutable.g.cs", Write(model));
        }
    }

    private static string Write(MutableModel model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        // a view model starts empty: its non-nullable properties are set by the form, or by From
        sb.AppendLine("#pragma warning disable CS8618");
        if (model.Namespace is { } ns) sb.Append("namespace ").Append(ns).AppendLine(";");
        sb.AppendLine();

        var depth = 0;
        foreach (var container in model.Containers)
        {
            sb.Append(' ', depth * 4).AppendLine(container);
            sb.Append(' ', depth * 4).AppendLine("{");
            depth++;
        }

        var indent = new string(' ', depth * 4);
        var target = model.Target;
        var cref = target.Replace("global::", "").Replace('<', '{').Replace('>', '}');
        sb.Append(indent).AppendLine("/// <summary>");
        sb.Append(indent).Append("///     A mutable copy of <see cref=\"").Append(cref).AppendLine("\" />, for a form to bind to.");
        sb.Append(indent).AppendLine("/// </summary>");
        sb.Append(indent).AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"Rocket.Surgery.LaunchPad.Analyzers\", \"1.0.0.0\")]");
        sb.Append(indent)
          .Append(model.Accessibility)
          .Append(" partial class ")
          .Append(model.Name)
          .Append(" : global::Rocket.Surgery.LaunchPad.Foundation.IMutable<")
          .Append(target)
          .AppendLine(">");
        sb.Append(indent).AppendLine("{");

        foreach (var property in model.Properties)
        {
            sb.Append(indent).Append("    /// <inheritdoc cref=\"").Append(cref).Append('.').Append(property.Name).AppendLine("\" />");
            sb.Append(indent).Append("    public ").Append(property.Type).Append(' ').Append(property.Name).AppendLine(" { get; set; }");
            sb.AppendLine();
        }

        sb.Append(indent).AppendLine("    /// <summary>");
        sb.Append(indent).Append("    ///     Copies a <see cref=\"").Append(cref).AppendLine("\" /> into a new view model.");
        sb.Append(indent).AppendLine("    /// </summary>");
        sb.Append(indent).AppendLine("    /// <param name=\"value\">The value to copy.</param>");
        sb.Append(indent).AppendLine("    /// <returns>The view model.</returns>");
        sb.Append(indent).Append("    public static ").Append(model.Name).Append(" From(").Append(target).AppendLine(" value) => new()");
        sb.Append(indent).AppendLine("    {");
        foreach (var property in model.Properties)
        {
            sb.Append(indent).Append("        ").Append(property.Name).Append(" = value.").Append(property.Name).AppendLine(",");
        }

        sb.Append(indent).AppendLine("    };");
        sb.AppendLine();

        sb.Append(indent).AppendLine("    /// <summary>");
        sb.Append(indent).Append("    ///     Makes the <see cref=\"").Append(cref).AppendLine("\" /> from the current values.");
        sb.Append(indent).AppendLine("    /// </summary>");
        sb.Append(indent).AppendLine("    /// <returns>The immutable value.</returns>");
        sb.Append(indent).Append("    public ").Append(target).Append(" ToImmutable() => new ").Append(target).Append('(');
        for (var index = 0; index < model.ConstructorArguments.Length; index++)
        {
            if (index > 0) sb.Append(", ");
            sb.Append(model.ConstructorArguments[index]);
        }

        if (model.Properties.Length == model.ConstructorArguments.Length)
        {
            sb.AppendLine(");");
        }
        else
        {
            sb.AppendLine(")");
            sb.Append(indent).AppendLine("    {");
            foreach (var property in model.Properties)
            {
                if (property.InConstructor) continue;
                sb.Append(indent).Append("        ").Append(property.Name).Append(" = ").Append(property.Name).AppendLine(",");
            }

            sb.Append(indent).AppendLine("    };");
        }
        sb.AppendLine();
        sb.Append(indent).AppendLine("    object global::Rocket.Surgery.LaunchPad.Foundation.IMutable.ToImmutable() => ToImmutable();");
        sb.Append(indent).AppendLine("}");

        for (depth--; depth >= 0; depth--)
        {
            sb.Append(' ', depth * 4).AppendLine("}");
        }

        return sb.ToString();
    }
}

internal sealed record MutableProperty(string Name, string Type, bool InConstructor);

internal sealed record MutableModel(
    string? Namespace,
    EquatableArray<string> Containers,
    string Accessibility,
    string Name,
    string Target,
    string Hint,
    EquatableArray<MutableProperty> Properties,
    EquatableArray<string> ConstructorArguments,
    string? NonPartialContainer,
    LocationInfo? NonPartialLocation
);
