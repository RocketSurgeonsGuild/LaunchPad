using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     A generator that is used to copy properties, fields and methods from one type onto another.
/// </summary>
[Generator]
public class GraphqlOptionalPropertyTrackingGenerator : IIncrementalGenerator
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
                        .Where(z => z is { IsStatic: false, IsIndexer: false, IsReadOnly: false });
        if (!targetSymbol.IsRecord)
        {
            // not able to use with operator, so ignore any init only properties.
            writeableProperties = writeableProperties.Where(z => z is { SetMethod.IsInitOnly: false, GetMethod.IsReadOnly: false });
        }


        writeableProperties = writeableProperties
                              // only works for `set`able properties not init only
                             .Where(z => !symbol.GetMembers(z.Name).Any())
                             .ToArray();
        var existingMembers = targetSymbol.GetMembers()
                                          .OfType<IPropertySymbol>()
                                          .Where(z => z is { IsStatic: false, IsIndexer: false, IsReadOnly: false })
                                          .Where(z => symbol.GetMembers(z.Name).Any())
                                          .Except(writeableProperties);

        var createBody = Block()
           .AddStatements(
                LocalDeclarationStatement(
                    VariableDeclaration(
                        IdentifierName(Identifier(TriviaList(), SyntaxKind.VarKeyword, "var", "var", TriviaList()))
                    ).WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(Identifier("value"))
                               .WithInitializer(
                                    EqualsValueClause(
                                        ObjectCreationExpression(
                                                IdentifierName(Identifier(targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                                            )
                                           .WithInitializer(
                                                InitializerExpression(
                                                    SyntaxKind.ObjectInitializerExpression,
                                                    SeparatedList<ExpressionSyntax>(
                                                        existingMembers
                                                           .Select(
                                                                z =>
                                                                    AssignmentExpression(
                                                                        SyntaxKind.SimpleAssignmentExpression,
                                                                        IdentifierName(z.Name),
                                                                        IdentifierName(z.Name)
                                                                    )
                                                            )
                                                    )
                                                )
                                            )
                                    )
                                )
                        )
                    )
                )
            );

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
            TypeSyntax type;
            ITypeSymbol propertyType;
            propertyType = propertySymbol.Type is INamedTypeSymbol
            {
                Name: "Assigned", ContainingAssembly.Name: "Rocket.Surgery.LaunchPad.Foundation"
            } namedTypeSymbol
                ? namedTypeSymbol.TypeArguments[0]
                : propertySymbol.Type;
            type = ParseTypeName(propertyType.ToDisplayString(NullableFlowState.MaybeNull, SymbolDisplayFormat.MinimallyQualifiedFormat));
            if (propertyType is { TypeKind: TypeKind.Struct or TypeKind.Enum } && type is not NullableTypeSyntax)
            {
                type = NullableType(type);
            }

            AddNamespacesFromPropertyType(namespaces, propertyType);

//            classToInherit = classToInherit.AddMembers(GenerateTrackingProperties(propertySymbol, type));
            classToInherit = classToInherit.AddMembers(GenerateTrackingProperties(propertySymbol, type));
            createBody = createBody.AddStatements(
                GenerateApplyChangesBodyPart(propertySymbol, IdentifierName("value"), isRecord)
            );
        }

        var createMethod = MethodDeclaration(
                               ParseTypeName(targetSymbol.ToDisplayString(NullableFlowState.NotNull, SymbolDisplayFormat.FullyQualifiedFormat)), "Create"
                           )
                          .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                          .WithBody(
                               createBody.AddStatements(
                                   ReturnStatement(IdentifierName("value"))
                               )
                           );

        classToInherit = classToInherit.AddMembers(createMethod);

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
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(propertySymbol.Name),
                IdentifierName("HasValue")
            ),
            Block(
                SingletonList<StatementSyntax>(
                    ExpressionStatement(
                        GenerateAssignmentExpression(
                            propertySymbol, valueIdentifier, isRecord,
                            propertySymbol.NullableAnnotation == NullableAnnotation.NotAnnotated && propertySymbol.Type.TypeKind == TypeKind.Struct
                                ? BinaryExpression(
                                    SyntaxKind.CoalesceExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(propertySymbol.Name),
                                        IdentifierName("Value")
                                    ),
                                    LiteralExpression(
                                        SyntaxKind.DefaultLiteralExpression,
                                        Token(SyntaxKind.DefaultKeyword)
                                    )
                                )
                                : MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(propertySymbol.Name),
                                    IdentifierName("Value")
                                )
                        )
                    )
                )
            )
        );
    }

    private static AssignmentExpressionSyntax GenerateAssignmentExpression(
        IPropertySymbol propertySymbol, IdentifierNameSyntax valueIdentifier, bool isRecord, ExpressionSyntax valueExpression
    )
    {
        return isRecord
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
                                valueExpression
                            )
                        )
                    )
                )
            )
            : AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, valueIdentifier, IdentifierName(propertySymbol.Name)),
                valueExpression
            );
    }

    private static MemberDeclarationSyntax[] GenerateTrackingProperties(IPropertySymbol propertySymbol, TypeSyntax typeSyntax)
    {
        var type = GenericName(Identifier("HotChocolate.Optional"))
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
                                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
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
                                         z => z.Type is GenericNameSyntax qns && qns.Identifier.Text.EndsWith("IOptionalTracking", StringComparison.Ordinal)
                                     ),
                                 static (syntaxContext, token) => (
                                     syntax: (TypeDeclarationSyntax)syntaxContext.Node, semanticModel: syntaxContext.SemanticModel,
                                     symbol: syntaxContext.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)syntaxContext.Node, token)!
                                 )
                             )
                            .Select(
                                 (tuple, _) =>
                                 {
                                     var interfaceSymbol = tuple.symbol
                                                                .Interfaces.FirstOrDefault(
                                                                     z => z.Name.StartsWith("IOptionalTracking", StringComparison.Ordinal)
                                                                 );
                                     var targetSymbol = interfaceSymbol?.ContainingAssembly.Name == "Rocket.Surgery.LaunchPad.HotChocolate"
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
