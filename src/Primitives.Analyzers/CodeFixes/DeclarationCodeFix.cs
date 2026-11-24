using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

/// <summary>
///     Fixes how a domain primitive is declared: adds <c>partial</c> to it or to a type that contains it (LPAD2001), <c>readonly</c> (LPAD2002) or
///     <c>sealed</c> (LPAD2019), and turns a struct or a class into a record (LPAD2003): a <c>readonly partial record struct</c>, or for a class a
///     <c>sealed partial record</c> too. A smart enum is a <c>closed partial class</c> or a <c>readonly partial record struct</c> instead:
///     LPAD2019 makes its class <c>closed</c>, and LPAD2003 turns a record class or a struct that isn't a record into either form, never a
///     <c>sealed</c> record; both rewrite how its members are made to suit the form (<c>new Pending_(1)</c> or <c>new(1)</c>), each member found
///     and each <c>new</c> read by what it binds to, as the generator reads them, however the smart enum or the member's class is spelled. A type declared in
///     several places is one type: the fixes that would leave its declarations disagreeing (LPAD2003, the kind of type; LPAD2019, abstract,
///     static, sealed or closed on another part) change every declaration the user wrote, in every file.
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
            var isSmartEnum = diagnostic.Properties.ContainsKey(DeclarationAnalyzer.SmartEnumProperty);
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
                case "LPAD2019" when isSmartEnum:
                    // closed on one declaration closes the type, but abstract, static or sealed on any other one would contradict it. A
                    // closed class makes each member with new of its own class, so new(1) or new Level(1) in any part becomes new Low_(1).
                    RegisterOnEveryPart(
                        context,
                        diagnostic,
                        "Make the smart enum closed",
                        declaration,
                        (d, isReported) =>
                        {
                            var updated = ModifierEditor.Remove(d, SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword, SyntaxKind.SealedKeyword);
                            return isReported ? ModifierEditor.Add(updated, SyntaxKind.ClosedKeyword) : updated;
                        },
                        toMemberClasses: true
                    );
                    break;
                case "LPAD2019":
                    // sealed on one declaration seals the type, but abstract, static or closed on any other one would contradict it.
                    RegisterOnEveryPart(
                        context,
                        diagnostic,
                        "Make the domain primitive sealed",
                        declaration,
                        (d, isReported) =>
                        {
                            var updated = ModifierEditor.Remove(d, SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ClosedKeyword);
                            return isReported ? ModifierEditor.Add(updated, SyntaxKind.SealedKeyword) : updated;
                        }
                    );
                    break;
                case "LPAD2003" when isSmartEnum:
                    RegisterOnEveryPart(context, diagnostic, "Convert to a closed partial class", declaration, ToClosedClass, toMemberClasses: true);
                    RegisterOnEveryPart(
                        context,
                        diagnostic,
                        "Convert to a readonly partial record struct",
                        declaration,
                        (d, isReported) => ToRecord(d, isStruct: true, isReported),
                        toMemberClasses: false
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
    ///     the declaration the diagnostic is on, which takes the modifiers one declaration is enough to carry. For a smart enum,
    ///     <paramref name="toMemberClasses" /> says how the form the fix gives it makes its members (<see cref="WithSmartEnumMembers" />), which
    ///     is rewritten in each declaration before <paramref name="fix" /> sees it.
    /// </summary>
    private static void RegisterOnEveryPart(
        CodeFixContext context,
        Diagnostic diagnostic,
        string title,
        TypeDeclarationSyntax reported,
        Func<TypeDeclarationSyntax, bool, TypeDeclarationSyntax> fix,
        bool? toMemberClasses = null
    ) =>
        context.RegisterCodeFix(CodeAction.Create(title, ct => FixEveryPartAsync(context.Document, reported, fix, toMemberClasses, ct), title), diagnostic);

    private static async Task<Solution> FixEveryPartAsync(
        Document document,
        TypeDeclarationSyntax reported,
        Func<TypeDeclarationSyntax, bool, TypeDeclarationSyntax> fix,
        bool? toMemberClasses,
        CancellationToken cancellationToken
    )
    {
        var solution = document.Project.Solution;
        var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        var type = model?.GetDeclaredSymbol(reported, cancellationToken);
        var declarations = type is { } ? PrimitiveSymbols.UserDeclarations(type, cancellationToken) : [reported];
        if (!declarations.Contains(reported)) declarations = declarations.Add(reported);
        foreach (var group in declarations.GroupBy(d => d.SyntaxTree))
        {
            if (solution.GetDocument(group.Key) is not { } part) continue;
            var root = await group.Key.GetRootAsync(cancellationToken).ConfigureAwait(false);
            // The members are read with the semantic model of the part they are declared in, in the compilation the type was read from: a
            // part fixed before this one would make another compilation, whose symbols aren't the type's.
            var partModel = toMemberClasses is null || type is null
                ? null
                : await document.Project.Solution.GetDocument(group.Key)!.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var updated = root.ReplaceNodes(
                group,
                (original, _) => fix(
                    partModel is null ? original : WithSmartEnumMembers(original, type!, partModel, toMemberClasses!.Value, cancellationToken),
                    original == reported
                )
            );
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
            ? ModifierEditor.Remove(declaration, SyntaxKind.SealedKeyword, SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword, SyntaxKind.RefKeyword, SyntaxKind.ClosedKeyword)
            : ModifierEditor.Remove(declaration, SyntaxKind.ReadOnlyKeyword, SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword, SyntaxKind.RefKeyword, SyntaxKind.ClosedKeyword);
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

    /// <summary>
    ///     A record, a record struct or a struct as a smart enum: a <c>closed partial class</c>, keeping its attributes, accessibility, type
    ///     parameters, parameter list, base list and members (which its fix first makes with <c>new</c> of their own classes, with
    ///     <see cref="WithSmartEnumMembers" />). Another declaration of the same type (<paramref name="addClosed" /> false) becomes a
    ///     <c>partial class</c>: one <c>closed</c> is enough for the type.
    /// </summary>
    internal static ClassDeclarationSyntax ToClosedClass(TypeDeclarationSyntax declaration, bool addClosed = true)
    {
        var removed = ModifierEditor.Remove(
            declaration,
            SyntaxKind.ReadOnlyKeyword,
            SyntaxKind.SealedKeyword,
            SyntaxKind.AbstractKeyword,
            SyntaxKind.StaticKeyword,
            SyntaxKind.RefKeyword
        );
        var updated = ModifierEditor.Add(addClosed ? ModifierEditor.Add(removed, SyntaxKind.ClosedKeyword) : removed, SyntaxKind.PartialKeyword);

        // `record`, `record class`, `record struct` or `struct`: one `class` keyword, with the trivia before the first and after the last.
        var first = updated.Keyword;
        var last = updated is RecordDeclarationSyntax { ClassOrStructKeyword: { RawKind: not 0 } classOrStruct } ? classOrStruct : first;
        var classKeyword = SyntaxFactory.Token(first.LeadingTrivia, SyntaxKind.ClassKeyword, last.TrailingTrivia);
        return SyntaxFactory.ClassDeclaration(
            updated.AttributeLists,
            updated.Modifiers,
            classKeyword,
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

    /// <summary>
    ///     <paramref name="declaration" />, a declaration of the smart enum <paramref name="smartEnum" /> still in the tree
    ///     <paramref name="model" /> reads, with each of its members made as its form makes it: with <c>new</c> of the member's own class in a
    ///     closed class (<paramref name="toMemberClasses" />: <c>new(1)</c>, or <c>new</c> of the smart enum however it is spelled, such as
    ///     <c>new Sample.Level(1)</c>, becomes <c>new Low_(1)</c>), with a target-typed <c>new</c> in a record struct (<c>new Low_(1)</c>, or
    ///     <c>new Level.Low_(1)</c>, becomes <c>new(1)</c>). A member is found as the generator finds it: a static field or property with an
    ///     initializer whose type is the smart enum, however that type is spelled. Any other initializer is left as it is.
    /// </summary>
    internal static TypeDeclarationSyntax WithSmartEnumMembers(
        TypeDeclarationSyntax declaration,
        INamedTypeSymbol smartEnum,
        SemanticModel model,
        bool toMemberClasses,
        CancellationToken cancellationToken
    )
    {
        var members = declaration.Members.Select(
            member => member switch
            {
                FieldDeclarationSyntax field when field.Modifiers.Any(SyntaxKind.StaticKeyword) => field.WithDeclaration(
                    field.Declaration
                         .WithType(
                              field.Declaration.Variables.Any(v => IsMember(model.GetDeclaredSymbol(v, cancellationToken)))
                                  ? AsStructType(field.Declaration.Type)
                                  : field.Declaration.Type
                          )
                         .WithVariables(
                              SyntaxFactory.SeparatedList(
                                  field.Declaration.Variables.Select(
                                      v => v.Initializer is { } initializer && IsMember(model.GetDeclaredSymbol(v, cancellationToken))
                                          ? v.WithInitializer(initializer.WithValue(MakeMember(initializer.Value, v.Identifier.ValueText)))
                                          : v
                                  ),
                                  field.Declaration.Variables.GetSeparators()
                              )
                          )
                ),
                PropertyDeclarationSyntax { Initializer: { } initializer } property when IsMember(model.GetDeclaredSymbol(property, cancellationToken)) =>
                    property.WithType(AsStructType(property.Type))
                            .WithInitializer(initializer.WithValue(MakeMember(initializer.Value, property.Identifier.ValueText))),
                _ => member,
            }
        );
        return declaration.WithMembers(SyntaxFactory.List(members));

        bool IsMember(ISymbol? symbol) => symbol is { } && PrimitiveReader.IsStaticOfItsType(symbol, smartEnum);

        // `Level?` is the smart enum in a class, but Nullable<Level> in a record struct, which isn't a member: the struct form drops the `?`.
        TypeSyntax AsStructType(TypeSyntax type) =>
            !toMemberClasses && type is NullableTypeSyntax nullable ? nullable.ElementType.WithTriviaFrom(nullable) : type;

        ExpressionSyntax MakeMember(ExpressionSyntax value, string memberName)
        {
            var memberClass = memberName + "_";
            return value switch
            {
                ImplicitObjectCreationExpressionSyntax creation when toMemberClasses => SyntaxFactory.ObjectCreationExpression(
                    creation.NewKeyword.WithTrailingTrivia(SyntaxFactory.Space),
                    SyntaxFactory.IdentifierName(memberClass),
                    creation.ArgumentList,
                    creation.Initializer
                ),
                ObjectCreationExpressionSyntax creation when toMemberClasses
                                                          && SymbolEqualityComparer.Default.Equals(model.GetTypeInfo(creation, cancellationToken).Type, smartEnum) =>
                    creation.WithType(SyntaxFactory.IdentifierName(memberClass).WithTriviaFrom(creation.Type)),
                ObjectCreationExpressionSyntax creation when !toMemberClasses
                                                          && PrimitiveReader.IsMemberClass(creation.Type, memberClass, smartEnum, model, cancellationToken) =>
                    SyntaxFactory.ImplicitObjectCreationExpression(
                        creation.NewKeyword.WithTrailingTrivia(creation.Type.GetTrailingTrivia()),
                        creation.ArgumentList ?? SyntaxFactory.ArgumentList(),
                        creation.Initializer
                    ),
                _ => value,
            };
        }
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
