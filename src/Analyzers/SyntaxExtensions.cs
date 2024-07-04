using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Analyzers;

internal static class SyntaxExtensions
{
    public static TypeSyntax EnsureNullable(this TypeSyntax typeSyntax, NullableAnnotation? annotation = null)
    {
        if (annotation.HasValue && annotation == NullableAnnotation.Annotated) return typeSyntax;
        return typeSyntax as NullableTypeSyntax ?? NullableType(typeSyntax);
    }

    public static TypeSyntax EnsureNotNullable(this TypeSyntax typeSyntax)
    {
        return typeSyntax is NullableTypeSyntax nts ? nts.ElementType : typeSyntax;
    }

    public static IEnumerable<TypeDeclarationSyntax> GetParentDeclarationsWithSelf(
        this TypeDeclarationSyntax source
    )
    {
        yield return source;
        var parent = source.Parent;
        while (parent is TypeDeclarationSyntax parentSyntax)
        {
            yield return parentSyntax;
            parent = parentSyntax.Parent;
        }
    }

    public static IEnumerable<INamedTypeSymbol> GetParentDeclarationsWithSelf(
        this INamedTypeSymbol source
    )
    {
        yield return source;
        var parent = source.ContainingType;
        while (parent is { } parentSymbol)
        {
            yield return parentSymbol;
            parent = parentSymbol.ContainingType;
        }
    }

    public static CompilationUnitSyntax AddSharedTrivia(this CompilationUnitSyntax source)
    {
        return source
              .WithLeadingTrivia(
                   Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)),
                   Trivia(
                       PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)
                          .WithErrorCodes(SeparatedList(List(DisabledWarnings.Value)))
                   )
               )
              .WithTrailingTrivia(
                   Trivia(
                       PragmaWarningDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)
                          .WithErrorCodes(SeparatedList(List(DisabledWarnings.Value)))
                   ),
                   Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)),
                   CarriageReturnLineFeed
               );
    }


    public static TypeDeclarationSyntax ReparentDeclaration(
        this TypeDeclarationSyntax classToNest,
        SourceProductionContext context,
        TypeDeclarationSyntax source
    )
    {
        var parent = source.Parent;
        while (parent is TypeDeclarationSyntax parentSyntax)
        {
            classToNest = ( parentSyntax is RecordDeclarationSyntax
                              ? (TypeDeclarationSyntax)RecordDeclaration(Token(SyntaxKind.RecordKeyword), parentSyntax.Identifier)
                              : ClassDeclaration(parentSyntax.Identifier) )
                         .WithModifiers(TokenList(parentSyntax.Modifiers.Select(z => z.WithoutTrivia())))
                         .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                         .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
                         .AddMembers(classToNest);

            if (!parentSyntax.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
                context.ReportDiagnostic(
                    Diagnostic.Create(GeneratorDiagnostics.MustBePartial, parentSyntax.Identifier.GetLocation(), parentSyntax.GetFullMetadataName())
                );

            parent = parentSyntax.Parent;
        }

        return classToNest;
    }

    public static string GetFullMetadataName(this TypeDeclarationSyntax? source)
    {
        if (source is null)
            return string.Empty;

        var sb = new StringBuilder(source.Identifier.Text);

        var parent = source.Parent;
        while (parent is { })
        {
            if (parent is TypeDeclarationSyntax tds)
            {
                sb.Insert(0, '+');
                sb.Insert(0, tds.Identifier.Text);
            }
            else if (parent is NamespaceDeclarationSyntax nds)
            {
                sb.Insert(0, '.');
                sb.Insert(0, nds.Name.ToString());
                break;
            }

            parent = parent.Parent;
        }

        return sb.ToString();
    }

    public static string GetFullMetadataName(this ISymbol? s)
    {
        if (s == null || IsRootNamespace(s)) return string.Empty;

        var sb = new StringBuilder(s.MetadataName);
        var last = s;

        s = s.ContainingSymbol;

        while (!IsRootNamespace(s))
        {
            if (s is ITypeSymbol && last is ITypeSymbol)
                sb.Insert(0, '+');
            else
                sb.Insert(0, '.');

            sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            s = s.ContainingSymbol;
        }

        return sb.ToString();

        static bool IsRootNamespace(ISymbol symbol)
        {
            return symbol is INamespaceSymbol s && s.IsGlobalNamespace;
        }
    }

    public static string? GetSyntaxName(this TypeSyntax typeSyntax)
    {
        return typeSyntax switch
               {
                   SimpleNameSyntax sns     => sns.Identifier.Text,
                   QualifiedNameSyntax qns  => qns.Right.Identifier.Text,
                   NullableTypeSyntax nts   => nts.ElementType.GetSyntaxName() + "?",
                   PredefinedTypeSyntax pts => pts.Keyword.Text,
                   ArrayTypeSyntax ats      => ats.ElementType.GetSyntaxName() + "[]",
                   TupleTypeSyntax tts      => "(" + tts.Elements.Select(z => $"{z.Type.GetSyntaxName()}{z.Identifier.Text}") + ")",
                   _                        => null, // there might be more but for now... throw new NotSupportedException(typeSyntax.GetType().FullName)
               };
    }

    public static bool ContainsAttribute(this TypeDeclarationSyntax syntax, string attributePrefixes) // string is comma separated
    {
        return syntax.AttributeLists.ContainsAttribute(attributePrefixes);
    }

    public static bool ContainsAttribute(this AttributeListSyntax list, string attributePrefixes) // string is comma separated
    {
        if (list is { Attributes: { Count: 0, }, })
            return false;
        var names = GetNames(attributePrefixes);

        foreach (var item in list.Attributes)
        {
            if (item.Name.GetSyntaxName() is { } n && names.Contains(n))
                return true;
        }

        return false;
    }

    public static bool ContainsAttribute(this in SyntaxList<AttributeListSyntax> list, string attributePrefixes) // string is comma separated
    {
        if (list is { Count: 0, })
            return false;
        var names = GetNames(attributePrefixes);

        foreach (var item in list)
        {
            foreach (var attribute in item.Attributes)
            {
                if (attribute.Name.GetSyntaxName() is { } n && names.Contains(n))
                    return true;
            }
        }

        return false;
    }

    public static AttributeSyntax? GetAttribute(this TypeDeclarationSyntax syntax, string attributePrefixes) // string is comma separated
    {
        return syntax.AttributeLists.GetAttribute(attributePrefixes);
    }

    public static AttributeSyntax? GetAttribute(this AttributeListSyntax list, string attributePrefixes) // string is comma separated
    {
        if (list is { Attributes: { Count: 0, }, })
            return null;
        var names = GetNames(attributePrefixes);

        foreach (var item in list.Attributes)
        {
            if (item.Name.GetSyntaxName() is { } n && names.Contains(n))
                return item;
        }

        return null;
    }

    public static AttributeSyntax? GetAttribute(this in SyntaxList<AttributeListSyntax> list, string attributePrefixes) // string is comma separated
    {
        if (list is { Count: 0, })
            return null;
        var names = GetNames(attributePrefixes);

        foreach (var item in list)
        {
            foreach (var attribute in item.Attributes)
            {
                if (attribute.Name.GetSyntaxName() is { } n && names.Contains(n))
                    return attribute;
            }
        }

        return null;
    }

    public static bool IsAttribute(this AttributeSyntax attributeSyntax, string attributePrefixes) // string is comma separated
    {
        var names = GetNames(attributePrefixes);
        return attributeSyntax.Name.GetSyntaxName() is { } n && names.Contains(n);
    }

    private static readonly string[] _disabledWarnings =
    {
        "CA1002",
        "CA1034",
        "CA1822",
        "CS0105",
        "CS1573",
        "CS8602",
        "CS8603",
        "CS8618",
        "CS8669",
    };

    private static readonly Lazy<ImmutableArray<ExpressionSyntax>> DisabledWarnings = new(
        () => _disabledWarnings
             .Select(z => (ExpressionSyntax)IdentifierName(z))
             .ToImmutableArray()
    );

    private static readonly ConcurrentDictionary<string, HashSet<string>> AttributeNames = new();

    private static HashSet<string> GetNames(string attributePrefixes)
    {
        if (!AttributeNames.TryGetValue(attributePrefixes, out var names))
        {
            names = new(attributePrefixes.Split(',').SelectMany(z => new[] { z, z + "Attribute", }));
            AttributeNames.TryAdd(attributePrefixes, names);
        }

        return names;
    }
}