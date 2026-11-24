using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

/// <summary>
///     Fixes LPAD2005 by removing <c>[LowerCase]</c> or <c>[UpperCase]</c>, one action each: the diagnostic carries where the two are written, in
///     whichever declaration of the type that is.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveNormalizationCodeFix))]
[Shared]
public sealed class RemoveNormalizationCodeFix : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [AnalyzerDiagnostics.ConflictingNormalization.Id];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        foreach (var diagnostic in context.Diagnostics)
        {
            // [LowerCase], then [UpperCase].
            if (diagnostic.AdditionalLocations is not [{ SourceTree: not null } lowerCase, { SourceTree: not null } upperCase]) continue;
            Register(context, diagnostic, "Remove [LowerCase]", lowerCase);
            Register(context, diagnostic, "Remove [UpperCase]", upperCase);
        }

        return Task.CompletedTask;
    }

    private static void Register(CodeFixContext context, Diagnostic diagnostic, string title, Location attribute) =>
        context.RegisterCodeFix(CodeAction.Create(title, ct => RemoveAsync(context.Document.Project.Solution, attribute, ct), title), diagnostic);

    private static async Task<Solution> RemoveAsync(Solution solution, Location location, CancellationToken cancellationToken)
    {
        if (solution.GetDocument(location.SourceTree) is not { } document) return solution;
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        return root?.FindNode(location.SourceSpan).FirstAncestorOrSelf<AttributeSyntax>() is not { Parent: AttributeListSyntax list } attribute
            ? solution
            : solution.WithDocumentSyntaxRoot(document.Id, RemoveAttribute(root, list, attribute));
    }

    /// <summary>
    ///     Removes the attribute, and its list when it is the only one in it; a removed first list leaves its leading trivia (comments, the
    ///     documentation) to what follows it.
    /// </summary>
    internal static SyntaxNode RemoveAttribute(SyntaxNode root, AttributeListSyntax list, AttributeSyntax attribute)
    {
        if (list.Attributes.Count > 1) return root.ReplaceNode(list, list.WithAttributes(list.Attributes.Remove(attribute)));
        if (list.Parent is not MemberDeclarationSyntax member) return root;
        var index = member.AttributeLists.IndexOf(list);
        var updated = member.WithAttributeLists(member.AttributeLists.RemoveAt(index));
        if (index == 0) updated = updated.WithLeadingTrivia(member.GetLeadingTrivia());
        return root.ReplaceNode(member, updated);
    }
}
