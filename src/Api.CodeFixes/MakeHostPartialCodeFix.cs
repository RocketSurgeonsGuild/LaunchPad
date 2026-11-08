using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>Adds the <c>partial</c> (and, for Minimal API hosts, <c>static</c>) modifiers that LPAD1010 asks for.</summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MakeHostPartialCodeFix))]
[Shared]
public sealed class MakeHostPartialCodeFix : CodeFixProvider
{
    /// <inheritdoc />
    // LPAD1010, ApiDiagnostics.HostMustBePartial: the code fix assembly doesn't compile the generators' core.
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ["LPAD1010"];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        foreach (var diagnostic in context.Diagnostics)
        {
            // Only a diagnostic in this document: its span means nothing in another one.
            if (diagnostic.Location.SourceTree?.FilePath is { } path && root?.SyntaxTree.FilePath is { } here && path != here) continue;
            if (root?.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ClassDeclarationSyntax>() is not { Parent: not ClassDeclarationSyntax } declaration) continue;
            var makeStatic = diagnostic.Properties.TryGetValue("static", out var value) && value == "true";
            var title = makeStatic ? "Make the operation host static partial" : "Make the operation host partial";
            context.RegisterCodeFix(
                CodeAction.Create(title, ct => Fix(context.Document, declaration, makeStatic, ct), title),
                diagnostic
            );
        }
    }

    private static async Task<Document> Fix(Document document, ClassDeclarationSyntax declaration, bool makeStatic, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) return document;

        var modifiers = declaration.Modifiers;
        // Modifier order: accessibility, static, partial.
        if (makeStatic && !modifiers.Any(SyntaxKind.StaticKeyword))
        {
            var index = modifiers.IndexOf(SyntaxKind.PartialKeyword);
            modifiers = modifiers.Insert(index >= 0 ? index : modifiers.Count, SyntaxFactory.Token(SyntaxKind.StaticKeyword));
        }

        if (!modifiers.Any(SyntaxKind.PartialKeyword)) modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

        var updated = declaration.Modifiers.Count == 0
            ? declaration.WithModifiers(modifiers.Replace(modifiers[0], modifiers[0].WithLeadingTrivia(declaration.Keyword.LeadingTrivia))).WithKeyword(declaration.Keyword.WithLeadingTrivia(SyntaxFactory.Space))
            : declaration.WithModifiers(SyntaxFactory.TokenList(modifiers.Select((m, i) => i == 0 ? m.WithLeadingTrivia(declaration.Modifiers[0].LeadingTrivia) : m.WithLeadingTrivia().WithTrailingTrivia(SyntaxFactory.Space)).Select((m, i) => i == 0 ? m.WithTrailingTrivia(SyntaxFactory.Space) : m)));
        return document.WithSyntaxRoot(root.ReplaceNode(declaration, updated));
    }
}
