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

        var inheritedMembers = targetSymbol
                              .GetAttributes()
                              .Where(z => z.AttributeClass?.Name is "InheritFromAttribute")
                              .Select(
                                   attribute => InheritFromGenerator.GetInheritingSymbol(context, attribute, symbol.Name) is not { } inheritFromSymbol
                                       ? ImmutableArray<ISymbol>.Empty
                                       : InheritFromGenerator.GetInheritableMemberSymbols(attribute, inheritFromSymbol)
                               )
                              .Aggregate(ImmutableArray<ISymbol>.Empty, (a, b) => a.AddRange(b));

        var writeableProperties =
            targetSymbol
               .GetMembers()
               .Concat(inheritedMembers)
               .OfType<IPropertySymbol>()
                // only works for `set`able properties not init only
               .Where(z => !symbol.GetMembers(z.Name).Any())
               .Where(z => z is { IsStatic: false, IsIndexer: false, IsReadOnly: false, })
               .ToArray();
        if (!targetSymbol.IsRecord)
        {
            // not able to use with operator, so ignore any init only properties.
            writeableProperties = writeableProperties.Where(z => z is { SetMethod.IsInitOnly: false, GetMethod.IsReadOnly: false, }).ToArray();
        }

        var changesRecord = RecordDeclaration(Token(SyntaxKind.RecordKeyword), "Changes")
                           .WithModifiers(SyntaxTokenList.Create(Token(SyntaxKind.PublicKeyword)))
                           .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                           .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
                           .WithLeadingTrivia(
                                Trivia(
                                    PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)
                                       .WithErrorCodes(SingletonSeparatedList<ExpressionSyntax>(IdentifierName("CA1034")))
                                )
                            );

        var getChangedStateMethodInitializer = InitializerExpression(SyntaxKind.ObjectInitializerExpression);
        var applyChangesBody = Block();
        var resetChangesBody = Block();
        var createMethodInitializer = InitializerExpression(SyntaxKind.ObjectInitializerExpression);
        var namespaces = new HashSet<string>();

        foreach (var propertySymbol in writeableProperties)
        {
            var type = ParseTypeName(propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            addNamespacesFromPropertyType(namespaces, propertySymbol.Type);

            var assignedType = GenericName(Identifier("Rocket.Surgery.LaunchPad.Foundation.Assigned"))
               .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(type)));

            classToInherit = classToInherit.AddMembers(GenerateTrackingProperties(propertySymbol, assignedType));
            changesRecord = changesRecord.AddMembers(
                PropertyDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier(propertySymbol.Name))
                   .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                   .WithAccessorList(
                        AccessorList(
                            List(
                                new[]
                                {
                                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                    AccessorDeclaration(SyntaxKind.InitAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
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
            createMethodInitializer = createMethodInitializer.AddExpressions(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(propertySymbol.Name),
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                assignedType,
                                IdentifierName("Empty")
                            )
                        )
                       .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("value"),
                                            IdentifierName(propertySymbol.Name)
                                        )
                                    )
                                )
                            )
                        )
                )
            );
            applyChangesBody = applyChangesBody.AddStatements(
                GenerateApplyChangesBodyPart(propertySymbol, IdentifierName("state"), isRecord)
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
                    ParseTypeName("Changes"),
                    Identifier("GetChangedState")
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
                                             Parameter(Identifier("state"))
                                                .WithType(
                                                     IdentifierName(targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                                                 )
                                         )
                                     )
                                 )
                                .WithBody(
                                     applyChangesBody.AddStatements(
                                         ExpressionStatement(InvocationExpression(IdentifierName("ResetChanges"))),
                                         ReturnStatement(IdentifierName("state"))
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


        var memberNamesSet = symbol.MemberNames.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        var constructor = symbol
                         .Constructors
                         .Where(z => !z.Parameters.Any(x => SymbolEqualityComparer.Default.Equals(x.Type, targetSymbol)))
                         .Where(z => !z.IsImplicitlyDeclared)
                         .OrderByDescending(z => z.Parameters.Length)
                         .FirstOrDefault();

        var createArgumentList = constructor is null
            ? ArgumentList()
            : ArgumentList(
                SeparatedList(
                    constructor.Parameters
                               .Select(
                                    z => Argument(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("value"),
                                            IdentifierName(memberNamesSet.TryGetValue(z.Name, out var name) ? name : z.Name)
                                        )
                                    )
                                )
                )
            );

        var createMethod = MethodDeclaration(
                               ParseTypeName(symbol.ToDisplayString(NullableFlowState.NotNull, SymbolDisplayFormat.FullyQualifiedFormat)),
                               "Create"
                           )
                          .WithParameterList(
                               ParameterList(
                                   SingletonSeparatedList(
                                       Parameter(Identifier("value"))
                                          .WithType(
                                               ParseTypeName(targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                                           )
                                   )
                               )
                           )
                          .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                          .WithExpressionBody(
                               ArrowExpressionClause(
                                   ObjectCreationExpression(
                                       IdentifierName(Identifier(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))),
                                       createArgumentList,
                                       createMethodInitializer
                                   )
                               )
                           )
                          .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        classToInherit = classToInherit
           .AddMembers(
                changesRecord,
                getChangedStateMethod,
                applyChangesMethod,
                resetChangesMethod,
                resetChangesImplementation,
                createMethod
            );

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
                .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
                .WithTrailingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)), CarriageReturnLineFeed);

        context.AddSource(
            $"{Path.GetFileNameWithoutExtension(declaration.SyntaxTree.FilePath)}_{declaration.Identifier.Text}",
            cu.NormalizeWhitespace().GetText(Encoding.UTF8)
        );
        return;

        static void addNamespacesFromPropertyType(HashSet<string> namespaces, ITypeSymbol symbol)
        {
            namespaces.Add(symbol.ContainingNamespace.GetFullMetadataName());
            if (symbol is not INamedTypeSymbol { IsGenericType: true } namedTypeSymbol) return;
            foreach (var genericType in namedTypeSymbol.TypeArguments)
            {
                addNamespacesFromPropertyType(namespaces, genericType);
            }
        }
    }

    private static StatementSyntax GenerateApplyChangesBodyPart(
        IPropertySymbol propertySymbol,
        IdentifierNameSyntax valueIdentifier,
        bool isRecord
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
                                                PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression, IdentifierName(propertySymbol.Name))
                                            )
                                        )
                                    )
                                )
                            )
                            : AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, valueIdentifier, IdentifierName(propertySymbol.Name)),
                                PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression, IdentifierName(propertySymbol.Name))
                            )
                    )
                )
            )
        );
    }

    private static MemberDeclarationSyntax[] GenerateTrackingProperties(IPropertySymbol propertySymbol, GenericNameSyntax assignedType)
    {
        return new MemberDeclarationSyntax[]
        {
            PropertyDeclaration(assignedType, Identifier(propertySymbol.Name))
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
               .WithInitializer(
                    EqualsValueClause(
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    assignedType,
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
                )
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
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
                                 z => z.Type is GenericNameSyntax qns && qns.Identifier.Text.EndsWith("IPropertyTracking", StringComparison.Ordinal)
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
                    .Where(x => x.symbol is { } && x.targetSymbol is { });

        context.RegisterSourceOutput(
            values,
            // ReSharper disable once NullableWarningSuppressionIsUsed
            static (productionContext, tuple) => GeneratePropertyTracking(productionContext, tuple.syntax, tuple.symbol, tuple.targetSymbol!)
        );
    }
}
