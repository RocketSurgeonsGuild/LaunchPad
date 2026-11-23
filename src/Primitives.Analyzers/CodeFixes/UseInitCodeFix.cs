using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

/// <summary>
///     Fixes LPAD2008 by turning the <c>set</c> accessor of the value object's property into <c>init</c>, keeping its accessibility and body. A
///     property declared by a record's parameter has no accessor to change, so it is not fixed. Nor is a property the solution assigns where an
///     <c>init</c> accessor can't be called (a method, or a constructor of another type): the fix would leave code that doesn't compile
///     (CS8852), and what that code does with the value object has to change first.
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
            if (root.FindToken(diagnostic.Location.SourceSpan.Start).Parent?.FirstAncestorOrSelf<PropertyDeclarationSyntax>() is not
                { AccessorList: { } accessors } property)
            {
                continue;
            }

            if (Setter(accessors) is not { } setter) continue;
            if (await AssignedOutsideInitializationAsync(context.Document, property, context.CancellationToken).ConfigureAwait(false)) continue;
            context.RegisterCodeFix(
                CodeAction.Create(Title, _ => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(setter, ToInit(setter)))), Title),
                diagnostic
            );
        }
    }

    /// <summary>
    ///     Whether the solution assigns the property anywhere an <c>init</c> accessor can't be called: anywhere but an object initializer, a
    ///     <c>with</c> expression, or a constructor or <c>init</c> accessor of the type that declares it, on <c>this</c>.
    /// </summary>
    private static async Task<bool> AssignedOutsideInitializationAsync(Document document, PropertyDeclarationSyntax declaration, CancellationToken cancellationToken)
    {
        var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (model?.GetDeclaredSymbol(declaration, cancellationToken) is not { } property) return false;
        var references = await SymbolFinder.FindReferencesAsync(property, document.Project.Solution, cancellationToken).ConfigureAwait(false);
        foreach (var location in references.SelectMany(r => r.Locations))
        {
            if (location.IsImplicit || location.Location.SourceTree is not { } tree) continue;
            var root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);
            if (root.FindNode(location.Location.SourceSpan, getInnermostNodeForTie: true) is not ExpressionSyntax name) continue;
            if (Written(name) is not { } written) continue;
            if (!await InitializesThisAsync(written, name, property.ContainingType, location.Document, cancellationToken).ConfigureAwait(false)) return true;
        }

        return false;
    }

    /// <summary>The expression that writes the property named by <paramref name="name" />, or null when it only reads it.</summary>
    private static ExpressionSyntax? Written(ExpressionSyntax name)
    {
        var target = name.Parent is MemberAccessExpressionSyntax access && access.Name == name ? access : name;
        while (target.Parent is ParenthesizedExpressionSyntax parenthesized) target = parenthesized;
        return target.Parent switch
        {
            AssignmentExpressionSyntax assignment when assignment.Left == target => target,
            PrefixUnaryExpressionSyntax prefix when prefix.IsKind(SyntaxKind.PreIncrementExpression) || prefix.IsKind(SyntaxKind.PreDecrementExpression) => target,
            PostfixUnaryExpressionSyntax postfix when postfix.IsKind(SyntaxKind.PostIncrementExpression) || postfix.IsKind(SyntaxKind.PostDecrementExpression) => target,
            ArgumentSyntax { RefKindKeyword.RawKind: (int)SyntaxKind.RefKeyword or (int)SyntaxKind.OutKeyword } => target,
            // A deconstruction: (City, Country) = ...
            ArgumentSyntax { Parent: TupleExpressionSyntax tuple } when Written(tuple) is not null => target,
            _ => null,
        };
    }

    /// <summary>
    ///     Whether the write is one an <c>init</c> accessor allows: an assignment in an object initializer or a <c>with</c> expression, or any
    ///     write in a constructor or <c>init</c> accessor of <paramref name="type" /> to its own property (a deconstruction or a compound
    ///     assignment included).
    /// </summary>
    private static async Task<bool> InitializesThisAsync(
        ExpressionSyntax written,
        ExpressionSyntax name,
        INamedTypeSymbol type,
        Document document,
        CancellationToken cancellationToken
    )
    {
        if (written.Parent is AssignmentExpressionSyntax { Parent: InitializerExpressionSyntax initializer } assignment
         && assignment.IsKind(SyntaxKind.SimpleAssignmentExpression)
         && ( initializer.IsKind(SyntaxKind.ObjectInitializerExpression) || initializer.IsKind(SyntaxKind.WithInitializerExpression) ))
        {
            return true;
        }

        // On this instance: City or this.City.
        if (written != name && written is not MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax }) return false;
        // The member the write is in; a lambda or a local function in a constructor can't call an init accessor either.
        var member = written.FirstAncestorOrSelf<SyntaxNode>(
            n => n is BaseMethodDeclarationSyntax or AccessorDeclarationSyntax or AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax
        );
        var allowed = member switch
        {
            ConstructorDeclarationSyntax constructor => !constructor.Modifiers.Any(SyntaxKind.StaticKeyword),
            AccessorDeclarationSyntax accessor => accessor.IsKind(SyntaxKind.InitAccessorDeclaration),
            _ => false,
        };
        if (!allowed || member!.FirstAncestorOrSelf<TypeDeclarationSyntax>() is not { } owner) return false;
        var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        return SymbolEqualityComparer.Default.Equals(model?.GetDeclaredSymbol(owner, cancellationToken), type);
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
