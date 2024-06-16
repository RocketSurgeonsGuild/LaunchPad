using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Analyzers;

internal class RuleExpressionVisitor(ImmutableHashSet<string> excludedMembers) : CSharpSyntaxWalker
{
    private readonly ImmutableArray<InvocationExpressionSyntax>.Builder _results = ImmutableArray.CreateBuilder<InvocationExpressionSyntax>();
    public ImmutableArray<InvocationExpressionSyntax> Results => _results.ToImmutable();

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        SyntaxNode? parent = node;
        while (parent is { } and not { Parent: StatementSyntax, })
        {
            parent = parent.Parent;
        }

        if (parent is not InvocationExpressionSyntax parentNode) throw new("Unable to find invocation node");
        switch (node)
        {
            case
            {
                Expression: SimpleNameSyntax { Identifier.ValueText: "RuleFor" or "RuleForEach", }
                         or MemberAccessExpressionSyntax { Name.Identifier.Text: "RuleFor" or "RuleForEach", },
                ArgumentList.Arguments: [{ Expression: SimpleLambdaExpressionSyntax { Body: MemberAccessExpressionSyntax memberAccessExpressionSyntax, }, },],
            }:
                HandleRuleFor(parentNode, memberAccessExpressionSyntax);
                break;
            case
            {
                Expression: SimpleNameSyntax { Identifier.ValueText: "RuleSet", } or MemberAccessExpressionSyntax { Name.Identifier.Text: "RuleSet", },
                ArgumentList.Arguments: [_, var action,],
            }:
                HandleRuleSet(parentNode, action);
                break;
            case
            {
                Expression: SimpleNameSyntax { Identifier.ValueText: "When" or "WhenAsync", }
                         or MemberAccessExpressionSyntax { Name.Identifier.Text: "When" or "WhenAsync", },
                ArgumentList.Arguments: [var predicate, var whenAction,],
            }:
                HandleWhen(parentNode, predicate, whenAction, null);
                break;
            case
            {
                Expression: SimpleNameSyntax { Identifier.ValueText: "Otherwise", }
                         or MemberAccessExpressionSyntax { Name.Identifier.Text: "Otherwise", },
                ArgumentList.Arguments: [var otherwiseAction2,],
            }:
                ( var predicate2, var whenAction2 ) = parentNode
                                                     .DescendantNodesAndSelf()
                                                     .OfType<InvocationExpressionSyntax>()
                                                     .Select(
                                                          static z => z is
                                                          {
                                                              Expression: SimpleNameSyntax { Identifier.ValueText: "When" or "WhenAsync", }
                                                                       or MemberAccessExpressionSyntax { Name.Identifier.Text: "When" or "WhenAsync", },
                                                              ArgumentList.Arguments: [var p, var action,],
                                                          }
                                                              ? ( p, action )
                                                              : ( null!, null! )
                                                      )
                                                     .First(z => z is { action: { }, p: { }, });

                HandleWhen(parentNode, predicate2, whenAction2, otherwiseAction2);
                break;
            default:
                base.VisitInvocationExpression(node);
                break;
        }
    }

    private void HandleRuleFor(InvocationExpressionSyntax parent, MemberAccessExpressionSyntax memberAccessExpressionSyntax)
    {
        if (excludedMembers.Contains(memberAccessExpressionSyntax.Name.Identifier.Text)) return;
        _results.Add(parent);
    }

    private void HandleRuleSet(InvocationExpressionSyntax parent, ArgumentSyntax action)
    {
        // TODO: Support methods?
        if (HandleNestedAction(action) is { } updatedAction) _results.Add(parent.ReplaceNode(action, updatedAction));
    }

    private void HandleWhen(InvocationExpressionSyntax parent, ArgumentSyntax predicate, ArgumentSyntax whenAction, ArgumentSyntax? otherwiseAction)
    {
        // TODO: this needs it's own visitor
//            var visitor = new RuleExpressionVisitor(excludedMembers);
//            visitor.Visit(predicate);
        var parameter = predicate.DescendantNodes().OfType<ParameterSyntax>().First();
        var predicateContainsExcludedMember = predicate
                                             .DescendantNodes()
                                             .OfType<MemberAccessExpressionSyntax>()
                                             .Where(
                                                  z => z is { Expression: SimpleNameSyntax expression, Name: { } name, }
                                                   && expression.Identifier.Text == parameter.Identifier.Text
                                              )
                                             .Any(z => excludedMembers.Contains(z.Name.Identifier.Text));
        if (predicateContainsExcludedMember) return;

        if (otherwiseAction is null && HandleNestedAction(whenAction) is { } updatedWhenAction)
        {
            _results.Add(parent.ReplaceNode(whenAction, updatedWhenAction));
            return;
        }

        if (otherwiseAction is null) return;

        switch ( HandleNestedAction(whenAction), HandleNestedAction(otherwiseAction) )
        {
            case (null, null):
                return;
            case ({ } updatedWhenAction2, null):
                _results.Add(
                    parent
                       .RemoveNode(otherwiseAction, SyntaxRemoveOptions.KeepEndOfLine)!
                       .ReplaceNode(whenAction, updatedWhenAction2)
                );
                break;
            case (null, { } updatedOtherwiseAction2):
                parent = parent.ReplaceNodes(
                    [whenAction, otherwiseAction,],
                    (syntax, argumentSyntax) => syntax == otherwiseAction
                        ? HandleNestedAction(otherwiseAction)
                     ?? argumentSyntax.WithExpression(SyntaxFactory.ParenthesizedLambdaExpression().WithBlock(SyntaxFactory.Block()))
                        : syntax == whenAction
                            ? argumentSyntax.WithExpression(SyntaxFactory.ParenthesizedLambdaExpression().WithBlock(SyntaxFactory.Block()))
                            : argumentSyntax
                );
                _results.Add(parent);
                break;
            case ({ }, { }):
                parent = parent.ReplaceNodes(
                    [whenAction, otherwiseAction,],
                    (syntax, argumentSyntax) =>
                    {
                        if (syntax == otherwiseAction)
                            return HandleNestedAction(otherwiseAction)
                             ?? argumentSyntax.WithExpression(SyntaxFactory.ParenthesizedLambdaExpression().WithBlock(SyntaxFactory.Block()));
                        if (syntax == whenAction)
                            return HandleNestedAction(whenAction)
                             ?? argumentSyntax.WithExpression(SyntaxFactory.ParenthesizedLambdaExpression().WithBlock(SyntaxFactory.Block()));
                        return argumentSyntax;
                    }
                );
                _results.Add(parent);
                break;
        }
    }

    private ArgumentSyntax? HandleNestedAction(ArgumentSyntax? action)
    {
        if (action is null) return null;
        // TODO: Support methods?
        var visitor = new RuleExpressionVisitor(excludedMembers);
        visitor.Visit(action);
        if (visitor.Results.Length == 0) return null;

        var removeNodes = action
                         .DescendantNodes()
                         .OfType<InvocationExpressionSyntax>()
                         .Where(z => z.Parent is StatementSyntax)
                         .Except(visitor.Results)
                         .Select(z => z.Parent)
                         .ToImmutableArray();
        return action.RemoveNodes(removeNodes, SyntaxRemoveOptions.KeepEndOfLine)!;
    }
}