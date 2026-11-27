using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Analyzers;

internal static class Helpers
{
    /// <summary>
    ///     Same as Pascalize except that the first character is lower case
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Camelize(string input)
    {
        var word = Pascalize(input);
#pragma warning disable CA1308
        return word.Length > 0 ? string.Concat(word[..1].ToLower(CultureInfo.InvariantCulture), word[1..]) : word;
#pragma warning restore CA1308
    }

    /// <summary>
    ///     By default, pascalize converts strings to UpperCamelCase also removing underscores
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Behaves exactly like <c>Regex.Replace(input, "(?:^|_| +)(.)", m => m.Groups[1].Value.ToUpperInvariant())</c>, which this replaced:
    ///     the first character, and the character after an underscore or a run of spaces, is upper-cased and the separator dropped; a
    ///     separator with nothing (or a line break) after it stays, a run of spaces as one.
    /// </remarks>
    public static string Pascalize(string input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        var builder = new StringBuilder();
        var atStart = true;
        var pendingUnderscore = false;
        var pendingSpace = false;
        foreach (var current in input)
        {
            if (atStart)
            {
                atStart = false;
                builder.Append(current == '\n' ? current : char.ToUpper(current, CultureInfo.InvariantCulture));
                continue;
            }

            if (pendingUnderscore || pendingSpace)
            {
                if (pendingSpace && current == ' ')
                {
                    continue;
                }

                var separator = pendingUnderscore ? '_' : ' ';
                pendingUnderscore = false;
                pendingSpace = false;
                if (current != '\n')
                {
                    builder.Append(char.ToUpper(current, CultureInfo.InvariantCulture));
                    continue;
                }

                builder.Append(separator);
            }

            if (current == '_')
            {
                pendingUnderscore = true;
            }
            else if (current == ' ')
            {
                pendingSpace = true;
            }
            else
            {
                builder.Append(current);
            }
        }

        if (pendingUnderscore)
        {
            builder.Append('_');
        }
        else if (pendingSpace)
        {
            builder.Append(' ');
        }

        return builder.ToString();
    }

    /// <summary>The <see cref="CompilerGenerated" /> attribute list as text.</summary>
    internal static readonly string CompilerGeneratedText =
        $"[System.CodeDom.Compiler.GeneratedCode(\"{AssemblyInfo.Project.AssemblyName}\", \"{AssemblyInfo.Attributes.FileVersion}\"), System.Runtime.CompilerServices.CompilerGenerated]";

    /// <summary>The <see cref="CompilerAttributes" /> attribute list as text.</summary>
    internal static readonly string CompilerAttributesText =
        $"[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage, System.CodeDom.Compiler.GeneratedCode(\"{AssemblyInfo.Project.AssemblyName}\", \"{AssemblyInfo.Attributes.FileVersion}\"), System.Runtime.CompilerServices.CompilerGenerated]";

    internal static AttributeListSyntax ExcludeFromCodeCoverage =
        AttributeList(SingletonSeparatedList(Attribute(ParseName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage"))));

    internal static AttributeListSyntax CompilerGenerated =
        AttributeList(
            SeparatedList(
                [
                    Attribute(ParseName("System.CodeDom.Compiler.GeneratedCode"))
                       .WithArgumentList(
                            AttributeArgumentList(
                                SeparatedList(
                                    [
                                        AttributeArgument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal(AssemblyInfo.Project.AssemblyName)
                                            )
                                        ),
                                        AttributeArgument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal(AssemblyInfo.Attributes.FileVersion)
                                            )
                                        ),
                                    ]
                                )
                            )
                        ),
                    Attribute(ParseName("System.Runtime.CompilerServices.CompilerGenerated")),
                ]
            )
        );

    internal static AttributeListSyntax CompilerAttributes =
        AttributeList(SeparatedList([.. ExcludeFromCodeCoverage.Attributes, .. CompilerGenerated.Attributes,]));
}
