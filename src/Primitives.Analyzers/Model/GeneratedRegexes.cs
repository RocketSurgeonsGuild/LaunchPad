using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

/// <summary>
///     The regular expressions the regular expression generator implements: the patterns of the primitive kinds, in Primitives, and the
///     <c>[GeneratedRegex]</c> members a domain primitive declares, each a pattern its value must match.
/// </summary>
internal static class GeneratedRegexes
{
    /// <summary>The class whose <c>[GeneratedRegex]</c> properties are the patterns of the primitive kinds.</summary>
    public const string PrimitivePatterns = "global::Rocket.Surgery.LaunchPad.Primitives.Advanced.PrimitivePatterns";

    /// <summary>
    ///     The <c>[GeneratedRegex]</c> members of <paramref name="type" />, in the order they are declared: a static property or a static method
    ///     without parameters that returns a <c>Regex</c>. Each with the C# expression of its <c>Regex</c> (<c>Name</c> or <c>Name()</c>) and its
    ///     <c>[GeneratedRegex]</c>.
    /// </summary>
    public static IEnumerable<(string Expression, AttributeData Attribute)> Members(INamedTypeSymbol type)
    {
        foreach (var member in type.GetMembers())
        {
            switch (member)
            {
                case IPropertySymbol { IsStatic: true, IsIndexer: false } property when IsRegex(property.Type) && Attribute(property) is { } attribute:
                    yield return (property.Name, attribute);
                    break;
                case IMethodSymbol { IsStatic: true, MethodKind: MethodKind.Ordinary, Parameters.IsEmpty: true, TypeParameters.IsEmpty: true } method
                    when IsRegex(method.ReturnType) && Attribute(method) is { } attribute:
                    yield return (method.Name + "()", attribute);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>Whether <paramref name="type" /> declares a <c>[GeneratedRegex]</c> member that is a rule.</summary>
    public static bool Any(INamedTypeSymbol type) => Members(type).Any();

    /// <summary>
    ///     The pattern of a <c>[GeneratedRegex]</c> as it describes the values it accepts: its options that change what matches (IgnoreCase,
    ///     Multiline, Singleline, ExplicitCapture, IgnorePatternWhitespace) written in front of it inline, <c>(?i)</c> and so on, since
    ///     <c>Regex.ToString()</c> and a schema's pattern carry no options.
    /// </summary>
    public static string? Describe(AttributeData attribute)
    {
        if (attribute.ConstructorArguments is not [{ Value: string pattern }, ..]) return null;
        var options = attribute.ConstructorArguments is [_, { Value: int value }, ..] ? value : 0;
        var inline = string.Concat(InlineOptions.Where(z => ( options & z.Option ) != 0).Select(z => z.Letter));
        return inline.Length == 0 ? pattern : $"(?{inline}){pattern}";
    }

    // RegexOptions.IgnoreCase = 1, Multiline = 2, ExplicitCapture = 4, Singleline = 16, IgnorePatternWhitespace = 32.
    private static readonly (int Option, char Letter)[] InlineOptions = [(1, 'i'), (2, 'm'), (16, 's'), (4, 'n'), (32, 'x')];

    private static AttributeData? Attribute(ISymbol member) =>
        member.GetAttributes().FirstOrDefault(z => IsRegularExpressions(z.AttributeClass, "GeneratedRegexAttribute"));

    private static bool IsRegex(ITypeSymbol type) => IsRegularExpressions(type, "Regex");

    // By name, not GetTypeByMetadataName, like ValueTypeInfo.Implements.
    private static bool IsRegularExpressions(ITypeSymbol? type, string name) =>
        type is { Name: var n, ContainingNamespace: { Name: "RegularExpressions", ContainingNamespace: { Name: "Text", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } } } }
     && n == name;
}
