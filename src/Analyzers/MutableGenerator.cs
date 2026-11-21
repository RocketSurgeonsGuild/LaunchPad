using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     Fills in each partial class marked <c>[Mutable(typeof(T))]</c> or <c>[Mutable&lt;T&gt;]</c> as the mutable view model of <c>T</c>: a
///     settable property for each of <c>T</c>'s properties the class doesn't declare itself, <c>From(value)</c>, <c>ToImmutable()</c> and an
///     implicit conversion each way. <c>T</c> can be a class, a record, a record struct or a readonly record struct. In a project that
///     references ZeroAlloc.Validation it also writes a nested <c>Validator</c> that validates the value the view model makes with <c>T</c>'s
///     validator, and names it on the view model with <c>[ValidatedBy]</c>.
/// </summary>
[Generator]
public class MutableGenerator : IIncrementalGenerator
{
    private const string MutableAttribute = "Rocket.Surgery.LaunchPad.Foundation.MutableAttribute";
    private const string GenericMutableAttribute = "Rocket.Surgery.LaunchPad.Foundation.MutableAttribute`1";
    private const string ValidatorFor = "ZeroAlloc.Validation.ValidatorFor`1";
    private const string ValidatedByAttribute = "Rocket.Surgery.LaunchPad.Foundation.ValidatedByAttribute";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context
                    .SyntaxProvider
                    .ForAttributeWithMetadataName(
                         MutableAttribute,
                         static (node, _) => IsClass(node),
                         static (syntaxContext, cancellationToken) => Read(syntaxContext, cancellationToken)
                     )
                    .WithTrackingName("Mutable");
        var genericModels = context
                           .SyntaxProvider
                           .ForAttributeWithMetadataName(
                                GenericMutableAttribute,
                                static (node, _) => IsClass(node),
                                static (syntaxContext, cancellationToken) => Read(syntaxContext, cancellationToken)
                            )
                           .WithTrackingName("MutableGeneric");

        context.RegisterSourceOutput(models, static (output, model) => Emit(output, model));
        context.RegisterSourceOutput(genericModels, static (output, model) => Emit(output, model));
    }

    // A class or a record class: the view model is a reference type a form can bind to.
    private static bool IsClass(SyntaxNode node) =>
        node is ClassDeclarationSyntax or RecordDeclarationSyntax { ClassOrStructKeyword.RawKind: not (int)SyntaxKind.StructKeyword };

    private static MutableModel? Read(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.TargetSymbol is not INamedTypeSymbol viewModel || context.Attributes is not [var attribute, ..])
        {
            return null;
        }
        if (attribute switch
        {
            { AttributeClass.TypeArguments: [INamedTypeSymbol argument] } => argument,
            { ConstructorArguments: [{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol argument }] } => argument,
            _ => null,
        } is not { } type)
        {
            return null;
        }

        var compilation = context.SemanticModel.Compilation;
        var nonPartial = IsPartial(viewModel) ? FirstNonPartialContainer(viewModel) : viewModel;
        var properties = Properties(type, compilation);
        var constructor = Constructor(type, properties, compilation);
        var arguments = ConstructorArguments(constructor, properties);
        var members = Members(viewModel, properties, arguments, compilation, cancellationToken);

        return new(
            viewModel.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : null,
            Containers(viewModel),
            Declaration(viewModel),
            viewModel.Name,
            type.FullyQualified(),
            Hint(viewModel),
            members,
            arguments,
            nonPartial?.Name,
            nonPartial is { } ? LocationInfo.From(nonPartial) : null,
            WritesValidator(viewModel, compilation)
        );
    }

    // A validator is written where ZeroAlloc.Validation and [ValidatedBy] are referenced, for a view model that isn't generic: an attribute
    // can't name a validator nested in an open generic type that could be created.
    private static bool WritesValidator(INamedTypeSymbol viewModel, Compilation compilation) =>
        !viewModel.IsGenericType
     && viewModel.GetMembers("Validator").Length == 0
     && compilation.GetTypeByMetadataName(ValidatorFor) is { }
     && compilation.GetTypeByMetadataName(ValidatedByAttribute) is { };

    // The properties the constructor's parameters set, in its order.
    private static EquatableArray<string> ConstructorArguments(IMethodSymbol? constructor, List<IPropertySymbol> properties)
    {
        var arguments = ImmutableArray.CreateBuilder<string>();
        if (constructor is null)
        {
            return EquatableArray<string>.Empty;
        }

        foreach (var parameter in constructor.Parameters)
        {
            arguments.Add(FindProperty(properties, parameter.Name)!.Name);
        }

        return new(arguments.ToImmutable());
    }

    // The view model's properties: each property the type can be made with, through its constructor or a setter it can reach. Another one
    // (no constructor parameter, no setter) couldn't come back from the view model. One the view model declares itself is used, not written.
    private static EquatableArray<MutableProperty> Members(
        INamedTypeSymbol viewModel,
        List<IPropertySymbol> properties,
        EquatableArray<string> arguments,
        Compilation compilation,
        CancellationToken cancellationToken
    )
    {
        var members = ImmutableArray.CreateBuilder<MutableProperty>(properties.Count);
        foreach (var property in properties)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var inConstructor = arguments.AsImmutableArray().Contains(property.Name);
            var settable = property.SetMethod is { } setter && compilation.IsSymbolAccessibleWithin(setter, compilation.Assembly);
            if (inConstructor || settable)
            {
                members.Add(new(property.Name, property.Type.FullyQualifiedNullable(), inConstructor, viewModel.GetMembers(property.Name).Length > 0));
            }
        }

        return new(members.ToImmutable());
    }

    // The public instance properties of the type and its base types, the type's own first; a property a derived type hides is read once.
    private static List<IPropertySymbol> Properties(INamedTypeSymbol type, Compilation compilation)
    {
        var properties = new List<IPropertySymbol>();
        for (var current = type; current is { SpecialType: not SpecialType.System_Object and not SpecialType.System_ValueType }; current = current.BaseType)
        {
            foreach (var member in current.GetMembers())
            {
                if (member is not IPropertySymbol { IsStatic: false, IsIndexer: false, DeclaredAccessibility: Accessibility.Public, GetMethod: { } } property)
                {
                    continue;
                }
                if (!compilation.IsSymbolAccessibleWithin(property.GetMethod, compilation.Assembly))
                {
                    continue;
                }
                if (FindProperty(properties, property.Name) is { })
                {
                    continue;
                }
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
            if (!compilation.IsSymbolAccessibleWithin(constructor, compilation.Assembly))
            {
                continue;
            }
            if (best is { } && constructor.Parameters.Length <= best.Parameters.Length)
            {
                continue;
            }
            var matches = true;
            foreach (var parameter in constructor.Parameters)
            {
                if (SymbolEqualityComparer.Default.Equals(parameter.Type, type) || FindProperty(properties, parameter.Name) is null)
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                best = constructor;
            }
        }

        return best;
    }

    private static IPropertySymbol? FindProperty(List<IPropertySymbol> properties, string name)
    {
        foreach (var property in properties)
        {
            if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return property;
            }
        }

        return null;
    }

    private static INamedTypeSymbol? FirstNonPartialContainer(INamedTypeSymbol type)
    {
        for (var current = type.ContainingType; current is { }; current = current.ContainingType)
        {
            if (!IsPartial(current))
            {
                return current;
            }
        }

        return null;
    }

    private static bool IsPartial(INamedTypeSymbol type)
    {
        foreach (var reference in type.DeclaringSyntaxReferences)
        {
            if (reference.GetSyntax() is not TypeDeclarationSyntax declaration)
            {
                continue;
            }
            foreach (var modifier in declaration.Modifiers)
            {
                if (modifier.Text == "partial")
                {
                    return true;
                }
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
        if (type.IsStatic)
        {
            declaration.Append("static ");
        }
        declaration.Append("partial ").Append(keyword).Append(' ').Append(type.Name);
        if (type.TypeParameters.Length > 0)
        {
            declaration.Append('<');
            for (var index = 0; index < type.TypeParameters.Length; index++)
            {
                if (index > 0)
                {
                    declaration.Append(", ");
                }
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
            if (!char.IsLetterOrDigit(name[index]) && name[index] != '.' && name[index] != '_')
            {
                name[index] = '_';
            }
        }

        return name.ToString();
    }

    private static void Emit(SourceProductionContext output, MutableModel? model)
    {
        if (model is null)
        {
            return;
        }
        if (model.NonPartialType is { } nonPartial)
        {
            output.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MustBePartial, model.NonPartialLocation?.ToLocation() ?? Location.None, nonPartial));
            return;
        }

        output.AddSource(model.Hint + ".Mutable.g.cs", Write(model));
    }

    private static string Write(MutableModel model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        // a view model starts empty: its non-nullable properties are set by the form, or by From
        sb.AppendLine("#pragma warning disable CS8618");
        if (model.Namespace is { } ns)
        {
            sb.Append("namespace ").Append(ns).AppendLine(";");
        }
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
        sb.Append(indent)
          .Append("[global::System.CodeDom.Compiler.GeneratedCode(\"")
          .Append(AssemblyInfo.Project.AssemblyName)
          .Append("\", \"")
          .Append(AssemblyInfo.Attributes.FileVersion)
          .AppendLine("\")]");
        if (model.WritesValidator)
        {
            sb.Append(indent).Append("[global::").Append(ValidatedByAttribute).Append("(typeof(").Append(model.Name).AppendLine(".Validator))]");
        }
        sb.Append(indent).AppendLine(model.Declaration);
        sb.Append(indent).AppendLine("{");
        WriteMembers(sb, model, indent, cref);
        sb.Append(indent).AppendLine("}");

        for (depth--; depth >= 0; depth--)
        {
            sb.Append(' ', depth * 4).AppendLine("}");
        }

        return sb.ToString();
    }

    // The view model's properties, From, ToImmutable and the implicit conversions.
    private static void WriteMembers(StringBuilder sb, MutableModel model, string indent, string cref)
    {
        var target = model.Target;
        foreach (var property in model.Properties)
        {
            if (property.Declared)
            {
                continue;
            }
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
        WriteToImmutable(sb, model, indent, cref);
        sb.AppendLine();
        WriteConversions(sb, model, indent, cref);
        if (model.WritesValidator)
        {
            sb.AppendLine();
            WriteValidator(sb, model, indent, cref);
        }
    }

    // The view model's validator: the value it makes is validated with the validator of that type, and the failures name the properties the
    // view model shares with it. Without that validator the view model is valid.
    private static void WriteValidator(StringBuilder sb, MutableModel model, string indent, string cref)
    {
        var target = model.Target;
        const string result = "global::ZeroAlloc.Validation.ValidationResult";
        sb.Append(indent).AppendLine("    /// <summary>");
        sb.Append(indent).Append("    ///     Validates the view model as the <see cref=\"").Append(cref).AppendLine("\" /> it makes, with that type's validator.");
        sb.Append(indent).AppendLine("    /// </summary>");
        sb.Append(indent).Append("    /// <param name=\"validator\">The validator of <see cref=\"").Append(cref).AppendLine("\" />; without one the view model is valid.</param>");
        sb.Append(indent)
          .Append("    public sealed class Validator(global::ZeroAlloc.Validation.ValidatorFor<")
          .Append(target)
          .Append(">? validator = null) : global::ZeroAlloc.Validation.ValidatorFor<")
          .Append(model.Name)
          .AppendLine(">");
        sb.Append(indent).AppendLine("    {");
        sb.Append(indent).AppendLine("        /// <inheritdoc />");
        sb.Append(indent)
          .Append("        public override ")
          .Append(result)
          .Append(" Validate(")
          .Append(model.Name)
          .Append(" instance) => validator?.Validate(instance.ToImmutable()) ?? new ")
          .Append(result)
          .AppendLine("([]);");
        sb.AppendLine();
        sb.Append(indent).AppendLine("        /// <inheritdoc />");
        sb.Append(indent)
          .Append("        public override global::System.Threading.Tasks.ValueTask<")
          .Append(result)
          .Append("> ValidateAsync(")
          .Append(model.Name)
          .Append(" instance, global::System.Threading.CancellationToken cancellationToken = default) => validator?.ValidateAsync(instance.ToImmutable(), cancellationToken) ?? new(new ")
          .Append(result)
          .AppendLine("([]));");
        sb.Append(indent).AppendLine("    }");
    }

    // The implicit conversions both ways: From and ToImmutable.
    private static void WriteConversions(StringBuilder sb, MutableModel model, string indent, string cref)
    {
        var target = model.Target;
        sb.Append(indent).AppendLine("    /// <summary>");
        sb.Append(indent).Append("    ///     Copies a <see cref=\"").Append(cref).AppendLine("\" /> into a new view model.");
        sb.Append(indent).AppendLine("    /// </summary>");
        sb.Append(indent).AppendLine("    /// <param name=\"value\">The value to copy.</param>");
        sb.Append(indent).Append("    public static implicit operator ").Append(model.Name).Append('(').Append(target).AppendLine(" value) => From(value);");
        sb.AppendLine();
        sb.Append(indent).AppendLine("    /// <summary>");
        sb.Append(indent).Append("    ///     Makes the <see cref=\"").Append(cref).AppendLine("\" /> from the view model's current values.");
        sb.Append(indent).AppendLine("    /// </summary>");
        sb.Append(indent).AppendLine("    /// <param name=\"viewModel\">The view model.</param>");
        sb.Append(indent).Append("    public static implicit operator ").Append(target).Append('(').Append(model.Name).AppendLine(" viewModel) => viewModel.ToImmutable();");
    }

    // ToImmutable: the type's constructor with the properties it takes, then an initializer with the others.
    private static void WriteToImmutable(StringBuilder sb, MutableModel model, string indent, string cref)
    {
        var target = model.Target;
        sb.Append(indent).AppendLine("    /// <summary>");
        sb.Append(indent).Append("    ///     Makes the <see cref=\"").Append(cref).AppendLine("\" /> from the current values.");
        sb.Append(indent).AppendLine("    /// </summary>");
        sb.Append(indent).AppendLine("    /// <returns>The immutable value.</returns>");
        sb.Append(indent).Append("    public ").Append(target).Append(" ToImmutable() => new ").Append(target).Append('(');
        for (var index = 0; index < model.ConstructorArguments.Length; index++)
        {
            if (index > 0)
            {
                sb.Append(", ");
            }
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
                if (property.InConstructor)
                {
                    continue;
                }
                sb.Append(indent).Append("        ").Append(property.Name).Append(" = ").Append(property.Name).AppendLine(",");
            }

            sb.Append(indent).AppendLine("    };");
        }
    }
}

internal sealed record MutableProperty(string Name, string Type, bool InConstructor, bool Declared);

internal sealed record MutableModel(
    string? Namespace,
    EquatableArray<string> Containers,
    string Declaration,
    string Name,
    string Target,
    string Hint,
    EquatableArray<MutableProperty> Properties,
    EquatableArray<string> ConstructorArguments,
    string? NonPartialType,
    LocationInfo? NonPartialLocation,
    bool WritesValidator
);
