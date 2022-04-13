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
        TypeDeclarationSyntax declaration,
        INamedTypeSymbol symbol,
        INamedTypeSymbol targetSymbol
    )
    {
        if (!declaration.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(GeneratorDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), declaration.GetFullMetadataName())
            );
            return;
        }

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
        var namespaces = new HashSet<string>();

        static void AddNamespacesFromPropertyType(HashSet<string> namespaces, ITypeSymbol symbol)
        {
            namespaces.Add(symbol.ContainingNamespace.GetFullMetadataName());
            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.IsGenericType)
                {
                    foreach (var genericType in namedTypeSymbol.TypeArguments)
                    {
                        AddNamespacesFromPropertyType(namespaces, genericType);
                    }
                }
            }
        }

        foreach (var propertySymbol in writeableProperties)
        {
            var type = ParseTypeName(propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            AddNamespacesFromPropertyType(namespaces, propertySymbol.Type);

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
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(propertySymbol.Name),
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    GenericName(Identifier("Rocket.Surgery.LaunchPad.Foundation.Assigned"))
                                       .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(type))),
                                    IdentifierName("Empty")
                                )
                            )
                           .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            IdentifierName(propertySymbol.Name)
                                        )
                                    )
                                )
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

        var applyChangesMethod = MethodDeclaration(ParseTypeName(targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)), "ApplyChanges")
                                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                .WithParameterList(
                                     ParameterList(
                                         SingletonSeparatedList(
                                             Parameter(Identifier("value")).WithType(
                                                 IdentifierName(targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
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
                                                                 IdentifierName(targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
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
                     List(
                         declaration.SyntaxTree.GetCompilationUnitRoot().Usings.AddRange(
                             namespaces
                                .Where(z => !string.IsNullOrWhiteSpace(z))
                                .Select(z => UsingDirective(ParseName(z)))
                         )
                     ),
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
        var type = GenericName(Identifier("Rocket.Surgery.LaunchPad.Foundation.Assigned"))
           .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(typeSyntax)));
        return new MemberDeclarationSyntax[]
        {
            PropertyDeclaration(type, Identifier(propertySymbol.Name))
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
                                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                            }
                        )
                    )
                ).WithInitializer(
                    EqualsValueClause(
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    type,
                                    IdentifierName("Empty")
                                )
                            )
                           .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            LiteralExpression(
                                                SyntaxKind.DefaultLiteralExpression,
                                                Token(SyntaxKind.DefaultKeyword)
                                            )
                                        )
                                    )
                                )
                            )
                    )
                ).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
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
                                         z => z.Type is GenericNameSyntax qns && qns.Identifier.Text.EndsWith("IPropertyTracking", StringComparison.Ordinal)
                                     ),
                                 static (syntaxContext, token) => (
                                     syntax: (TypeDeclarationSyntax)syntaxContext.Node, semanticModel: syntaxContext.SemanticModel,
                                     symbol: syntaxContext.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)syntaxContext.Node, token)!
                                 )
                             )
                            .Select(
                                 (tuple, token) =>
                                 {
                                     var interfaceSymbol = tuple.symbol
                                                                .Interfaces.FirstOrDefault(
                                                                     z => z.Name.StartsWith("IPropertyTracking", StringComparison.Ordinal)
                                                                 );
                                     var targetSymbol = interfaceSymbol?.ContainingAssembly.Name == "Rocket.Surgery.LaunchPad.Foundation"
                                         ? (INamedTypeSymbol?)interfaceSymbol.TypeArguments[0]
                                         : null;
                                     return (
                                         tuple.symbol,
                                         tuple.syntax,
                                         tuple.semanticModel,
                                         interfaceSymbol,
                                         targetSymbol
                                     );
                                 }
                             )
                            .Where(x => x.symbol is not null && x.targetSymbol is not null);

        context.RegisterSourceOutput(
            values,
            static (productionContext, tuple) => GeneratePropertyTracking(productionContext, tuple.syntax, tuple.symbol, tuple.targetSymbol!)
        );
    }
}
