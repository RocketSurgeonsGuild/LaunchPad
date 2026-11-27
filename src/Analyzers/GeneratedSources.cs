using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     What a generator writes for one declaration: its sources, as text, and its diagnostics. The generators that read symbols build it in
///     their transform and only add it in their output step, so the pipeline holds no symbol, semantic model or compilation and compares by
///     value: when an edit leaves a declaration's output as it was, the output step is skipped.
/// </summary>
/// <remarks>
///     A diagnostic compares by its descriptor, arguments and location; a location in a syntax tree an edit did not touch stays equal.
/// </remarks>
internal sealed class GeneratedSources : IEquatable<GeneratedSources>
{
    private readonly List<(string HintName, string Text)> _sources = [];
    private readonly List<Diagnostic> _diagnostics = [];

    /// <summary>
    ///     The hint name of a declaration's source: its file name, the names of the types it is nested in and its own, then
    ///     <paramref name="suffix" />.
    /// </summary>
    public static string HintNameRelativeTo(TypeDeclarationSyntax declaration, string suffix) =>
        $"{Path.GetFileNameWithoutExtension(declaration.SyntaxTree.FilePath)}_{string.Join("_", declaration.GetParentDeclarationsWithSelf().Reverse().Select(z => z.Identifier.Text))}_{suffix}";

    public void AddSource(string hintName, string text) => _sources.Add((hintName, text));

    public void ReportDiagnostic(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);

    public void AddTo(SourceProductionContext context)
    {
        foreach (var diagnostic in _diagnostics) context.ReportDiagnostic(diagnostic);
        foreach (var (hintName, text) in _sources) context.AddSource(hintName, SourceText.From(text, Encoding.UTF8));
    }

    public bool Equals(GeneratedSources? other) =>
        other is { }
     && ( ReferenceEquals(this, other) || ( _sources.SequenceEqual(other._sources) && _diagnostics.SequenceEqual(other._diagnostics) ) );

    public override bool Equals(object? obj) => Equals(obj as GeneratedSources);

    public override int GetHashCode()
    {
        var hash = 17;
        foreach (var (hintName, text) in _sources) hash = unchecked(( hash * 31 ) + StringComparer.Ordinal.GetHashCode(hintName) + text.Length);
        return unchecked(( hash * 31 ) + _diagnostics.Count);
    }
}
