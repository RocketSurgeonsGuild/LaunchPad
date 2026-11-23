using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

/// <summary>Fixes LPAD2015 by writing a <c>&lt;summary&gt;</c> to fill in above the member, at its indentation.</summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddDocumentationCodeFix))]
[Shared]
public sealed class AddDocumentationCodeFix : CodeFixProvider
{
    private const string Title = "Add XML documentation";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [AnalyzerDiagnostics.MissingDocumentation.Id];

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
            if (root.FindToken(diagnostic.Location.SourceSpan.Start).Parent?.FirstAncestorOrSelf<MemberDeclarationSyntax>() is not { } member) continue;
            context.RegisterCodeFix(
                CodeAction.Create(Title, _ => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(member, Document(member, AddNotEmptyCodeFix.EndOfLine(root))))), Title),
                diagnostic
            );
        }
    }

    /// <summary>The member with a summary placed after its other leading trivia (comments, blank lines) and before its indentation.</summary>
    internal static MemberDeclarationSyntax Document(MemberDeclarationSyntax member, string endOfLine)
    {
        var leading = member.GetLeadingTrivia();
        var indentation = AddNotEmptyCodeFix.Indentation(leading);
        var indent = indentation.ToFullString();
        var summary = SyntaxFactory.ParseLeadingTrivia(
            $"{indent}/// <summary>{endOfLine}{indent}/// {Describe(member)}{endOfLine}{indent}/// </summary>{endOfLine}"
        );
        var insertAt = indentation.Count > 0 ? leading.Count - 1 : leading.Count;
        return member.WithLeadingTrivia(leading.InsertRange(insertAt, summary));
    }

    /// <summary>A sentence to start from: what kind of member it is, named.</summary>
    private static string Describe(MemberDeclarationSyntax member) =>
        member switch
        {
            MethodDeclarationSyntax method => $"{method.Identifier.ValueText}.",
            PropertyDeclarationSyntax property => $"Gets the {property.Identifier.ValueText}.",
            FieldDeclarationSyntax { Declaration.Variables: [var variable, ..] } => $"The {variable.Identifier.ValueText}.",
            _ => "",
        };
}
