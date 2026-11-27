using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     Writes generated C# as text, a line at a time, at the current indentation. The generators that emit strings lay out their code with it:
///     four spaces an indent, Allman braces, a blank line between members, CRLF line endings (what <c>NormalizeWhitespace</c> wrote before).
/// </summary>
internal sealed class SourceWriter(string endOfLine = SourceWriter.DefaultEndOfLine)
{
    public const string DefaultEndOfLine = "\r\n";

    private const string IndentUnit = "    ";
    private static readonly string[] Indents = [.. Enumerable.Range(0, 16).Select(depth => string.Concat(Enumerable.Repeat(IndentUnit, depth)))];

    private readonly StringBuilder _text = new(4096);
    private int _depth;

    /// <summary>A line of code at the current indentation.</summary>
    public SourceWriter Line(string text)
    {
        _text.Append(_depth < Indents.Length ? Indents[_depth] : string.Concat(Enumerable.Repeat(IndentUnit, _depth))).Append(text).Append(endOfLine);
        return this;
    }

    /// <summary>An empty line.</summary>
    public SourceWriter Line()
    {
        _text.Append(endOfLine);
        return this;
    }

    /// <summary><c>{</c> on its own line; what follows is indented one more.</summary>
    public SourceWriter OpenBlock()
    {
        Line("{");
        _depth++;
        return this;
    }

    /// <summary><c>}</c> (followed by <paramref name="suffix" />, as <c>;</c>) on its own line, one indent less.</summary>
    public SourceWriter CloseBlock(string suffix = "")
    {
        _depth--;
        return Line(suffix.Length == 0 ? "}" : "}" + suffix);
    }

    /// <summary>An object or <c>with</c> initializer: braces on their own lines, one element a line, separated by commas.</summary>
    public SourceWriter Initializer(IReadOnlyList<string> elements, string suffix)
    {
        OpenBlock();
        for (var i = 0; i < elements.Count; i++) Line(i < elements.Count - 1 ? elements[i] + "," : elements[i]);
        return CloseBlock(suffix);
    }

    public override string ToString() => _text.ToString();

    /// <summary>
    ///     The start of a file for a partial part of <paramref name="declaration" />: <c>#nullable enable</c>, the disabled warnings, the
    ///     <paramref name="usings" />, the namespace and the types it is nested in, each opened. <see cref="CloseFile" /> closes them.
    /// </summary>
    /// <remarks>
    ///     The usings are the declaring file's, copied as the user wrote them, so they are laid out from their syntax.
    /// </remarks>
    public int OpenFile(SyntaxList<UsingDirectiveSyntax> usings, INamespaceSymbol containingNamespace, TypeDeclarationSyntax declaration, GeneratedSources output)
    {
        Line("#nullable enable");
        Line("#pragma warning disable " + SyntaxExtensions.DisabledWarningsText);
        if (usings.Count > 0)
        {
            // The shared header replaces what led the first using, as it did when it was the file's leading trivia.
            var first = usings[0].WithLeadingTrivia();
            _text.Append(SyntaxLayout.ToText(SyntaxFactory.CompilationUnit().WithUsings(usings.Replace(usings[0], first)), endOfLine)).Append(endOfLine).Append(endOfLine);
        }

        var opened = 0;
        if (!containingNamespace.IsGlobalNamespace)
        {
            Line("namespace " + containingNamespace.ToDisplayString());
            OpenBlock();
            opened++;
        }

        // The types the declaration is nested in, outermost first: each must be partial too.
        var parents = new List<TypeDeclarationSyntax>();
        for (var parent = declaration.Parent; parent is TypeDeclarationSyntax type; parent = type.Parent) parents.Add(type);
        for (var i = parents.Count - 1; i >= 0; i--)
        {
            var parent = parents[i];
            Line(Modifiers(parent.Modifiers) + ( parent is RecordDeclarationSyntax ? "record " : "class " ) + parent.Identifier.Text);
            OpenBlock();
            opened++;
        }

        // Reported innermost first, as the syntax version reparented them.
        foreach (var parent in parents)
        {
            if (!parent.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                output.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MustBePartial, parent.Identifier.GetLocation(), parent.GetFullMetadataName()));
            }
        }

        return opened;
    }

    /// <summary>Closes what <see cref="OpenFile" /> opened and restores the warnings and nullable context.</summary>
    public void CloseFile(int opened)
    {
        for (var i = 0; i < opened; i++) CloseBlock();
        Line("#pragma warning restore " + SyntaxExtensions.DisabledWarningsText);
        Line("#nullable restore");
    }

    /// <summary>The modifiers of a declaration as written, each followed by a space.</summary>
    public static string Modifiers(SyntaxTokenList modifiers)
    {
        var text = new StringBuilder();
        foreach (var modifier in modifiers) text.Append(modifier.Text).Append(' ');
        return text.ToString();
    }
}
