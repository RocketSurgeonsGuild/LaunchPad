using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

/// <summary>
///     Fixes LPAD2012 by making the public constructor private, so that only the domain primitive itself (its generated <c>Create</c> and
///     <c>TryCreate</c>) can call it. Not offered for the primary constructor of a positional record, whose accessibility can't be changed, nor
///     for a constructor with the parameters of another one (the generator's <c>private T(TValue value)</c>): it is a duplicate, private or not.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MakeConstructorPrivateCodeFix))]
[Shared]
public sealed class MakeConstructorPrivateCodeFix : CodeFixProvider
{
    private const string Title = "Make the constructor private";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [AnalyzerDiagnostics.PublicConstructor.Id];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null || model is null) return;
        foreach (var diagnostic in context.Diagnostics)
        {
            if (diagnostic.Location.SourceTree != root.SyntaxTree) continue;
            if (root.FindToken(diagnostic.Location.SourceSpan.Start).Parent?.FirstAncestorOrSelf<ConstructorDeclarationSyntax>() is not { } constructor) continue;
            var publicIndex = constructor.Modifiers.IndexOf(SyntaxKind.PublicKeyword);
            if (publicIndex < 0) continue;
            if (model.GetDeclaredSymbol(constructor, context.CancellationToken) is not { } symbol || HasTwin(symbol)) continue;
            var modifier = constructor.Modifiers[publicIndex];
            var updated = constructor.WithModifiers(
                constructor.Modifiers.Replace(modifier, SyntaxFactory.Token(modifier.LeadingTrivia, SyntaxKind.PrivateKeyword, modifier.TrailingTrivia))
            );
            context.RegisterCodeFix(
                CodeAction.Create(Title, _ => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(constructor, updated))), Title),
                diagnostic
            );
        }
    }

    /// <summary>Whether the type has another constructor with the same parameter types.</summary>
    private static bool HasTwin(IMethodSymbol constructor) =>
        constructor.ContainingType.InstanceConstructors.Any(
            other => !SymbolEqualityComparer.Default.Equals(other, constructor)
             && !other.IsImplicitlyDeclared
             && other.Parameters.Length == constructor.Parameters.Length
             && other.Parameters.Zip(constructor.Parameters, (a, b) => SymbolEqualityComparer.Default.Equals(a.Type, b.Type)).All(same => same)
        );
}
