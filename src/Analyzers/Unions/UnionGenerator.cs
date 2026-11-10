using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     Implements portable unions: a <c>partial struct</c> with <c>[System.Runtime.CompilerServices.Union]</c> and one <c>partial</c> constructor per case.
///     It writes what a C# 15 <c>union</c> declaration lowers to (<c>IUnion</c>, the constructors and <c>Value</c>), plus <c>HasValue</c>,
///     <c>TryGetValue</c> and implicit conversions from each case, so the same declaration works on C# 14 and C# 15. Before .NET 11 the attribute and
///     <c>IUnion</c> come from the Polyfill package (LPAD0012 when they're missing). See <c>docs/design/union-types.md</c>.
/// </summary>
[Generator]
public class UnionGenerator : IIncrementalGenerator
{
    internal const string UnionAttributeMetadataName = UnionNames.UnionAttribute;

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var unions = context
                    .SyntaxProvider
                    .ForAttributeWithMetadataName(
                         UnionAttributeMetadataName,
                         static (node, _) => node is TypeDeclarationSyntax declaration && HasPartialConstructor(declaration),
                         static (syntaxContext, cancellationToken) => CreateModel(syntaxContext, cancellationToken)
                     )
                    .Where(static model => model is { });

        context.RegisterSourceOutput(
            unions,
            static (output, model) =>
            {
                foreach (var diagnostic in model!.Diagnostics)
                {
                    output.ReportDiagnostic(diagnostic.ToDiagnostic());
                }

                if (model.Source is { } source)
                {
                    output.AddSource(model.HintName, source);
                }
            }
        );
    }

    internal static bool HasPartialConstructor(TypeDeclarationSyntax declaration)
    {
        foreach (var member in declaration.Members)
        {
            if (member is ConstructorDeclarationSyntax constructor && IsPartialDefinition(constructor)) return true;
        }

        return false;
    }

    internal static bool IsPartialDefinition(ConstructorDeclarationSyntax constructor) =>
        constructor.Modifiers.Any(SyntaxKind.PartialKeyword) && constructor is { Body: null, ExpressionBody: null, };

    private static UnionModel? CreateModel(GeneratorAttributeSyntaxContext syntaxContext, CancellationToken cancellationToken)
    {
        if (syntaxContext.TargetSymbol is not INamedTypeSymbol symbol) return null;
        var declaration = (TypeDeclarationSyntax)syntaxContext.TargetNode;

        // Partial types have several declarations; only the first one carrying partial constructors generates, so the output is written once.
        var firstDeclaration = symbol
                              .DeclaringSyntaxReferences
                              .Select(reference => reference.GetSyntax(cancellationToken))
                              .OfType<TypeDeclarationSyntax>()
                              .First(HasPartialConstructor);
        if (firstDeclaration != declaration) return null;

        var diagnostics = ImmutableArray.CreateBuilder<DiagnosticInfo>();
        var hintName = $"{symbol.GetFullMetadataName().Replace('`', '_').Replace('+', '.')}.Union.g.cs";

        if (symbol.TypeKind != TypeKind.Struct)
        {
            diagnostics.Add(DiagnosticInfo.Create(UnionDiagnostics.UnionMustBeStruct, declaration.Identifier.GetLocation(), symbol.Name));
        }

        for (var containing = symbol.ContainingType; containing is { }; containing = containing.ContainingType)
        {
            if (containing.DeclaringSyntaxReferences.Select(reference => reference.GetSyntax(cancellationToken)).OfType<TypeDeclarationSyntax>().Any(
                    syntax => syntax.Modifiers.Any(SyntaxKind.PartialKeyword)
                ))
            {
                continue;
            }

            diagnostics.Add(DiagnosticInfo.Create(UnionDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), containing.GetFullMetadataName()));
        }

        var cases = ImmutableArray.CreateBuilder<UnionCase>();
        foreach (var constructorSyntax in symbol
                                         .DeclaringSyntaxReferences
                                         .Select(reference => reference.GetSyntax(cancellationToken))
                                         .OfType<TypeDeclarationSyntax>()
                                         .SelectMany(syntax => syntax.Members.OfType<ConstructorDeclarationSyntax>())
                                         .Where(IsPartialDefinition))
        {
            if (constructorSyntax.ParameterList.Parameters.Count != 1)
            {
                diagnostics.Add(
                    DiagnosticInfo.Create(
                        UnionDiagnostics.UnionCaseConstructorMustTakeOneParameter,
                        constructorSyntax.Identifier.GetLocation(),
                        symbol.Name,
                        constructorSyntax.ParameterList.Parameters.Count
                    )
                );
                continue;
            }

            if (syntaxContext.SemanticModel.GetDeclaredSymbol(constructorSyntax, cancellationToken) is not { Parameters: [var parameter,], } constructor)
            {
                continue;
            }

            // The user implemented this case themselves.
            if (constructor.PartialImplementationPart is { }) continue;

            cases.Add(CreateCase(constructor, parameter));
        }

        return diagnostics.Count > 0 || cases.Count == 0
            ? new(hintName, null, new(diagnostics.ToImmutable()))
            : new(hintName, Emit(symbol, cases.ToImmutable()), new(diagnostics.ToImmutable()));
    }

    private static UnionCase CreateCase(IMethodSymbol constructor, IParameterSymbol parameter)
    {
        var type = parameter.Type;
        var declaredType = type.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNullable);
        // The type a pattern tests: `x is string?` doesn't parse and `x is int?` never matches a boxed int, so drop the nullable annotation.
        var testedType = type switch
        {
            INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T, TypeArguments: [var underlying,], } =>
                underlying.ToDisplayString(SymbolDisplayFormats.FullyQualified),
            _ => type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(SymbolDisplayFormats.FullyQualified),
        };
        // C# doesn't allow a user-defined conversion from an interface or a base class (CS0552, CS0553); C# 15 still converts those through the union.
        var canConvert = constructor.DeclaredAccessibility == Accessibility.Public
         && type.TypeKind != TypeKind.Interface
         && type.SpecialType is not ( SpecialType.System_Object or SpecialType.System_ValueType );

        return new(
            declaredType,
            testedType,
            parameter.Name,
            AccessibilityText(constructor.DeclaredAccessibility),
            canConvert,
            type.TypeKind == TypeKind.TypeParameter
        );
    }

    private static string AccessibilityText(Accessibility accessibility) =>
        accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.NotApplicable => "",
            _ => "",
        };

    private static string Emit(INamedTypeSymbol symbol, ImmutableArray<UnionCase> cases)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable enable");
        builder.AppendLine();

        var containingTypes = new List<INamedTypeSymbol>();
        for (var containing = symbol.ContainingType; containing is { }; containing = containing.ContainingType)
        {
            containingTypes.Insert(0, containing);
        }

        var hasNamespace = symbol.ContainingNamespace is { IsGlobalNamespace: false, };
        if (hasNamespace)
        {
            builder.Append("namespace ").Append(symbol.ContainingNamespace.ToDisplayString()).AppendLine();
            builder.AppendLine("{");
        }

        foreach (var containing in containingTypes)
        {
            builder.Append("partial ").Append(TypeKeyword(containing)).Append(' ').Append(NameWithTypeParameters(containing)).AppendLine();
            builder.AppendLine("{");
        }

        var name = symbol.Name;
        builder
           .Append(symbol.IsReadOnly ? "readonly " : "")
           .Append("partial ")
           .Append(TypeKeyword(symbol))
           .Append(' ')
           .Append(NameWithTypeParameters(symbol))
           .AppendLine(" : global::System.Runtime.CompilerServices.IUnion");
        builder.AppendLine("{");
        builder.AppendLine("    private readonly object? _value;");
        builder.AppendLine();

        foreach (var unionCase in cases)
        {
            builder
               .Append("    ")
               .Append(unionCase.Accessibility)
               .Append(" partial ")
               .Append(name)
               .Append('(')
               .Append(unionCase.DeclaredType)
               .Append(' ')
               .Append(Identifier(unionCase.ParameterName))
               .Append(") => _value = ")
               .Append(Identifier(unionCase.ParameterName))
               .AppendLine(";");
        }

        builder.AppendLine();
        builder.AppendLine("    /// <summary>The case value, or <see langword=\"null\" /> when the union is <c>default</c> or holds a null case value.</summary>");
        builder.AppendLine("    public object? Value => _value;");
        builder.AppendLine();
        builder.AppendLine("    /// <summary>Whether the union holds a non-null case value.</summary>");
        builder.AppendLine("    public bool HasValue => _value is not null;");

        foreach (var unionCase in cases)
        {
            builder.AppendLine();
            builder.Append("    /// <summary>Gets the case value when it is a ").Append(DocReference(unionCase)).AppendLine(".</summary>");
            builder
               .Append("    public bool TryGetValue([global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ")
               .Append(unionCase.DeclaredType)
               .AppendLine(" value)");
            builder.AppendLine("    {");
            builder.Append("        if (_value is ").Append(unionCase.TestedType).AppendLine(" caseValue)");
            builder.AppendLine("        {");
            builder.AppendLine("            value = caseValue;");
            builder.AppendLine("            return true;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        value = default!;");
            builder.AppendLine("        return false;");
            builder.AppendLine("    }");
        }

        foreach (var unionCase in cases.Where(unionCase => unionCase.CanConvert))
        {
            builder.AppendLine();
            builder.Append("    /// <summary>Creates the union from a ").Append(DocReference(unionCase)).AppendLine(".</summary>");
            builder
               .Append("    public static implicit operator ")
               .Append(NameWithTypeParameters(symbol))
               .Append('(')
               .Append(unionCase.DeclaredType)
               .Append(" value) => new(value);")
               .AppendLine();
        }

        builder.AppendLine("}");

        for (var i = 0; i < containingTypes.Count; i++)
        {
            builder.AppendLine("}");
        }

        if (hasNamespace) builder.AppendLine("}");

        return builder.ToString();
    }

    private static string Identifier(string name) => SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ? "@" + name : name;

    // A cref names generic types with braces (global::Foo<T> becomes global::Foo{T}); a type parameter is a typeparamref.
    private static string DocReference(UnionCase unionCase) =>
        unionCase.IsTypeParameter
            ? $"<typeparamref name=\"{unionCase.TestedType}\" />"
            : $"<see cref=\"{unionCase.TestedType.Replace('<', '{').Replace('>', '}')}\" />";

    private static string TypeKeyword(INamedTypeSymbol symbol) =>
        symbol switch
        {
            { IsRecord: true, TypeKind: TypeKind.Struct, } => "record struct",
            { IsRecord: true, } => "record",
            { TypeKind: TypeKind.Struct, } => "struct",
            { TypeKind: TypeKind.Interface, } => "interface",
            _ => "class",
        };

    private static string NameWithTypeParameters(INamedTypeSymbol symbol) =>
        symbol.TypeParameters.IsEmpty ? symbol.Name : $"{symbol.Name}<{string.Join(", ", symbol.TypeParameters.Select(parameter => parameter.Name))}>";

    private sealed record UnionCase(string DeclaredType, string TestedType, string ParameterName, string Accessibility, bool CanConvert, bool IsTypeParameter);

    private sealed record UnionModel(string HintName, string? Source, EquatableArray<DiagnosticInfo> Diagnostics);
}

internal static class SymbolDisplayFormats
{
    public static SymbolDisplayFormat FullyQualified { get; } = SymbolDisplayFormat.FullyQualifiedFormat;

    public static SymbolDisplayFormat FullyQualifiedNullable { get; } = SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(
        SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
    );
}

/// <summary>A diagnostic without its <see cref="Location" /> tree, so a generator model holding it compares by value.</summary>
internal sealed record DiagnosticInfo(DiagnosticDescriptor Descriptor, string FilePath, TextSpan Span, LinePositionSpan LineSpan, EquatableArray<string> Arguments)
{
    public static DiagnosticInfo Create(DiagnosticDescriptor descriptor, Location location, params object?[] arguments)
    {
        var lineSpan = location.GetLineSpan();
        return new(
            descriptor,
            lineSpan.Path,
            location.SourceSpan,
            lineSpan.Span,
            new([.. arguments.Select(argument => argument?.ToString() ?? "")])
        );
    }

    public Diagnostic ToDiagnostic() =>
        Diagnostic.Create(Descriptor, Location.Create(FilePath, Span, LineSpan), Arguments.AsImmutableArray().Cast<object?>().ToArray());
}

/// <summary>An immutable array with value equality, so incremental generator models compare by content.</summary>
internal readonly struct EquatableArray<T>(ImmutableArray<T> array) : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    private readonly ImmutableArray<T> _array = array;

    public ImmutableArray<T> AsImmutableArray() => _array.IsDefault ? [] : _array;

    public bool Equals(EquatableArray<T> other) => AsImmutableArray().SequenceEqual(other.AsImmutableArray());

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        var hash = 17;
        foreach (var item in AsImmutableArray())
        {
            hash = unchecked(hash * 31 + ( item?.GetHashCode() ?? 0 ));
        }

        return hash;
    }

    public IEnumerator<T> GetEnumerator() => ( (IEnumerable<T>)AsImmutableArray() ).GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
