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
                    targetSymbol.IsRecord ? "record" : "class"
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
           .WithAttributeLists(SingletonList(Helpers.CompilerGenerated));

        var targetMembers = targetSymbol.FilterProperties().ToImmutableArray();
        var excludedProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var inheritedMembers = InheritFromGenerator.GetInheritableMemberSymbols(targetSymbol, excludedProperties);
        var symbolMemberNames = symbol.MemberNames.Except(excludedProperties).ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        var targetSymbolMemberNames =
            targetSymbol.MemberNames.Except(excludedProperties).ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        var targetSymbolInheritedMemberNames =
            targetSymbolMemberNames
               .Concat(inheritedMembers.Select(z => z.Name))
               .Except(excludedProperties)
               .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

        targetMembers = targetMembers.AddRange(inheritedMembers);

        var writeableProperties = targetMembers
                                  // only works for `set`able properties not init only
                                 .Where(z => !symbolMemberNames.Contains(z.Name))
                                 .Where(z => targetSymbol.IsRecord || z is { SetMethod.IsInitOnly: false, GetMethod.IsReadOnly: false, })
                                 .ToArray();

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
        var constructor = symbol
                         .Constructors
                         .Where(z => !z.Parameters.Any(x => SymbolEqualityComparer.Default.Equals(x.Type, targetSymbol)))
                         .Where(z => !z.Parameters.Any(x => SymbolEqualityComparer.Default.Equals(x.Type, symbol)))
                         .OrderByDescending(z => z.Parameters.Length)
                         .FirstOrDefault();

        var existingMembers = targetSymbol
                             .FilterProperties()
                             .Where(z => symbolMemberNames.Contains(z.Name))
                             .Except(writeableProperties)
                             .ToArray();
        var missingMembers = symbol
                            .FilterProperties()
                            .Where(z => !targetSymbolMemberNames.Contains(z.Name))
                            .Except(writeableProperties)
                            .ToArray();

        var constructorParams = constructor?.Parameters.Select(z => z.Name).ToImmutableHashSet(StringComparer.OrdinalIgnoreCase)
         ?? ImmutableHashSet<string>.Empty;

        var constructorArguments = constructor
 ?
.Parameters
                                  .Select(
                                       param => Argument(
                                           targetSymbolInheritedMemberNames.TryGetValue(param.Name, out var name)
                                               ? MemberAccessExpression(
                                                   SyntaxKind.SimpleMemberAccessExpression,
                                                   IdentifierName("value"),
                                                   IdentifierName(name)
                                               )
                                               : IdentifierName(Helpers.Camelize(param.Name))
                                       )
                                   )
                                  .ToImmutableArray()
         ?? ImmutableArray<ArgumentSyntax>.Empty;

        var constructorParameters = constructor
 ?
.Parameters
                                   .Where(param => !missingMembers.Any(z => z.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase)))
                                   .Where(param => !targetSymbolInheritedMemberNames.Contains(param.Name))
                                   .Select(
                                        param => Parameter(Identifier(Helpers.Camelize(param.Name)))
                                           .WithType(ParseTypeName(param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                                    )
                                   .ToImmutableArray()
         ?? ImmutableArray<ParameterSyntax>.Empty;

        var createParameterList =
            ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier("value"))
                           .WithType(
                                ParseTypeName(targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                            )
                    )
                )
               .AddParameters(constructorParameters.ToArray())
               .AddParameters(
                    missingMembers
                       .Select(
                            z => Parameter(Identifier(Helpers.Camelize(z.Name)))
                               .WithType(ParseTypeName(z.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                        )
                       .ToArray()
                );
        var createArgumentList = ArgumentList(SeparatedList(constructorArguments));

        var createMethodInitializer = InitializerExpression(SyntaxKind.ObjectInitializerExpression)
                                     .AddExpressions(
                                          existingMembers
                                             .Where(z => !constructorParams.Contains(z.Name))
                                             .Select(
                                                  z =>
                                                      AssignmentExpression(
                                                          SyntaxKind.SimpleAssignmentExpression,
                                                          IdentifierName(z.Name),
                                                          MemberAccessExpression(
                                                              SyntaxKind.SimpleMemberAccessExpression,
                                                              IdentifierName("value"),
                                                              IdentifierName(z.Name)
                                                          )
                                                      )
                                              )
                                             .OfType<ExpressionSyntax>()
                                             .ToArray()
                                      )
                                     .AddExpressions(
                                          missingMembers
                                             .Where(z => !constructorParams.Contains(z.Name))
                                             .Select(
                                                  z =>
                                                      AssignmentExpression(
                                                          SyntaxKind.SimpleAssignmentExpression,
                                                          IdentifierName(z.Name),
                                                          IdentifierName(Helpers.Camelize(z.Name))
                                                      )
                                              )
                                             .OfType<ExpressionSyntax>()
                                             .ToArray()
                                      );

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


        var createMethod = MethodDeclaration(
                               ParseTypeName(symbol.ToDisplayString(NullableFlowState.NotNull, SymbolDisplayFormat.FullyQualifiedFormat)),
                               "TrackChanges"
                           )
                          .WithParameterList(createParameterList)
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

        classToInherit = classToInherit.WithMembers(List(classToInherit.Members.Select(z => z.WithAttributeLists(SingletonList(Helpers.CompilerAttributes)))));

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

        context.AddSourceRelativeTo(declaration, "PropertyTracking", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
        return;

        static void addNamespacesFromPropertyType(HashSet<string> namespaces, ITypeSymbol symbol)
        {
            namespaces.Add(symbol.ContainingNamespace.GetFullMetadataName());
            if (symbol is not INamedTypeSymbol { IsGenericType: true, } namedTypeSymbol) return;
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
                             var interfaceSymbol =
                                 tuple.symbol.Interfaces.FirstOrDefault(z => z.Name.StartsWith("IPropertyTracking", StringComparison.Ordinal));
                             if (interfaceSymbol is not
                                 {
                                     ContainingAssembly.Name: "Rocket.Surgery.LaunchPad.Foundation",
                                     TypeArguments: [INamedTypeSymbol targetSymbol,],
                                 }) return default;

                             return (
                                 tuple.symbol,
                                 tuple.syntax,
                                 tuple.semanticModel,
                                 interfaceSymbol,
                                 targetSymbol
                             );
                         }
                     )
                    .Where(x => x is { symbol: { }, targetSymbol: { }, });

        context.RegisterSourceOutput(
            values,
            // ReSharper disable once NullableWarningSuppressionIsUsed
            static (productionContext, tuple) => GeneratePropertyTracking(productionContext, tuple.syntax, tuple.symbol, tuple.targetSymbol!)
        );
    }
}

