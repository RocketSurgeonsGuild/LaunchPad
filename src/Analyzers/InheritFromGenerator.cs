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
public class InheritFromGenerator : IIncrementalGenerator
{
    internal static INamedTypeSymbol? GetInheritingSymbol(SourceProductionContext context, AttributeData attribute, string otherSymbolName)
    {
        var inheritFromSymbol = attribute switch
                                {
                                    { AttributeClass.TypeArguments: [INamedTypeSymbol genericArgumentSymbol,], } => genericArgumentSymbol,
                                    { ConstructorArguments: [{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol constructorArgumentSymbol, },], } =>
                                        constructorArgumentSymbol,
                                    _ => null,
                                };
        switch (inheritFromSymbol)
        {
            case { DeclaringSyntaxReferences.Length: 0, }:
                // TODO: Support generation from another assembly
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.TypeMustLiveInSameProject,
                        attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                        inheritFromSymbol.Name,
                        otherSymbolName
                    )
                );
                return null;
        }

        return inheritFromSymbol;
    }

    internal static ImmutableArray<ISymbol> GetInheritableMemberSymbols(AttributeData attribute, INamedTypeSymbol inheritFromSymbol)
    {
        var excludeMembers = new HashSet<string>(
            attribute is { NamedArguments: [{ Key: "Exclude", Value: { Kind: TypedConstantKind.Array, Values: { Length: > 0, } values, }, },], }
                ? values.Select(z => (string)z.Value!).ToArray()
                : Array.Empty<string>()
        );

        return inheritFromSymbol
              .GetMembers()
              .Where(z => z is not INamedTypeSymbol)
              .Where(z => z is not IPropertySymbol property || !excludeMembers.Contains(property.Name))
              .ToImmutableArray();
    }

    private static void GenerateInheritance(
        SourceProductionContext context,
        Compilation compilation,
        TypeDeclarationSyntax declaration,
        INamedTypeSymbol targetSymbol,
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

        var isRecord = declaration is RecordDeclarationSyntax;

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

        foreach (var attribute in attributes)
        {
            var inheritFromSymbol = GetInheritingSymbol(context, attribute, classToInherit.Identifier.Text);
            if (inheritFromSymbol is null)
            {
                continue;
            }

            var inheritableMembers = GetInheritableMembers(attribute, inheritFromSymbol);

            classToInherit = AddInheritableMembers(
                classToInherit,
                inheritableMembers,
                compilation,
                declaration,
                targetSymbol,
                inheritFromSymbol
            );

            if (classToInherit is ClassDeclarationSyntax classDeclarationSyntax)
            {
                classToInherit = AddWithMethod(
                    classDeclarationSyntax,
                    declaration,
                    inheritableMembers,
                    inheritFromSymbol.Name
                );
            }

            if (classToInherit is RecordDeclarationSyntax recordDeclarationSyntax)
            {
                classToInherit = AddWithMethod(
                    recordDeclarationSyntax,
                    declaration,
                    inheritableMembers,
                    inheritFromSymbol.Name
                );
            }
        }

        var cu = CompilationUnit(
                     List<ExternAliasDirectiveSyntax>(),
                     List(declaration.SyntaxTree.GetCompilationUnitRoot().Usings),
                     List<AttributeListSyntax>(),
                     SingletonList<MemberDeclarationSyntax>(
                         targetSymbol.ContainingNamespace.IsGlobalNamespace
                             ? classToInherit.ReparentDeclaration(context, declaration)
                             : NamespaceDeclaration(ParseName(targetSymbol.ContainingNamespace.ToDisplayString()))
                                .WithMembers(SingletonList<MemberDeclarationSyntax>(classToInherit.ReparentDeclaration(context, declaration)))
                     )
                 )
                .WithLeadingTrivia()
                .WithTrailingTrivia()
                .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
                .WithTrailingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)), CarriageReturnLineFeed);

        context.AddSource(
            $"{string.Join("_", declaration.GetParentDeclarationsWithSelf().Reverse().Select(z => z.Identifier.Text))}_InheritFrom",
            cu.NormalizeWhitespace().GetText(Encoding.UTF8)
        );
    }

    internal static INamedTypeSymbol? GetInheritingSymbol(SourceProductionContext context, AttributeData attribute, string otherSymbolName)
    {
        var inheritFromSymbol = attribute switch
                                {
                                    { AttributeClass.TypeArguments: [INamedTypeSymbol genericArgumentSymbol,], } => genericArgumentSymbol,
                                    { ConstructorArguments: [{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol constructorArgumentSymbol, },], } =>
                                        constructorArgumentSymbol,
                                    _ => null
                                };
        switch (inheritFromSymbol)
        {
            case { DeclaringSyntaxReferences.Length: 0, }:
                // TODO: Support generation from another assembly
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.TypeMustLiveInSameProject,
                        attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
                        inheritFromSymbol.Name,
                        otherSymbolName
                    )
                );
                return null;
        }

        return inheritFromSymbol;
    }

    private static ImmutableArray<MemberDeclarationSyntax> GetInheritableMembers(
        AttributeData attribute,
        INamedTypeSymbol inheritFromSymbol
    )
    {
        var excludeMembers = new HashSet<string>(
            attribute is { NamedArguments: [{ Key: "Exclude", Value: { Kind: TypedConstantKind.Array, Values: { Length: > 0, } values, }, },], }
                ? values.Select(z => (string)z.Value!).ToArray()
                : Array.Empty<string>()
        );

        return inheritFromSymbol
              .DeclaringSyntaxReferences.Select(z => z.GetSyntax())
              .OfType<TypeDeclarationSyntax>()
              .SelectMany(
                   type =>
                       type
                          .Members
                          .Where(z => z is not TypeDeclarationSyntax)
                          .Where(
                               z => z is not PropertyDeclarationSyntax propertyDeclarationSyntax
                                || !excludeMembers.Contains(propertyDeclarationSyntax.Identifier.ToString())
                           )
               )
              .ToImmutableArray();
    }

    internal static ImmutableArray<ISymbol> GetInheritableMemberSymbols(AttributeData attribute, INamedTypeSymbol inheritFromSymbol)
    {
        var excludeMembers = new HashSet<string>(
            attribute is { NamedArguments: [{ Key: "Exclude", Value: { Kind: TypedConstantKind.Array, Values: { Length: > 0, } values, }, },], }
                ? values.Select(z => (string)z.Value!).ToArray()
                : Array.Empty<string>()
        );

        return inheritFromSymbol
              .GetMembers()
              .Where(z => z is not INamedTypeSymbol)
              .Where(z => z is not IPropertySymbol property || !excludeMembers.Contains(property.Name))
              .ToImmutableArray();
    }

    private static TypeDeclarationSyntax AddInheritableMembers(
        TypeDeclarationSyntax classToInherit,
        ImmutableArray<MemberDeclarationSyntax> members,
        Compilation compilation,
        TypeDeclarationSyntax declaration,
        INamedTypeSymbol targetSymbol,
        INamedTypeSymbol inheritFromSymbol
    )
    {
        if (!compilation.HasImplicitConversion(targetSymbol, inheritFromSymbol))
        {
            classToInherit = classToInherit.AddMembers(members.ToArray());
        }

        return inheritFromSymbol
              .DeclaringSyntaxReferences.Select(z => z.GetSyntax())
              .OfType<TypeDeclarationSyntax>()
              .Aggregate(
                   classToInherit,
                   (current1, type) => ( type.BaseList?.Types ?? SeparatedList<BaseTypeSyntax>() )
                                      .Where(item => declaration.BaseList?.Types.Any(z => z.IsEquivalentTo(item)) != true)
                                      .Aggregate(
                                           // are attributes needed?
                                           // current1.AddAttributeLists(type.AttributeLists.ToArray())
                                           current1,
                                           (current, item) => ( current.AddBaseListTypes(item) as TypeDeclarationSyntax )!
                                       )
               );
    }

    private static TypeDeclarationSyntax AddWithMethod(
        ClassDeclarationSyntax classToInherit,
        TypeDeclarationSyntax declaration,
        ImmutableArray<MemberDeclarationSyntax> members,
        string inheritFromSymbolName
    )
    {
        var sourceAssignmentMembers = declaration.Members
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
                                          declaration
                                             .Members.OfType<FieldDeclarationSyntax>()
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
                                         classToInherit
                                            .Members
                                            .OfType<FieldDeclarationSyntax>()
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


        return classToInherit.AddMembers(
            MethodDeclaration(IdentifierName(declaration.Identifier.Text), Identifier("With"))
               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
               .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(
                            Parameter(Identifier("value")).WithType(IdentifierName(inheritFromSymbolName))
                        )
                    )
                )
               .WithExpressionBody(
                    ArrowExpressionClause(
                        ObjectCreationExpression(IdentifierName(declaration.Identifier.Text))
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
        TypeDeclarationSyntax declaration,
        ImmutableArray<MemberDeclarationSyntax> members,
        string inheritFromSymbolName
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
                                         declaration
                                            .Members
                                            .OfType<FieldDeclarationSyntax>()
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
                            Parameter(Identifier("value")).WithType(IdentifierName(inheritFromSymbolName))
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
        var values = context
                    .SyntaxProvider.ForAttributeWithMetadataName(
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
                             attributes: tuple
                                        .Left.Attributes
                                        .Where(z => z.AttributeClass?.Name is "InheritFromAttribute")
                                        .ToArray()
                         )
                     );
        var values2 = context
                     .SyntaxProvider.ForAttributeWithMetadataName(
                          "Rocket.Surgery.LaunchPad.Foundation.InheritFromAttribute`1",
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
                              attributes: tuple
                                         .Left.Attributes
                                         .Where(z => z.AttributeClass?.Name is "InheritFromAttribute")
                                         .ToArray()
                          )
                      );
        context.RegisterSourceOutput(
            values2,
            // ReSharper disable once NullableWarningSuppressionIsUsed
            static (productionContext, tuple) => GenerateInheritance(productionContext, tuple.compilation, tuple.syntax, tuple.symbol, tuple.attributes!)
        );
        #else
        var values = context
                    .SyntaxProvider
                    .CreateSyntaxProvider(
                         static (node, _) =>
                             node is (ClassDeclarationSyntax or RecordDeclarationSyntax)
                                 and TypeDeclarationSyntax
                                     {
                                         AttributeLists: { Count: > 0, },
                                     } recordDeclarationSyntax
                          && recordDeclarationSyntax.AttributeLists.ContainsAttribute("InheritFrom"),
                         static (syntaxContext, token) => (
                             syntax: (TypeDeclarationSyntax)syntaxContext.Node, semanticModel: syntaxContext.SemanticModel,
                             // ReSharper disable once NullableWarningSuppressionIsUsed
                             symbol: syntaxContext.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)syntaxContext.Node, token)! )
                     )
                    .Combine(
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
                             attributes: tuple
                                        .Left.symbol?.GetAttributes()
                                        .Where(z => SymbolEqualityComparer.Default.Equals(tuple.Right.inheritFromAttribute, z.AttributeClass))
                                        .ToArray()
                         )
                     )
                    .Where(x => !( x.symbol is null || x.attributes is null or { Length: 0, } ));
        #endif

        context.RegisterSourceOutput(
            values,
            // ReSharper disable once NullableWarningSuppressionIsUsed
            static (productionContext, tuple) => GenerateInheritance(productionContext, tuple.compilation, tuple.syntax, tuple.symbol, tuple.attributes!)
        );
    }
}
