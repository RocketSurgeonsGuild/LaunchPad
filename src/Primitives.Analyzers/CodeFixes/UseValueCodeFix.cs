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
///     Fixes LPAD2010 and LPAD2011 by reading the domain primitive's <c>Value</c>: on the primitive side of a comparison with a string, or on a
///     primitive given where a string is expected.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseValueCodeFix))]
[Shared]
public sealed class UseValueCodeFix : CodeFixProvider
{
    private const string CompareTitle = "Compare with the domain primitive's Value";
    private const string UseTitle = "Use the domain primitive's Value";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
    [
        AnalyzerDiagnostics.StringComparedWithPrimitive.Id,
        AnalyzerDiagnostics.StringAssignedFromPrimitive.Id,
    ];

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
            var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: false);
            if (diagnostic.Id == AnalyzerDiagnostics.StringComparedWithPrimitive.Id)
            {
                if (node.FirstAncestorOrSelf<BinaryExpressionSyntax>() is not { } comparison) continue;
                context.RegisterCodeFix(CodeAction.Create(CompareTitle, ct => FixComparisonAsync(context.Document, comparison, ct), CompareTitle), diagnostic);
                continue;
            }

            var value = node switch
            {
                VariableDeclaratorSyntax { Initializer.Value: var initializer } => initializer,
                AssignmentExpressionSyntax assignment => assignment.Right,
                ArgumentSyntax argument => argument.Expression,
                _ => null,
            };
            if (value is null) continue;
            context.RegisterCodeFix(
                CodeAction.Create(UseTitle, _ => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(value, ReadValue(value)))), UseTitle),
                diagnostic
            );
        }
    }

    private static async Task<Document> FixComparisonAsync(Document document, BinaryExpressionSyntax comparison, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || model is null) return document;
        var left = PrimitiveSymbols.IsStringPrimitive(model.GetTypeInfo(comparison.Left, cancellationToken).Type);
        var right = PrimitiveSymbols.IsStringPrimitive(model.GetTypeInfo(comparison.Right, cancellationToken).Type);
        var updated = comparison.WithLeft(left ? ReadValue(comparison.Left) : comparison.Left).WithRight(right ? ReadValue(comparison.Right) : comparison.Right);
        return document.WithSyntaxRoot(root.ReplaceNode(comparison, updated));
    }

    /// <summary><c>expression.Value</c>, with the expression in parentheses unless it is a primary expression.</summary>
    internal static ExpressionSyntax ReadValue(ExpressionSyntax expression)
    {
        var target = expression.WithoutTrivia();
        if (target is not ( IdentifierNameSyntax or GenericNameSyntax or MemberAccessExpressionSyntax or InvocationExpressionSyntax or ElementAccessExpressionSyntax
                          or ParenthesizedExpressionSyntax or ThisExpressionSyntax or DefaultExpressionSyntax or ObjectCreationExpressionSyntax
                          or PostfixUnaryExpressionSyntax ))
        {
            target = SyntaxFactory.ParenthesizedExpression(target);
        }

        return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, target, SyntaxFactory.IdentifierName("Value"))
                            .WithTriviaFrom(expression);
    }
}
