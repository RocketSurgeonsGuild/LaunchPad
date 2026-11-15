using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Analyzers;

internal static class SymbolExtensions
{
    public static AttributeData? GetAttribute(this ISymbol symbol, string attributeClassName)
    {
        return symbol
              .GetAttributes()
              .FirstOrDefault(z => z.AttributeClass?.Name == attributeClassName || z.AttributeClass.GetFullMetadataName() == attributeClassName);
    }

    public static SyntaxList<UsingDirectiveSyntax> AddDistinctUsingStatements(
        this SyntaxList<UsingDirectiveSyntax> usingDirectiveSyntax,
        IEnumerable<string> namespaces
    )
    {
        foreach (var additionalUsing in namespaces.Where(z => !string.IsNullOrWhiteSpace(z)))
        {
            if (ContainsUsing(usingDirectiveSyntax, additionalUsing)) continue;
            usingDirectiveSyntax = usingDirectiveSyntax.Add(UsingDirective(ParseName(additionalUsing)));
        }

        return usingDirectiveSyntax;
    }

    public static SyntaxList<UsingDirectiveSyntax> AddDistinctUsingStatements(
        this SyntaxList<UsingDirectiveSyntax> usingDirectiveSyntax,
        SyntaxList<UsingDirectiveSyntax> namespaces
    )
    {
        foreach (var additionalUsing in namespaces)
        {
            if (ContainsUsing(usingDirectiveSyntax, additionalUsing.Name?.ToString())) continue;
            usingDirectiveSyntax = usingDirectiveSyntax.Add(additionalUsing);
        }

        return usingDirectiveSyntax;
    }

    private static bool ContainsUsing(SyntaxList<UsingDirectiveSyntax> usingDirectiveSyntax, string? name)
    {
        foreach (var usingDirective in usingDirectiveSyntax)
        {
            if (usingDirective.Name?.ToString() == name) return true;
        }

        return false;
    }

    public static IEnumerable<IPropertySymbol> FilterProperties(this ITypeSymbol typeSymbol)
    {
        var members = typeSymbol
                     .GetMembers()
                     .OfType<IPropertySymbol>()
                     .Where(z => z.GetAttribute("ExcludeFromGenerationAttribute") is null)
                     .Where(z => z.GetAttribute("GenerationIgnoreAttribute") is null)
                     .Where(z => z is { IsStatic: false, IsIndexer: false, IsReadOnly: false, });

        return members;
    }
}
