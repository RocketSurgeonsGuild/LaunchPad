using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Analyzers;

internal static class Helpers
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
