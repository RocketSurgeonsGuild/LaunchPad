using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

/// <summary>
///     Fixes LPAD2009 on a string primitive by adding <c>[NotEmpty]</c> after its other attributes. A number or a date has no rule that fits every
///     one, so only a string is fixed.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddNotEmptyCodeFix))]
[Shared]
public sealed class AddNotEmptyCodeFix : CodeFixProvider
{
    private const string Title = "Add [NotEmpty]";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [AnalyzerDiagnostics.MissingValidation.Id];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;
        foreach (var diagnostic in context.Diagnostics)
        {
            if (!diagnostic.Properties.ContainsKey(RulesAnalyzer.AddNotEmptyProperty) || diagnostic.Location.SourceTree != root.SyntaxTree) continue;
            if (root.FindToken(diagnostic.Location.SourceSpan.Start).Parent?.FirstAncestorOrSelf<TypeDeclarationSyntax>() is not { } declaration) continue;
            context.RegisterCodeFix(CodeAction.Create(Title, ct => AddAsync(context.Document, declaration, ct), Title), diagnostic);
        }
    }

    private static async Task<Document> AddAsync(Document document, TypeDeclarationSyntax declaration, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || model is null) return document;

        // NotEmpty where the primitive's attribute is in scope; its full name where it isn't.
        var inScope = model.LookupNamespacesAndTypes(declaration.SpanStart, name: "NotEmptyAttribute")
                           .Any(s => s.ContainingNamespace?.ToDisplayString() == "Rocket.Surgery.LaunchPad.Primitives");
        var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(inScope ? "NotEmpty" : "global::Rocket.Surgery.LaunchPad.Primitives.NotEmpty"));
        var list = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute));

        TypeDeclarationSyntax updated;
        if (declaration.AttributeLists.Count > 0)
        {
            var last = declaration.AttributeLists[^1];
            list = list.WithLeadingTrivia(Indentation(last.GetLeadingTrivia())).WithTrailingTrivia(last.GetTrailingTrivia());
            updated = declaration.WithAttributeLists(declaration.AttributeLists.Add(list));
        }
        else
        {
            // The attribute takes the declaration's leading trivia (its comments), and the declaration keeps its indentation.
            var leading = declaration.GetLeadingTrivia();
            list = list.WithLeadingTrivia(leading).WithTrailingTrivia(SyntaxFactory.EndOfLine(EndOfLine(root)));
            updated = declaration.WithLeadingTrivia(Indentation(leading)).WithAttributeLists(SyntaxFactory.SingletonList(list));
        }

        return document.WithSyntaxRoot(root.ReplaceNode(declaration, updated));
    }

    /// <summary>The white space that indents the line the trivia ends on.</summary>
    internal static SyntaxTriviaList Indentation(SyntaxTriviaList trivia) =>
        trivia.Count > 0 && trivia[^1].IsKind(SyntaxKind.WhitespaceTrivia) ? SyntaxFactory.TriviaList(trivia[^1]) : SyntaxFactory.TriviaList();

    /// <summary>The line ending the document uses.</summary>
    internal static string EndOfLine(SyntaxNode root) =>
        root.DescendantTrivia().FirstOrDefault(t => t.IsKind(SyntaxKind.EndOfLineTrivia)) is { RawKind: not 0 } endOfLine ? endOfLine.ToString() : "\n";
}
