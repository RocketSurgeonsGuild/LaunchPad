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
///     Fixes how a domain primitive is declared: adds <c>partial</c> to it or to a type that contains it (LPAD2001), <c>readonly</c> (LPAD2002) or
///     <c>sealed</c> (LPAD2019), and turns a struct or a class into a record (LPAD2003): a <c>readonly partial record struct</c>, or for a class a
///     <c>sealed partial record</c> too. A type declared in several places is one type: the fixes that would leave its declarations disagreeing
///     (LPAD2003, the kind of type; LPAD2019, abstract or static on another part) change every declaration the user wrote, in every file.
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
                    Register(
                        context,
                        diagnostic,
                        diagnostic.Properties.ContainsKey(DeclarationAnalyzer.ContainingTypeProperty) ? "Make the containing type partial" : "Make the domain primitive partial",
                        root,
                        declaration,
                        d => ModifierEditor.Add(d, SyntaxKind.PartialKeyword)
                    );
                    break;
                case "LPAD2002":
                    Register(context, diagnostic, "Make the domain primitive readonly", root, declaration, d => ModifierEditor.Add(d, SyntaxKind.ReadOnlyKeyword));
                    break;
                case "LPAD2019":
                    // sealed on one declaration seals the type, but abstract or static on any other one would contradict it.
                    RegisterOnEveryPart(
                        context,
                        diagnostic,
                        "Make the domain primitive sealed",
                        declaration,
                        (d, isReported) =>
                        {
                            var updated = ModifierEditor.Remove(d, SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword);
                            return isReported ? ModifierEditor.Add(updated, SyntaxKind.SealedKeyword) : updated;
                        }
                    );
                    break;
                case "LPAD2003":
                    if (declaration is ClassDeclarationSyntax)
                    {
                        RegisterOnEveryPart(context, diagnostic, "Convert to a sealed partial record", declaration, (d, isReported) => ToRecord(d, isStruct: false, isReported));
                    }

                    RegisterOnEveryPart(context, diagnostic, "Convert to a readonly partial record struct", declaration, (d, isReported) => ToRecord(d, isStruct: true, isReported));
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
    ///     Registers a fix that rewrites every declaration of the type the user wrote, in whichever document it is; <c>isReported</c> is true for
    ///     the declaration the diagnostic is on, which takes the modifiers one declaration is enough to carry.
    /// </summary>
    private static void RegisterOnEveryPart(
        CodeFixContext context,
        Diagnostic diagnostic,
        string title,
        TypeDeclarationSyntax reported,
        Func<TypeDeclarationSyntax, bool, TypeDeclarationSyntax> fix
    ) =>
        context.RegisterCodeFix(CodeAction.Create(title, ct => FixEveryPartAsync(context.Document, reported, fix, ct), title), diagnostic);

    private static async Task<Solution> FixEveryPartAsync(
        Document document,
        TypeDeclarationSyntax reported,
        Func<TypeDeclarationSyntax, bool, TypeDeclarationSyntax> fix,
        CancellationToken cancellationToken
    )
    {
        var solution = document.Project.Solution;
        var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        var declarations = model?.GetDeclaredSymbol(reported, cancellationToken) is { } type
            ? PrimitiveSymbols.UserDeclarations(type, cancellationToken)
            : [reported];
        if (!declarations.Contains(reported)) declarations = declarations.Add(reported);
        foreach (var group in declarations.GroupBy(d => d.SyntaxTree))
        {
            if (solution.GetDocument(group.Key) is not { } part) continue;
            var root = await group.Key.GetRootAsync(cancellationToken).ConfigureAwait(false);
            var updated = root.ReplaceNodes(group, (original, _) => fix(original, original == reported));
            solution = solution.WithDocumentSyntaxRoot(part.Id, updated);
        }

        return solution;
    }

    /// <summary>
    ///     The declaration as a record: a <c>readonly partial record struct</c> or a <c>sealed partial record</c>, keeping its attributes,
    ///     accessibility, type parameters, primary constructor, base list and members. Another declaration of the same type
    ///     (<paramref name="addModifier" /> false) becomes a <c>partial record struct</c> or <c>partial record</c>: one <c>readonly</c> or
    ///     <c>sealed</c> is enough for the type.
    /// </summary>
    internal static RecordDeclarationSyntax ToRecord(TypeDeclarationSyntax declaration, bool isStruct, bool addModifier = true)
    {
        var removed = isStruct
            ? ModifierEditor.Remove(declaration, SyntaxKind.SealedKeyword, SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword, SyntaxKind.RefKeyword)
            : ModifierEditor.Remove(declaration, SyntaxKind.ReadOnlyKeyword, SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword, SyntaxKind.RefKeyword);
        var updated = ModifierEditor.Add(
            addModifier ? ModifierEditor.Add(removed, isStruct ? SyntaxKind.ReadOnlyKeyword : SyntaxKind.SealedKeyword) : removed,
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
