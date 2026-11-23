using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

/// <summary>
///     Fixes LPAD2016 by renaming the factory method, and every call to it, to <c>Create</c> or <c>Parse</c>: each name the type has no method
///     of with the same parameters (the generated <c>Create(TValue)</c> takes one of them).
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RenameFactoryCodeFix))]
[Shared]
public sealed class RenameFactoryCodeFix : CodeFixProvider
{
    private static readonly ImmutableArray<string> Names = ["Create", "Parse"];

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [AnalyzerDiagnostics.InvalidFactoryName.Id];

    /// <inheritdoc />
    public override FixAllProvider? GetFixAllProvider() => null;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null || model is null) return;
        foreach (var diagnostic in context.Diagnostics)
        {
            if (diagnostic.Location.SourceTree != root.SyntaxTree) continue;
            if (root.FindToken(diagnostic.Location.SourceSpan.Start).Parent?.FirstAncestorOrSelf<MethodDeclarationSyntax>() is not { } declaration) continue;
            if (model.GetDeclaredSymbol(declaration, context.CancellationToken) is not IMethodSymbol method) continue;
            foreach (var name in Names)
            {
                if (Collides(method, name)) continue;
                var title = $"Rename to '{name}'";
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title,
                        ct => Renamer.RenameSymbolAsync(context.Document.Project.Solution, method, new SymbolRenameOptions(), name, ct),
                        title
                    ),
                    diagnostic
                );
            }
        }
    }

    /// <summary>Whether the type already has a method named <paramref name="name" /> with the parameters of <paramref name="method" />.</summary>
    private static bool Collides(IMethodSymbol method, string name) =>
        method.ContainingType.GetMembers(name)
              .OfType<IMethodSymbol>()
              .Any(
                   other => other.Parameters.Length == method.Parameters.Length
                    && other.Parameters.Zip(method.Parameters, (a, b) => SymbolEqualityComparer.Default.Equals(a.Type, b.Type)).All(same => same)
               );
}
