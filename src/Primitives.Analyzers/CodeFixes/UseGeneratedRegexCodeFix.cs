using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

/// <summary>
///     Fixes LPAD2023 by replacing a <c>[Regex]</c> with a <c>[GeneratedRegex]</c> property of the domain primitive, with the same pattern, the
///     options and the one second timeout the generator gives a <c>[Regex]</c>:
///     <code>
///     [GeneratedRegex("^[A-Z]{3}$", RegexOptions.CultureInvariant, 1000)]
///     private static partial Regex Format { get; }
///     </code>
///     The property is named Format (Format2, Format3, ... when the name is taken), never Pattern followed by a number, the names of the fields
///     the generator writes for the <c>[Regex]</c> rules left.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseGeneratedRegexCodeFix))]
[Shared]
public sealed class UseGeneratedRegexCodeFix : CodeFixProvider
{
    private const string Title = "Use a [GeneratedRegex]";
    private const string RegularExpressions = "global::System.Text.RegularExpressions";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [AnalyzerDiagnostics.UseGeneratedRegex.Id];

    // Each fix names its property against the members the type had, so two fixes merged by a fix all could pick the same name.
    /// <inheritdoc />
    public override FixAllProvider? GetFixAllProvider() => null;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;
        foreach (var diagnostic in context.Diagnostics)
        {
            if (diagnostic.Location.SourceTree != root.SyntaxTree) continue;
            if (root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<AttributeSyntax>() is not
                { Parent: AttributeListSyntax { Parent: TypeDeclarationSyntax }, ArgumentList.Arguments: [{ NameEquals: null, NameColon: null }] } attribute)
            {
                continue;
            }

            context.RegisterCodeFix(CodeAction.Create(Title, ct => FixAsync(context.Document, attribute, ct), Title), diagnostic);
        }
    }

    private static async Task<Document> FixAsync(Document document, AttributeSyntax attribute, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || model is null || attribute is not { Parent: AttributeListSyntax { Parent: TypeDeclarationSyntax declaration } list }) return document;
        if (model.GetDeclaredSymbol(declaration, cancellationToken) is not { } type) return document;

        // Not a name of a member of the type, in any of its parts, nor the type's own name.
        var name = "Format";
        for (var i = 2; type.MemberNames.Contains(name) || type.Name == name; i++)
        {
            name = $"Format{i}";
        }

        // Every line written in the line ending the document uses.
        var endOfLine = AddNotEmptyCodeFix.EndOfLine(root);
        var newLine = SyntaxFactory.EndOfLine(endOfLine);

        // The pattern as it is written: a literal of any form, or a constant.
        var pattern = attribute.ArgumentList!.Arguments[0].Expression.WithoutTrivia();
        var member = (PropertyDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(
            $"[{RegularExpressions}.GeneratedRegex({pattern}, {RegularExpressions}.RegexOptions.CultureInvariant, 1000)]{endOfLine}"
          + $"private static partial {RegularExpressions}.Regex {name} {{ get; }}{endOfLine}"
        )!;

        // A comment after the [Regex] stays with the pattern when its attribute list goes: after the [GeneratedRegex].
        if (list.Attributes.Count == 1
         && list.GetTrailingTrivia().Where(z => z.IsKind(SyntaxKind.SingleLineCommentTrivia) || z.IsKind(SyntaxKind.MultiLineCommentTrivia)).ToArray() is
         { Length: > 0 } comments)
        {
            var generated = member.AttributeLists[0];
            var trivia = new List<SyntaxTrivia>();
            foreach (var comment in comments)
            {
                trivia.Add(SyntaxFactory.Space);
                trivia.Add(comment);
            }

            trivia.Add(newLine);
            member = member.ReplaceNode(generated, generated.WithTrailingTrivia(trivia));
        }

        member = member.WithAdditionalAnnotations(Simplifier.Annotation, Simplifier.AddImportsAnnotation, Formatter.Annotation);

        var tracked = new SyntaxAnnotation();
        root = root.ReplaceNode(declaration, declaration.WithAdditionalAnnotations(tracked));
        var annotated = root.GetAnnotatedNodes(tracked).OfType<TypeDeclarationSyntax>().Single();
        var attributeList = annotated.AttributeLists[declaration.AttributeLists.IndexOf(list)];
        root = RemoveNormalizationCodeFix.RemoveAttribute(root, attributeList, attributeList.Attributes[list.Attributes.IndexOf(attribute)]);
        annotated = root.GetAnnotatedNodes(tracked).OfType<TypeDeclarationSyntax>().Single();

        TypeDeclarationSyntax updated;
        if (annotated.OpenBraceToken.IsKind(SyntaxKind.None))
        {
            // `record struct Code;` becomes a body. The braces go where the semicolon was, after a parameter list, a base list or constraints,
            // each on its own line, and the semicolon's trailing trivia (the line end) goes after the closing brace.
            var semicolon = annotated.SemicolonToken;
            updated = annotated
                     .WithSemicolonToken(default)
                     .WithOpenBraceToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(newLine), SyntaxKind.OpenBraceToken, SyntaxFactory.TriviaList(newLine)))
                     .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken).WithTrailingTrivia(semicolon.TrailingTrivia))
                     .AddMembers(member);
            updated = updated.WithAdditionalAnnotations(Formatter.Annotation);
        }
        else
        {
            // A blank line between the members the body has and the property.
            if (annotated.Members.Count > 0) member = member.WithLeadingTrivia(newLine);
            updated = annotated.AddMembers(member);
        }

        document = document.WithSyntaxRoot(root.ReplaceNode(annotated, updated));
        // The using the property may need, in the document's line ending too.
        var options = document.Project.Solution.Options.WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, endOfLine);
        document = await ImportAdder.AddImportsAsync(document, Simplifier.AddImportsAnnotation, options, cancellationToken).ConfigureAwait(false);
        document = await Simplifier.ReduceAsync(document, Simplifier.Annotation, options, cancellationToken).ConfigureAwait(false);
        return await Formatter.FormatAsync(document, Formatter.Annotation, options, cancellationToken).ConfigureAwait(false);
    }
}
