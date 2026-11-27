using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

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
    ///     The start of a file for a partial part of a declaration: <c>#nullable enable</c>, the disabled warnings, the usings, the namespace
    ///     and the types the declaration is nested in, each opened, all from <paramref name="frame" />. <see cref="CloseFile" /> closes them.
    /// </summary>
    /// <remarks>
    ///     The declaring file's usings are copied as the user wrote them, so they are laid out from their syntax, read again from the text the
    ///     frame keeps; this runs only when the frame or what follows it changed.
    /// </remarks>
    public int OpenFile(GeneratedFileFrame frame)
    {
        Line("#nullable enable");
        Line("#pragma warning disable " + SyntaxExtensions.DisabledWarningsText);
        var usings = frame.Usings is { } written ? SyntaxFactory.ParseCompilationUnit(written).Usings : default;
        foreach (var added in frame.AddedUsings) usings = usings.Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(added)));
        if (usings.Count > 0)
        {
            // The shared header replaces what led the first using, as it did when it was the file's leading trivia.
            var first = usings[0].WithLeadingTrivia();
            _text.Append(SyntaxLayout.ToText(SyntaxFactory.CompilationUnit().WithUsings(usings.Replace(usings[0], first)), endOfLine)).Append(endOfLine).Append(endOfLine);
        }

        var opened = 0;
        if (frame.Namespace is { } @namespace)
        {
            Line("namespace " + @namespace);
            OpenBlock();
            opened++;
        }

        foreach (var containingType in frame.ContainingTypes)
        {
            Line(containingType);
            OpenBlock();
            opened++;
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

/// <summary>
///     What the start of a generated file takes from the declaration it completes, as text, so a generator can read it in its transform and
///     compare it by value.
/// </summary>
/// <param name="Usings">The declaring file's using directives as written, or null for none.</param>
/// <param name="AddedUsings">The namespaces to import that the declaring file doesn't.</param>
/// <param name="Namespace">The declaration's namespace, or null for the global one.</param>
/// <param name="ContainingTypes">The types the declaration is nested in, outermost first, as their declaration lines.</param>
internal sealed record GeneratedFileFrame(string? Usings, EquatableArray<string> AddedUsings, string? Namespace, EquatableArray<string> ContainingTypes)
{
    /// <summary>
    ///     Reads the frame of <paramref name="declaration" />, importing <paramref name="namespaces" /> the file doesn't, and reports each
    ///     containing type that isn't partial (LPAD0001), innermost first.
    /// </summary>
    public static GeneratedFileFrame Read(
        TypeDeclarationSyntax declaration,
        INamespaceSymbol containingNamespace,
        IEnumerable<string> namespaces,
        ICollection<Diagnostic> diagnostics
    )
    {
        var usings = declaration.SyntaxTree.GetCompilationUnitRoot().Usings;
        var written = usings.Count == 0 ? null : declaration.SyntaxTree.GetText().ToString(TextSpan.FromBounds(usings[0].FullSpan.Start, usings[^1].FullSpan.End));

        // A namespace the file already imports, under any name, isn't imported again.
        var imported = new HashSet<string?>(StringComparer.Ordinal);
        foreach (var directive in usings) imported.Add(directive.Name?.ToString());
        var added = new List<string>();
        foreach (var name in namespaces)
        {
            if (!string.IsNullOrWhiteSpace(name) && imported.Add(name)) added.Add(name);
        }

        var containingTypes = new List<string>();
        for (var parent = declaration.Parent; parent is TypeDeclarationSyntax type; parent = type.Parent)
        {
            containingTypes.Insert(0, SourceWriter.Modifiers(type.Modifiers) + ( type is RecordDeclarationSyntax ? "record " : "class " ) + type.Identifier.Text);
            if (!type.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                diagnostics.Add(Diagnostic.Create(GeneratorDiagnostics.MustBePartial, type.Identifier.GetLocation(), type.GetFullMetadataName()));
            }
        }

        return new(
            written,
            added.ToEquatableArray(),
            containingNamespace.IsGlobalNamespace ? null : containingNamespace.ToDisplayString(),
            containingTypes.ToEquatableArray()
        );
    }
}
