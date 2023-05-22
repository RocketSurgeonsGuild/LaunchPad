using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Analyzers;

internal static class SymbolExtensions
{
    public static AttributeData? GetAttribute(this ISymbol symbol, string attributeClassName)
    {
        return symbol.GetAttributes().FirstOrDefault(z => z.AttributeClass?.Name == attributeClassName);
    }
}
