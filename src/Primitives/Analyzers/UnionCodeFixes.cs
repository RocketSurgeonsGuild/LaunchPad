using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers;

/// <summary>
///     C# 14 can't match a union's cases (CS9327, or CS8652 from a compiler where unions are a preview feature): match its <c>Value</c>
///     instead, with a discard arm, since C# 14 can't tell the match is exhaustive.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnionPatternCodeFix))]
[Shared]
public sealed class UnionPatternCodeFix : CodeFixProvider
{
    private const string Title = "Match the union's Value";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ["CS9327", "CS8652",];

    /// <inheritdoc />
    public override FixAllProvider? GetFixAllProvider() => null;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null || semanticModel is null) return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var match = node.AncestorsAndSelf().FirstOrDefault(ancestor => ancestor is SwitchExpressionSyntax or IsPatternExpressionSyntax);
            var governing = match switch
            {
                SwitchExpressionSyntax switchExpression => switchExpression.GoverningExpression,
                IsPatternExpressionSyntax isPattern => isPattern.Expression,
                _ => null,
            };
            if (governing is null || !UnionNames.IsUnion(semanticModel.GetTypeInfo(governing, context.CancellationToken).Type)) continue;

            context.RegisterCodeFix(
                CodeAction.Create(Title, cancellationToken => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(match!, Rewrite(match!)))), Title),
                diagnostic
            );
            return;
        }
    }

    private static SyntaxNode Rewrite(SyntaxNode match) =>
        match switch
        {
            SwitchExpressionSyntax switchExpression => RewriteSwitch(switchExpression),
            IsPatternExpressionSyntax isPattern => isPattern.WithExpression(ValueOf(isPattern.Expression)),
            _ => match,
        };

    private static SwitchExpressionSyntax RewriteSwitch(SwitchExpressionSyntax switchExpression)
    {
        var rewritten = switchExpression.WithGoverningExpression(ValueOf(switchExpression.GoverningExpression));
        if (rewritten.Arms.Any(arm => arm is { Pattern: DiscardPatternSyntax, WhenClause: null, })) return rewritten;

        // `_ => throw new global::System.Diagnostics.UnreachableException()`, written as text so the arm's layout matches the others.
        var discard = SwitchExpressionArm(
            DiscardPattern(),
            ThrowExpression(ObjectCreationExpression(ParseTypeName("global::System.Diagnostics.UnreachableException")).WithArgumentList(ArgumentList()))
        );
        var arms = rewritten.Arms;
        var separators = arms.GetSeparators().ToList();
        if (separators.Count < arms.Count) separators.Add(Token(SyntaxKind.CommaToken));
        return rewritten.WithArms(SeparatedList(arms.Add(discard.WithLeadingTrivia(arms.Last().GetLeadingTrivia())), separators));
    }

    private static ExpressionSyntax ValueOf(ExpressionSyntax expression)
    {
        var receiver = expression is IdentifierNameSyntax or MemberAccessExpressionSyntax or InvocationExpressionSyntax or ThisExpressionSyntax or ElementAccessExpressionSyntax
            ? expression.WithoutTrivia()
            : ParenthesizedExpression(expression.WithoutTrivia());
        return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, receiver, IdentifierName("Value")).WithTriviaFrom(expression);
    }
}

/// <summary>LPAD0013: match the union itself, and drop the discard arm that only C# 14 needed.</summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RedundantValueSwitchCodeFix))]
[Shared]
public sealed class RedundantValueSwitchCodeFix : CodeFixProvider
{
    private const string Title = "Match the union";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [UnionDiagnostics.RedundantValueSwitch.Id,];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            if (node.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault() is not { Name.Identifier.ValueText: "Value", } access) continue;

            context.RegisterCodeFix(
                CodeAction.Create(Title, _ => Task.FromResult(context.Document.WithSyntaxRoot(Rewrite(root, access))), Title),
                diagnostic
            );
        }
    }

    private static SyntaxNode Rewrite(SyntaxNode root, MemberAccessExpressionSyntax access)
    {
        var receiver = ( access.Expression is ParenthesizedExpressionSyntax parenthesized ? parenthesized.Expression : access.Expression ).WithTriviaFrom(access);
        return access.Parent is SwitchExpressionSyntax switchExpression
            ? root.ReplaceNode(switchExpression, RemoveThrowingDiscard(switchExpression.WithGoverningExpression(receiver)))
            : root.ReplaceNode(access, receiver);
    }

    private static SwitchExpressionSyntax RemoveThrowingDiscard(SwitchExpressionSyntax switchExpression)
    {
        if (switchExpression.Arms is not [.., { Pattern: DiscardPatternSyntax, Expression: ThrowExpressionSyntax, } last,]) return switchExpression;

        var arms = switchExpression.Arms.Remove(last);
        // Keep a trailing comma only if the original had one after the removed arm.
        var separators = switchExpression.Arms.GetSeparators().Take(arms.Count).ToList();
        if (switchExpression.Arms.SeparatorCount < switchExpression.Arms.Count && separators.Count == arms.Count) separators.RemoveAt(separators.Count - 1);
        return switchExpression.WithArms(SeparatedList(arms, separators));
    }
}

/// <summary>LPAD0014: rewrite a portable union as the native declaration it stands for.</summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NativeUnionCodeFix))]
[Shared]
public sealed class NativeUnionCodeFix : CodeFixProvider
{
    private const string Title = "Declare as a C# union";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [UnionDiagnostics.PortableUnionCanBeNative.Id,];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true).AncestorsAndSelf().OfType<StructDeclarationSyntax>().FirstOrDefault() is not { } declaration
             || !UnionAnalyzer.IsNativeCandidate(declaration)
             || CreateNativeDeclaration(declaration) is not { } replacement)
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    Title,
                    _ => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(declaration, replacement))),
                    Title + declaration.Identifier.ValueText
                ),
                diagnostic
            );
        }
    }

    // Parsed with the document's options: only a compiler that knows unions can build the replacement.
    internal static MemberDeclarationSyntax? CreateNativeDeclaration(StructDeclarationSyntax declaration)
    {
        var accessibility = string.Join(
            " ",
            declaration.Modifiers.Where(modifier => SyntaxFacts.IsAccessibilityModifier(modifier.Kind()) || modifier.IsKind(SyntaxKind.NewKeyword)).Select(modifier => modifier.Text)
        );
        var cases = string.Join(", ", UnionAnalyzer.CaseConstructors(declaration).Select(constructor => constructor.ParameterList.Parameters[0].Type!.ToString()));
        var constraints = declaration.ConstraintClauses.Count > 0 ? " " + string.Join(" ", declaration.ConstraintClauses.Select(clause => clause.ToString())) : "";
        var text = $"{accessibility}{( accessibility.Length > 0 ? " " : "" )}union {declaration.Identifier.ValueText}{declaration.TypeParameterList}({cases}){constraints};";
        return ParseMemberDeclaration(text, options: declaration.SyntaxTree.Options) is { ContainsDiagnostics: false, } replacement
            ? replacement.WithLeadingTrivia(declaration.GetLeadingTrivia()).WithTrailingTrivia(declaration.GetTrailingTrivia())
            : null;
    }
}
