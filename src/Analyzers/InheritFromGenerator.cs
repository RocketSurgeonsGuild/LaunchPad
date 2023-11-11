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
public class InheritFromGenerator : IIncrementalGenerator
{
    private static void GenerateInheritance(
        SourceProductionContext context,
        Compilation compilation,
        TypeDeclarationSyntax declaration,
        INamedTypeSymbol symbol,
        AttributeData[] attributes
    )
    {
        if (!declaration.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(GeneratorDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), declaration.GetFullMetadataName())
            );
            return;
        }

        var classToInherit = declaration
                            .WithMembers(List<MemberDeclarationSyntax>())
                            .WithAttributeLists(List<AttributeListSyntax>())
                            .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                            .WithBaseList(null)
                            .WithAttributeLists(
                                 SingletonList(
                                     AttributeList(
                                         SingletonSeparatedList(Attribute(ParseName("System.Runtime.CompilerServices.CompilerGenerated")))
                                     )
                                 )
                             );

        foreach (var attribute in attributes)
        {
            if (attribute.ApplicationSyntaxReference?.GetSyntax() is not { } attributeSyntax)
                continue;
            if (attribute is { ConstructorArguments: { Length: 0 } } || attribute.ConstructorArguments[0] is { Kind: not TypedConstantKind.Type })
            {
                // will be a normal compiler error
                continue;
            }

            if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol inheritFromSymbol)
            {
                // will be a normal compiler error
                continue;
            }

            if (inheritFromSymbol is { DeclaringSyntaxReferences: { Length: 0 } })
            {
                // TODO: Support generation from another assembly
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.TypeMustLiveInSameProject,
                        attributeSyntax.GetLocation(),
                        inheritFromSymbol.Name,
                        declaration.Identifier.Text
                    )
                );
                continue;
            }

            var members = new List<MemberDeclarationSyntax>();
            foreach (var type in inheritFromSymbol.DeclaringSyntaxReferences.Select(z => z.GetSyntax()).OfType<TypeDeclarationSyntax>())
            {
                members.AddRange(type.Members);
                classToInherit = classToInherit.AddAttributeLists(type.AttributeLists.ToArray());
                foreach (var item in type.BaseList?.Types ?? SeparatedList<BaseTypeSyntax>())
                {
                    if (declaration.BaseList?.Types.Any(z => z.IsEquivalentTo(item)) != true)
                    {
                        // ReSharper disable once NullableWarningSuppressionIsUsed
                        classToInherit = ( classToInherit.AddBaseListTypes(item) as TypeDeclarationSyntax )!;
                    }
                }
            }

            if (!compilation.HasImplicitConversion(symbol, inheritFromSymbol))
            {
                classToInherit = classToInherit.AddMembers(members.ToArray());
            }

            if (classToInherit is ClassDeclarationSyntax classDeclarationSyntax)
            {
                classToInherit = AddWithMethod(
                    declaration,
                    classDeclarationSyntax,
                    members,
                    inheritFromSymbol
                );
            }

            if (classToInherit is RecordDeclarationSyntax recordDeclarationSyntax)
            {
                classToInherit = AddWithMethod(
                    recordDeclarationSyntax,
                    members,
                    inheritFromSymbol
                );
            }
        }

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

    private static TypeDeclarationSyntax AddWithMethod(
        TypeDeclarationSyntax sourceSyntax,
        ClassDeclarationSyntax syntax,
        List<MemberDeclarationSyntax> members,
        ITypeSymbol inheritFromSymbol
    )
    {
        var sourceAssignmentMembers = sourceSyntax.Members
                                                  .OfType<PropertyDeclarationSyntax>()
                                                  .Select(
                                                       m => AssignmentExpression(
                                                           SyntaxKind.SimpleAssignmentExpression,
                                                           IdentifierName(m.Identifier.Text),
                                                           MemberAccessExpression(
                                                               SyntaxKind.SimpleMemberAccessExpression,
                                                               ThisExpression(),
                                                               IdentifierName(m.Identifier.Text)
                                                           )
                                                       )
                                                   )
                                                  .Concat(
                                                       sourceSyntax.Members.OfType<FieldDeclarationSyntax>()
                                                                   .SelectMany(
                                                                        m => m.Declaration.Variables.Select(
                                                                            z => AssignmentExpression(
                                                                                SyntaxKind.SimpleAssignmentExpression,
                                                                                IdentifierName(z.Identifier.Text),
                                                                                MemberAccessExpression(
                                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                                    ThisExpression(),
                                                                                    IdentifierName(z.Identifier.Text)
                                                                                )
                                                                            )
                                                                        )
                                                                    )
                                                   )
                                                  .Cast<ExpressionSyntax>()
                                                  .ToArray();

        var valueAssignmentMembers = members
                                    .OfType<PropertyDeclarationSyntax>()
                                    .Select(
                                         m => AssignmentExpression(
                                             SyntaxKind.SimpleAssignmentExpression,
                                             IdentifierName(m.Identifier.Text),
                                             MemberAccessExpression(
                                                 SyntaxKind.SimpleMemberAccessExpression,
                                                 IdentifierName("value"),
                                                 IdentifierName(m.Identifier.Text)
                                             )
                                         )
                                     )
                                    .Concat(
                                         members.OfType<FieldDeclarationSyntax>()
                                                .SelectMany(
                                                     m => m.Declaration.Variables.Select(
                                                         z => AssignmentExpression(
                                                             SyntaxKind.SimpleAssignmentExpression,
                                                             IdentifierName(z.Identifier.Text),
                                                             MemberAccessExpression(
                                                                 SyntaxKind.SimpleMemberAccessExpression,
                                                                 IdentifierName("value"),
                                                                 IdentifierName(z.Identifier.Text)
                                                             )
                                                         )
                                                     )
                                                 )
                                     )
                                    .Cast<ExpressionSyntax>()
                                    .ToArray();


        return syntax.AddMembers(
            MethodDeclaration(IdentifierName(sourceSyntax.Identifier.Text), Identifier("With"))
               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
               .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(
                            Parameter(Identifier("value")).WithType(IdentifierName(inheritFromSymbol.Name))
                        )
                    )
                )
               .WithExpressionBody(
                    ArrowExpressionClause(
                        ObjectCreationExpression(IdentifierName(sourceSyntax.Identifier.Text))
                           .WithInitializer(
                                InitializerExpression(SyntaxKind.ObjectInitializerExpression, SeparatedList<ExpressionSyntax>())
                                   .AddExpressions(sourceAssignmentMembers)
                                   .AddExpressions(valueAssignmentMembers)
                            )
                    )
                )
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
        );
    }

    private static TypeDeclarationSyntax AddWithMethod(
        RecordDeclarationSyntax syntax,
        List<MemberDeclarationSyntax> members,
        ITypeSymbol inheritFromSymbol
    )
    {
        var valueAssignmentMembers = members
                                    .OfType<PropertyDeclarationSyntax>()
                                    .Select(
                                         m => AssignmentExpression(
                                             SyntaxKind.SimpleAssignmentExpression,
                                             IdentifierName(m.Identifier.Text),
                                             MemberAccessExpression(
                                                 SyntaxKind.SimpleMemberAccessExpression,
                                                 IdentifierName("value"),
                                                 IdentifierName(m.Identifier.Text)
                                             )
                                         )
                                     )
                                    .Concat(
                                         members.OfType<FieldDeclarationSyntax>()
                                                .SelectMany(
                                                     m => m.Declaration.Variables.Select(
                                                         z => AssignmentExpression(
                                                             SyntaxKind.SimpleAssignmentExpression,
                                                             IdentifierName(z.Identifier.Text),
                                                             MemberAccessExpression(
                                                                 SyntaxKind.SimpleMemberAccessExpression,
                                                                 IdentifierName("value"),
                                                                 IdentifierName(z.Identifier.Text)
                                                             )
                                                         )
                                                     )
                                                 )
                                     )
                                    .Cast<ExpressionSyntax>()
                                    .ToArray();


        return syntax.AddMembers(
            MethodDeclaration(IdentifierName(syntax.Identifier.Text), Identifier("With"))
               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
               .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(
                            Parameter(Identifier("value")).WithType(IdentifierName(inheritFromSymbol.Name))
                        )
                    )
                )
               .WithExpressionBody(
                    ArrowExpressionClause(
                        WithExpression(
                            ThisExpression(),
                            InitializerExpression(
                                SyntaxKind.WithInitializerExpression,
                                SeparatedList(valueAssignmentMembers)
                            )
                        )
                    )
                )
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
        );
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if ROSLYN4_4
        var values = context.SyntaxProvider.ForAttributeWithMetadataName(
                                 "Rocket.Surgery.LaunchPad.Foundation.InheritFromAttribute",
                                 (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                                 (syntaxContext, _) => syntaxContext
                             )
                            .Combine(context.CompilationProvider)
                            .Select(
                                 static (tuple, _) => (
                                     syntax: (TypeDeclarationSyntax)tuple.Left.TargetNode,
                                     semanticModel: tuple.Left.SemanticModel,
                                     symbol: (INamedTypeSymbol)tuple.Left.TargetSymbol,
                                     compilation: tuple.Right,
                                     attributes: tuple.Left.Attributes
                                                      .Where(z => z.AttributeClass?.Name == "InheritFromAttribute")
                                                      .ToArray()
                                 )
                             );
#else
        var values = context.SyntaxProvider
                            .CreateSyntaxProvider(
                                 static (node, _) =>
                                     node is (ClassDeclarationSyntax or RecordDeclarationSyntax) and TypeDeclarationSyntax
                                     {
                                         AttributeLists: { Count: > 0 }
                                     } recordDeclarationSyntax && recordDeclarationSyntax.AttributeLists.ContainsAttribute("InheritFrom"),
                                 static (syntaxContext, token) => (
                                     syntax: (TypeDeclarationSyntax)syntaxContext.Node, semanticModel: syntaxContext.SemanticModel,
                                     // ReSharper disable once NullableWarningSuppressionIsUsed
                                     symbol: syntaxContext.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)syntaxContext.Node, token)! )
                             ).Combine(
                                 context.CompilationProvider
                                        .Select(
                                             static (z, _) => (
                                                 compilation: z,
                                                 // ReSharper disable once NullableWarningSuppressionIsUsed
                                                 inheritFromAttribute: z.GetTypeByMetadataName("Rocket.Surgery.LaunchPad.Foundation.InheritFromAttribute")! )
                                         )
                             )
                            .Select(
                                 static (tuple, _) => (
                                     tuple.Left.syntax,
                                     tuple.Left.semanticModel,
                                     tuple.Left.symbol,
                                     tuple.Right.compilation,
                                     attributes: tuple.Left.symbol?.GetAttributes()
                                                      .Where(z => SymbolEqualityComparer.Default.Equals(tuple.Right.inheritFromAttribute, z.AttributeClass))
                                                      .ToArray()
                                 )
                             )
                            .Where(x => !( x.symbol is null || x.attributes is null or { Length: 0 } ));
#endif

        context.RegisterSourceOutput(
            values,
            // ReSharper disable once NullableWarningSuppressionIsUsed
            static (productionContext, tuple) => GenerateInheritance(productionContext, tuple.compilation, tuple.syntax, tuple.symbol, tuple.attributes!)
        );
    }
}
