using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

/// <summary>
///     Fixes how a domain primitive is declared: adds <c>partial</c> (LPAD2001), <c>readonly</c> (LPAD2002) or <c>sealed</c> (LPAD2019), and turns
///     a struct or a class into a record (LPAD2003): a <c>readonly partial record struct</c>, or for a class a <c>sealed partial record</c> too.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DeclarationCodeFix))]
[Shared]
public sealed class DeclarationCodeFix : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
    [
        AnalyzerDiagnostics.MustBePartial.Id,
        AnalyzerDiagnostics.StructMustBeReadOnly.Id,
        AnalyzerDiagnostics.MustBeRecord.Id,
        AnalyzerDiagnostics.ClassMustBeSealed.Id,
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
            if (root.FindToken(diagnostic.Location.SourceSpan.Start).Parent?.FirstAncestorOrSelf<TypeDeclarationSyntax>() is not { } declaration) continue;
            switch (diagnostic.Id)
            {
                case "LPAD2001":
                    Register(context, diagnostic, "Make the domain primitive partial", root, declaration, d => ModifierEditor.Add(d, SyntaxKind.PartialKeyword));
                    break;
                case "LPAD2002":
                    Register(context, diagnostic, "Make the domain primitive readonly", root, declaration, d => ModifierEditor.Add(d, SyntaxKind.ReadOnlyKeyword));
                    break;
                case "LPAD2019":
                    Register(
                        context,
                        diagnostic,
                        "Make the domain primitive sealed",
                        root,
                        declaration,
                        d => ModifierEditor.Add(ModifierEditor.Remove(d, SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword), SyntaxKind.SealedKeyword)
                    );
                    break;
                case "LPAD2003":
                    if (declaration is ClassDeclarationSyntax)
                    {
                        Register(context, diagnostic, "Convert to a sealed partial record", root, declaration, d => ToRecord(d, isStruct: false));
                    }

                    Register(context, diagnostic, "Convert to a readonly partial record struct", root, declaration, d => ToRecord(d, isStruct: true));
                    break;
                default:
                    break;
            }
        }
    }

    private static void Register(
        CodeFixContext context,
        Diagnostic diagnostic,
        string title,
        SyntaxNode root,
        TypeDeclarationSyntax declaration,
        Func<TypeDeclarationSyntax, TypeDeclarationSyntax> fix
    ) =>
        context.RegisterCodeFix(
            CodeAction.Create(title, _ => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(declaration, fix(declaration)))), title),
            diagnostic
        );

    /// <summary>
    ///     The declaration as a record: a <c>readonly partial record struct</c> or a <c>sealed partial record</c>, keeping its attributes,
    ///     accessibility, type parameters, primary constructor, base list and members.
    /// </summary>
    internal static RecordDeclarationSyntax ToRecord(TypeDeclarationSyntax declaration, bool isStruct)
    {
        var updated = isStruct
            ? ModifierEditor.Add(
                ModifierEditor.Add(
                    ModifierEditor.Remove(declaration, SyntaxKind.SealedKeyword, SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword, SyntaxKind.RefKeyword),
                    SyntaxKind.ReadOnlyKeyword
                ),
                SyntaxKind.PartialKeyword
            )
            : ModifierEditor.Add(
                ModifierEditor.Add(
                    ModifierEditor.Remove(declaration, SyntaxKind.ReadOnlyKeyword, SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword, SyntaxKind.RefKeyword),
                    SyntaxKind.SealedKeyword
                ),
                SyntaxKind.PartialKeyword
            );

        var keyword = updated.Keyword;
        var recordKeyword = SyntaxFactory.Token(keyword.LeadingTrivia, SyntaxKind.RecordKeyword, isStruct ? SyntaxFactory.TriviaList(SyntaxFactory.Space) : keyword.TrailingTrivia);
        var structKeyword = isStruct ? SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.StructKeyword, keyword.TrailingTrivia) : default;
        return SyntaxFactory.RecordDeclaration(
            isStruct ? SyntaxKind.RecordStructDeclaration : SyntaxKind.RecordDeclaration,
            updated.AttributeLists,
            updated.Modifiers,
            recordKeyword,
            structKeyword,
            updated.Identifier,
            updated.TypeParameterList,
            updated.ParameterList,
            updated.BaseList,
            updated.ConstraintClauses,
            updated.OpenBraceToken,
            updated.Members,
            updated.CloseBraceToken,
            updated.SemicolonToken
        );
    }
}

/// <summary>Adds and removes the modifiers of a type declaration, keeping its trivia where it was.</summary>
internal static class ModifierEditor
{
    /// <summary>
    ///     Adds <paramref name="kind" /> if the declaration lacks it: <c>partial</c> last, any other modifier just before <c>partial</c> (after the
    ///     accessibility), so the result reads <c>public sealed partial record</c>.
    /// </summary>
    public static TypeDeclarationSyntax Add(TypeDeclarationSyntax declaration, SyntaxKind kind)
    {
        var modifiers = declaration.Modifiers;
        if (modifiers.Any(kind)) return declaration;
        if (modifiers.Count == 0)
        {
            var first = SyntaxFactory.Token(declaration.Keyword.LeadingTrivia, kind, SyntaxFactory.TriviaList(SyntaxFactory.Space));
            return declaration.WithKeyword(declaration.Keyword.WithLeadingTrivia()).WithModifiers(SyntaxFactory.TokenList(first));
        }

        var partial = modifiers.IndexOf(SyntaxKind.PartialKeyword);
        var index = kind == SyntaxKind.PartialKeyword || partial < 0 ? modifiers.Count : partial;
        var token = SyntaxFactory.Token(SyntaxFactory.TriviaList(), kind, SyntaxFactory.TriviaList(SyntaxFactory.Space));
        if (index == 0)
        {
            token = token.WithLeadingTrivia(modifiers[0].LeadingTrivia);
            modifiers = modifiers.Replace(modifiers[0], modifiers[0].WithLeadingTrivia());
        }

        return declaration.WithModifiers(modifiers.Insert(index, token));
    }

    /// <summary>Removes each of <paramref name="kinds" /> the declaration has; the next token takes over the leading trivia of a removed first one.</summary>
    public static TypeDeclarationSyntax Remove(TypeDeclarationSyntax declaration, params SyntaxKind[] kinds)
    {
        foreach (var kind in kinds)
        {
            var modifiers = declaration.Modifiers;
            var index = modifiers.IndexOf(kind);
            if (index < 0) continue;
            var leading = modifiers[index].LeadingTrivia;
            modifiers = modifiers.RemoveAt(index);
            declaration = index > 0
                ? declaration.WithModifiers(modifiers)
                : modifiers.Count > 0
                    ? declaration.WithModifiers(modifiers.Replace(modifiers[0], modifiers[0].WithLeadingTrivia(leading)))
                    : declaration.WithModifiers(modifiers).WithKeyword(declaration.Keyword.WithLeadingTrivia(leading));
        }

        return declaration;
    }
}
