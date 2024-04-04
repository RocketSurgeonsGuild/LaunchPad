using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Rocket.Surgery.LaunchPad.Analyzers;

internal static class ContextExtensions
{
    public static void AddSourceRelativeTo(this SourceProductionContext context, TypeDeclarationSyntax declaration, string suffix, SourceText sourceText)
    {
        context.AddSource(
            $"{Path.GetFileNameWithoutExtension(declaration.SyntaxTree.FilePath)}_{string.Join("_", declaration.GetParentDeclarationsWithSelf().Reverse().Select(z => z.Identifier.Text))}_{suffix}",
            sourceText
        );
    }
    /// <summary>
    ///     Same as Pascalize except that the first character is lower case
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Camelize(string input)
    {
        var word = Pascalize(input);
        #pragma warning disable CA1308
        return word.Length > 0 ? string.Concat(word.Substring(0, 1).ToLower(CultureInfo.InvariantCulture), word.Substring(1)) : word;
        #pragma warning restore CA1308
    }

    /// <summary>
    ///     By default, pascalize converts strings to UpperCamelCase also removing underscores
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Pascalize(string input)
    {
        return Regex.Replace(input, "(?:^|_| +)(.)", match => match.Groups[1].Value.ToUpper(CultureInfo.InvariantCulture));
    }
}
