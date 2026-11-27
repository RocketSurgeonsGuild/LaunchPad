using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.LaunchPad.Analyzers;

namespace Analyzers.Tests;

/// <summary>
///     The generators' <c>SyntaxLayout</c> must write exactly what <c>NormalizeWhitespace</c> writes. The generators' snapshots cover the trees they
///     build; these cover everything else C# can hold (comments, directives, documentation, patterns, queries), using the repository's own
///     sources as the corpus.
/// </summary>
public class SyntaxLayoutTests
{
    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Preview, DocumentationMode.Diagnose);

    // SyntaxLayout is internal to the analyzer assembly, which gives the test project no access: its ToText(SyntaxNode, string) is bound once.
    private static readonly Func<SyntaxNode, string, string> ToText = typeof(PropertyTrackingGenerator)
                                                                      .Assembly.GetType("Rocket.Surgery.LaunchPad.Analyzers.SyntaxLayout", throwOnError: true)!
                                                                      .GetMethod("ToText", [typeof(SyntaxNode), typeof(string)])!
                                                                      .CreateDelegate<Func<SyntaxNode, string, string>>();

    private static readonly string[] SourceFolders = ["src", "sample", "test"];

    public static IEnumerable<Func<string>> Sources()
    {
        var root = RepositoryRoot();
        return SourceFolders
              .SelectMany(folder => Directory.EnumerateFiles(Path.Combine(root, folder), "*.cs", SearchOption.AllDirectories))
              .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                          && !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
              .Order(StringComparer.Ordinal)
              .Select(path => (Func<string>)( () => Path.GetRelativePath(root, path) ));
    }

    [Test]
    [MethodDataSource(nameof(Sources))]
    public async Task Should_Lay_Out_A_Tree_As_NormalizeWhitespace_Does(string path)
    {
        var tree = CSharpSyntaxTree.ParseText(await File.ReadAllTextAsync(Path.Combine(RepositoryRoot(), path)), ParseOptions);
        var root = await tree.GetRootAsync();

        await ToText(root, "\r\n").Should().BeEqualTo(root.NormalizeWhitespace().ToFullString());
        await ToText(root, "\n").Should().BeEqualTo(root.NormalizeWhitespace(eol: "\n").ToFullString());
    }

    [Test]
    public async Task Should_Lay_Out_A_Tree_Built_With_SyntaxFactory_As_NormalizeWhitespace_Does()
    {
        // Built trees carry no whitespace at all, and elastic markers where the factory adds them.
        var root = SyntaxFactory.ParseCompilationUnit(
            """
            namespace A { /// <summary>Doc <see cref="B"/> text</summary>
            public sealed record B(int X) { public int Y { get; init; } = X switch { 1 => 2, _ => 3 }; #region r
            public static B operator +(B a, B b) => new(a.X + b.X) { Y = a.Y is > 1 and < 5 ? 1 : 2 }; #endregion
            } }
            """
        );
        var stripped = (CompilationUnitSyntax)new WhitespaceStripper().Visit(root);

        await ToText(stripped, "\r\n").Should().BeEqualTo(stripped.NormalizeWhitespace().ToFullString());
    }

    private static string RepositoryRoot([CallerFilePath] string file = "") => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, "..", ".."));

    private sealed class WhitespaceStripper() : CSharpSyntaxRewriter(visitIntoStructuredTrivia: false)
    {
        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia) =>
            trivia.IsKind(SyntaxKind.WhitespaceTrivia) || trivia.IsKind(SyntaxKind.EndOfLineTrivia) ? SyntaxFactory.ElasticMarker : trivia;
    }
}
