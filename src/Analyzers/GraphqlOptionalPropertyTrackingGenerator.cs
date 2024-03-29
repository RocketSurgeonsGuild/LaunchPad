using System.Collections.Immutable;
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
                    GeneratorDiagnostics.ParameterMustBeSameTypeOfObject,
                    declaration.Keyword.GetLocation(),
                    declaration.GetFullMetadataName(),
                    declaration.Keyword.IsKind(SyntaxKind.ClassKeyword) ? "record" : "class"
                )
            );
            return;
        }

        var classToInherit =
            ( isRecord
                ? (TypeDeclarationSyntax)RecordDeclaration(Token(SyntaxKind.RecordKeyword), declaration.Identifier)
                : ClassDeclaration(declaration.Identifier) )
           .WithModifiers(TokenList(declaration.Modifiers.Select(z => z.WithoutTrivia())))
           .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
           .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
           .WithAttributeLists(
                SingletonList(
                    AttributeList(
                        SingletonSeparatedList(Attribute(ParseName("System.Runtime.CompilerServices.CompilerGenerated")))
                    )
                )
            );

        var writeableProperties =
            targetSymbol
               .GetMembers()
               .OfType<IPropertySymbol>()
               .Where(z => z is { IsStatic: false, IsIndexer: false, IsReadOnly: false, });
        if (!targetSymbol.IsRecord)
        {
            // not able to use with operator, so ignore any init only properties.
            writeableProperties = writeableProperties.Where(z => z is { SetMethod.IsInitOnly: false, GetMethod.IsReadOnly: false, });
        }


        writeableProperties = writeableProperties
                              // only works for `set`able properties not init only
                             .Where(z => !symbol.GetMembers(z.Name).Any())
                             .ToArray();
        var existingMembers = targetSymbol
                             .GetMembers()
                             .OfType<IPropertySymbol>()
                             .Where(z => z is { IsStatic: false, IsIndexer: false, IsReadOnly: false, })
                             .Where(z => symbol.GetMembers(z.Name).Any())
                             .Except(writeableProperties)
                             .ToArray();

        var memberNamesSet = targetSymbol.MemberNames.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

        var constructor = targetSymbol
                         .Constructors
                         .Where(z => !z.Parameters.Any(x => SymbolEqualityComparer.Default.Equals(x.Type, targetSymbol)))
                         .Where(z => !z.IsImplicitlyDeclared)
                         .OrderByDescending(z => z.Parameters.Length)
                         .FirstOrDefault();

        existingMembers = existingMembers
                         .Except(
                              existingMembers
                                 .Join(
                                      constructor?.Parameters ?? ImmutableArray<IParameterSymbol>.Empty,
                                      z => z.Name,
                                      z => z.Name,
                                      (a, _) => a,
                                      StringComparer.OrdinalIgnoreCase
                                  )
                          )
                         .ToArray();
        var createArgumentList = constructor is null
            ? ArgumentList()
            : ArgumentList(
                SeparatedList(constructor.Parameters.Select(z => Argument(IdentifierName(memberNamesSet.TryGetValue(z.Name, out var name) ? name : z.Name))))
            );

        var createBody = Block()
           .AddStatements(
                LocalDeclarationStatement(
                    VariableDeclaration(
                            IdentifierName(Identifier(TriviaList(), SyntaxKind.VarKeyword, "var", "var", TriviaList()))
                        )
                       .WithVariables(
                            SingletonSeparatedList(
                                VariableDeclarator(Identifier("value"))
                                   .WithInitializer(
                                        EqualsValueClause(
                                            ObjectCreationExpression(
                                                IdentifierName(Identifier(targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))),
                                                createArgumentList,
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
                Name: "Assigned", ContainingAssembly.Name: "Rocket.Surgery.LaunchPad.Foundation",
            } namedTypeSymbol
                ? namedTypeSymbol.TypeArguments[0]
                : propertySymbol.Type;
            type = ParseTypeName(propertyType.ToDisplayString(NullableFlowState.MaybeNull, SymbolDisplayFormat.MinimallyQualifiedFormat));
            if (propertyType is { TypeKind: TypeKind.Struct or TypeKind.Enum, } && type is not NullableTypeSyntax)
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

        // declaration.Members.OfType<ConstructorDeclarationSyntax>();
        // or primary consturctor list

        var createMethod = MethodDeclaration(
                               ParseTypeName(targetSymbol.ToDisplayString(NullableFlowState.NotNull, SymbolDisplayFormat.FullyQualifiedFormat)),
                               "Create"
                           )
                          .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                          .WithBody(
                               createBody.AddStatements(
                                   ReturnStatement(IdentifierName("value"))
                               )
                           );

        classToInherit = classToInherit.AddMembers(createMethod);
        var usings = declaration
                    .SyntaxTree.GetCompilationUnitRoot()
                    .Usings
                    .AddDistinctUsingStatements(namespaces.Where(z => !string.IsNullOrWhiteSpace(z)));

        var cu = CompilationUnit(
                     List<ExternAliasDirectiveSyntax>(),
                     List(usings),
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
            $"{Path.GetFileNameWithoutExtension(declaration.SyntaxTree.FilePath)}_{string.Join("_", declaration.GetParentDeclarationsWithSelf().Reverse().Select(z => z.Identifier.Text))}",
            cu.NormalizeWhitespace().GetText(Encoding.UTF8)
        );
    }

    private static StatementSyntax GenerateApplyChangesBodyPart(
        IPropertySymbol propertySymbol,
        IdentifierNameSyntax valueIdentifier,
        bool isRecord
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
                            propertySymbol,
                            valueIdentifier,
                            isRecord,
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
        IPropertySymbol propertySymbol,
        IdentifierNameSyntax valueIdentifier,
        bool isRecord,
        ExpressionSyntax valueExpression
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
                ),
        };
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var values = context
                    .SyntaxProvider
                    .CreateSyntaxProvider(
                         static (node, _) =>
                             node is (ClassDeclarationSyntax or RecordDeclarationSyntax)
                                 and TypeDeclarationSyntax
                                     {
                                         BaseList: { } baseList,
                                     }
                          && baseList.Types.Any(
                                 z => z.Type is GenericNameSyntax qns && qns.Identifier.Text.EndsWith("IOptionalTracking", StringComparison.Ordinal)
                             ),
                         static (syntaxContext, token) => (
                             syntax: (TypeDeclarationSyntax)syntaxContext.Node, semanticModel: syntaxContext.SemanticModel,
                             // ReSharper disable once NullableWarningSuppressionIsUsed
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
                    .Where(x => x.symbol is { } && x.targetSymbol is { });

        context.RegisterSourceOutput(
            values,
            // ReSharper disable once NullableWarningSuppressionIsUsed
            static (productionContext, tuple) => GeneratePropertyTracking(productionContext, tuple.syntax, tuple.symbol, tuple.targetSymbol!)
        );
    }
}