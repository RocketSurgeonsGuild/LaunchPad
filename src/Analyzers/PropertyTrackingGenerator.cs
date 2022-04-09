using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     A generator that is used to copy properties, fields and methods from one type onto another.
/// </summary>
[Generator]
public class PropertyTrackingGenerator : IIncrementalGenerator
{
    private static void GeneratePropertyTracking(
        SourceProductionContext context,
        Compilation compilation,
        TypeDeclarationSyntax declaration,
        INamedTypeSymbol symbol
    )
    {
        if (!declaration.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(GeneratorDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), declaration.GetFullMetadataName())
            );
            return;
        }

        var targetSymbol = (INamedTypeSymbol)symbol.Interfaces.SingleOrDefault(z => z.Name.StartsWith("IPropertyTracking", StringComparison.Ordinal))
                                                  ?.TypeArguments[0];
        var isRecord = declaration is RecordDeclarationSyntax;

        if (targetSymbol.IsRecord != isRecord)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    GeneratorDiagnostics.ParameterMustBeSameTypeOfObject, declaration.Keyword.GetLocation(), declaration.GetFullMetadataName(),
                    declaration.Keyword.IsKind(SyntaxKind.ClassKeyword) ? "record" : "class"
                )
            );
            return;
        }

        var classToInherit = declaration
                            .WithMembers(List<MemberDeclarationSyntax>())
                            .WithAttributeLists(List<AttributeListSyntax>())
                            .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                            .WithBaseList(null);

        var writeableProperties =
            targetSymbol.GetMembers()
                        .OfType<IPropertySymbol>()
                         // only works for `set`able properties not init only
                        .Where(z => !symbol.GetMembers(z.Name).Any())
                        .Where(z => z is { IsStatic: false, IsIndexer: false, IsReadOnly: false });
        if (!targetSymbol.IsRecord)
        {
            // not able to use with operator, so ignore any init only properties.
            writeableProperties = writeableProperties.Where(z => z is { SetMethod.IsInitOnly: false, GetMethod.IsReadOnly: false });
        }

        var changesRecord = RecordDeclaration(Token(SyntaxKind.RecordKeyword), "Changes")
                           .WithModifiers(SyntaxTokenList.Create(Token(SyntaxKind.PublicKeyword)))
                           .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                           .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
            ;
        var getChangedStateMethodInitializer = InitializerExpression(SyntaxKind.ObjectInitializerExpression);
        var applyChangesBody = Block();
        var resetChangesBody = Block();

        foreach (var propertySymbol in writeableProperties)
        {
            var type = ParseTypeName(propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
//            classToInherit = classToInherit.AddMembers(GenerateTrackingProperties(propertySymbol, type));
            classToInherit = classToInherit.AddMembers(GenerateTrackingProperties(propertySymbol, type));
            changesRecord = changesRecord.AddMembers(
                PropertyDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier(propertySymbol.Name))
                   .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                   .WithAccessorList(
                        AccessorList(
                            List(
                                new[]
                                {
                                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                    AccessorDeclaration(SyntaxKind.InitAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                                }
                            )
                        )
                    )
            );
            getChangedStateMethodInitializer = getChangedStateMethodInitializer.AddExpressions(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(propertySymbol.Name),
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(propertySymbol.Name),
                            IdentifierName("HasBeenSet")
                        )
                    )
                )
            );
            applyChangesBody = applyChangesBody.AddStatements(
                GenerateApplyChangesBodyPart(propertySymbol, IdentifierName("value"), isRecord)
            );
            resetChangesBody = resetChangesBody.AddStatements(
                ExpressionStatement(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(propertySymbol.Name),
                            IdentifierName("ResetState")
                        )
                    )
                )
            );
        }

        var getChangedStateMethod =
            MethodDeclaration(
                    ParseTypeName("Changes"), Identifier("GetChangedState")
                )
               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
               .WithBody(
                    Block(
                        SingletonList<StatementSyntax>(
                            ReturnStatement(
                                ObjectCreationExpression(IdentifierName("Changes"))
                                   .WithArgumentList(ArgumentList())
                                   .WithInitializer(getChangedStateMethodInitializer)
                            )
                        )
                    )
                );

        var applyChangesMethod = MethodDeclaration(ParseTypeName(targetSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)), "ApplyChanges")
                                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                .WithParameterList(
                                     ParameterList(
                                         SingletonSeparatedList(
                                             Parameter(Identifier("value")).WithType(
                                                 IdentifierName(targetSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                                             )
                                         )
                                     )
                                 )
                                .WithBody(
                                     applyChangesBody.AddStatements(
                                         ExpressionStatement(InvocationExpression(IdentifierName("ResetChanges"))),
                                         ReturnStatement(IdentifierName("value"))
                                     )
                                 );
        var resetChangesMethod = MethodDeclaration(IdentifierName(symbol.Name), "ResetChanges")
                                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                .WithBody(resetChangesBody.AddStatements(ReturnStatement(ThisExpression())));
        var resetChangesImplementation = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), "ResetChanges")
                                        .WithExplicitInterfaceSpecifier(
                                             ExplicitInterfaceSpecifier(
                                                 GenericName(Identifier("IPropertyTracking"))
                                                    .WithTypeArgumentList(
                                                         TypeArgumentList(
                                                             SingletonSeparatedList<TypeSyntax>(
                                                                 IdentifierName(targetSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                                                             )
                                                         )
                                                     )
                                             )
                                         )
                                        .WithBody(Block().AddStatements(ExpressionStatement(InvocationExpression(IdentifierName("ResetChanges")))));

        classToInherit = classToInherit
           .AddMembers(
                changesRecord,
                getChangedStateMethod,
                applyChangesMethod,
                resetChangesMethod,
                resetChangesImplementation
            );

        var cu = CompilationUnit(
                     List<ExternAliasDirectiveSyntax>(),
                     List(declaration.SyntaxTree.GetCompilationUnitRoot().Usings),
                     List<AttributeListSyntax>(),
                     SingletonList<MemberDeclarationSyntax>(
                         symbol.ContainingNamespace.IsGlobalNamespace
                             ? classToInherit.ReparentDeclaration(context, declaration)
                             : NamespaceDeclaration(ParseName(symbol.ContainingNamespace.ToDisplayString()))
                                .WithMembers(SingletonList<MemberDeclarationSyntax>(classToInherit.ReparentDeclaration(context, declaration)))
                     )
                 )
                .WithLeadingTrivia()
                .WithTrailingTrivia()
                .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
                .WithTrailingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)), CarriageReturnLineFeed);

        context.AddSource(
            $"{Path.GetFileNameWithoutExtension(declaration.SyntaxTree.FilePath)}_{declaration.Identifier.Text}",
            cu.NormalizeWhitespace().GetText(Encoding.UTF8)
        );
    }

    private static StatementSyntax GenerateApplyChangesBodyPart(
        IPropertySymbol propertySymbol, IdentifierNameSyntax valueIdentifier, bool isRecord
    )
    {
        return IfStatement(
            InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(propertySymbol.Name),
                    IdentifierName("HasBeenSet")
                )
            ),
            Block(
                SingletonList<StatementSyntax>(
                    ExpressionStatement(
                        isRecord
                            ? AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                valueIdentifier,
                                WithExpression(
                                    valueIdentifier,
                                    InitializerExpression(
                                        SyntaxKind.WithInitializerExpression,
                                        SingletonSeparatedList<ExpressionSyntax>(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                IdentifierName(propertySymbol.Name),
                                                IdentifierName(propertySymbol.Name)
                                            )
                                        )
                                    )
                                )
                            )
                            : AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, valueIdentifier, IdentifierName(propertySymbol.Name)),
                                IdentifierName(propertySymbol.Name)
                            )
                    )
                )
            )
        );
    }

    private static MemberDeclarationSyntax[] GenerateTrackingProperties(IPropertySymbol propertySymbol, TypeSyntax typeSyntax)
    {
        var fieldName = $"_{propertySymbol.Name}";
        return new MemberDeclarationSyntax[]
        {
            FieldDeclaration(
                    VariableDeclaration(
                            NullableType(
                                GenericName(Identifier("Rocket.Surgery.LaunchPad.Foundation.Assigned"))
                                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(typeSyntax)))
                            )
                        )
                       .WithVariables(
                            SingletonSeparatedList(
                                VariableDeclarator(
                                    Identifier(fieldName)
                                ).WithInitializer(
                                    EqualsValueClause(
                                        ObjectCreationExpression(
                                                GenericName(Identifier("Rocket.Surgery.LaunchPad.Foundation.Assigned"))
                                                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(typeSyntax)))
                                            )
                                           .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList(
                                                        Argument(LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword)))
                                                    )
                                                )
                                            )
                                    )
                                )
                            )
                        )
                )
               .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
            PropertyDeclaration(
                    NullableType(
                        GenericName(Identifier("Rocket.Surgery.LaunchPad.Foundation.Assigned"))
                           .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(typeSyntax)))
                    ),
                    Identifier(propertySymbol.Name)
                )
               .WithAttributeLists(
                    SingletonList(
                        AttributeList(
                            SingletonSeparatedList(
                                Attribute(
                                    ParseName("System.Diagnostics.CodeAnalysis.AllowNull")
                                )
                            )
                        )
                    )
                )
               .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)
                    )
                )
               .WithAccessorList(
                    AccessorList(
                        List(
                            new[]
                            {
                                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                   .WithExpressionBody(ArrowExpressionClause(IdentifierName(fieldName)))
                                   .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(
                                        SyntaxKind.SetAccessorDeclaration
                                    )
                                   .WithExpressionBody(
                                        ArrowExpressionClause(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression, IdentifierName(fieldName), IdentifierName("Value")
                                                ),
                                                BinaryExpression(
                                                    SyntaxKind.CoalesceExpression,
                                                    ConditionalAccessExpression(IdentifierName("value"), MemberBindingExpression(IdentifierName("Value"))),
                                                    LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword))
                                                )
                                            )
                                        )
                                    )
                                   .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                            }
                        )
                    )
                )
        };
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var values = context.SyntaxProvider
                            .CreateSyntaxProvider(
                                 static (node, _) =>
                                     node is (ClassDeclarationSyntax or RecordDeclarationSyntax) and TypeDeclarationSyntax
                                     {
                                         BaseList: { } baseList
                                     } && baseList.Types.Any(
                                         z => z.Type is GenericNameSyntax qns && qns.Identifier.Text.EndsWith(
                                             "IPropertyTracking", StringComparison.OrdinalIgnoreCase
                                         )
                                     ),
                                 static (syntaxContext, token) => (
                                     syntax: (TypeDeclarationSyntax)syntaxContext.Node, semanticModel: syntaxContext.SemanticModel,
                                     symbol: syntaxContext.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)syntaxContext.Node, token)!
                                 )
                             ).Combine(
                                 context.CompilationProvider
                             )
                            .Select(
                                 static (tuple, _) => (
                                     tuple.Left.syntax,
                                     tuple.Left.semanticModel,
                                     tuple.Left.symbol,
                                     compilation: tuple.Right
                                 )
                             )
                            .Where(x => x.symbol is not null);

        context.RegisterSourceOutput(
            values,
            static (productionContext, tuple) => GeneratePropertyTracking(productionContext, tuple.compilation, tuple.syntax, tuple.symbol)
        );
    }
}
