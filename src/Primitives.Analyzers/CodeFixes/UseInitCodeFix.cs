using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

/// <summary>
///     Fixes LPAD2008 by turning the <c>set</c> accessor of the value object's property into <c>init</c>, keeping its accessibility and body. A
///     property declared by a record's parameter has no accessor to change, so it is not fixed.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseInitCodeFix))]
[Shared]
public sealed class UseInitCodeFix : CodeFixProvider
{
    private const string Title = "Use 'init' instead of 'set'";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [AnalyzerDiagnostics.ValueObjectPropertyMustBeInit.Id];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;
        foreach (var diagnostic in context.Diagnostics)
        {
            if (diagnostic.Location.SourceTree != root.SyntaxTree) continue;
            if (root.FindToken(diagnostic.Location.SourceSpan.Start).Parent?.FirstAncestorOrSelf<PropertyDeclarationSyntax>() is not { AccessorList: { } accessors }) continue;
            if (Setter(accessors) is not { } setter) continue;
            context.RegisterCodeFix(
                CodeAction.Create(Title, _ => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(setter, ToInit(setter)))), Title),
                diagnostic
            );
        }
    }

    private static AccessorDeclarationSyntax? Setter(AccessorListSyntax accessors)
    {
        foreach (var accessor in accessors.Accessors)
        {
            if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration)) return accessor;
        }

        return null;
    }

    /// <summary>The accessor as <c>init</c>, with the trivia of its <c>set</c> keyword.</summary>
    internal static AccessorDeclarationSyntax ToInit(AccessorDeclarationSyntax setter) =>
        SyntaxFactory.AccessorDeclaration(
            SyntaxKind.InitAccessorDeclaration,
            setter.AttributeLists,
            setter.Modifiers,
            SyntaxFactory.Token(setter.Keyword.LeadingTrivia, SyntaxKind.InitKeyword, setter.Keyword.TrailingTrivia),
            setter.Body,
            setter.ExpressionBody,
            setter.SemicolonToken
        );
}
