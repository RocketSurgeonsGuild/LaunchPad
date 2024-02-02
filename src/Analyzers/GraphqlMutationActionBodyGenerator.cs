using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     Generator to automatically implement partial methods for controllers to call mediatr methods
/// </summary>
[Generator]
public class GraphqlMutationActionBodyGenerator : IIncrementalGenerator
{
    private static MethodDeclarationSyntax? GenerateMethod(
        SourceProductionContext context,
        INamedTypeSymbol mediator,
        INamedTypeSymbol claimsPrincipal,
        INamedTypeSymbol cancellationToken,
        MethodDeclarationSyntax syntax,
        IMethodSymbol symbol,
        IParameterSymbol parameter
    )
    {
        var otherParams = symbol.Parameters.Remove(parameter);
        var parameterType = (INamedTypeSymbol)parameter.Type;
        var isUnit = parameterType.AllInterfaces.Any(z => z.MetadataName == "IRequest");
        var isStream = symbol.ReturnType.MetadataName == "IAsyncEnumerable`1";

        var newSyntax = syntax
                       .WithParameterList(
                            // ReSharper disable once NullableWarningSuppressionIsUsed
                            syntax.ParameterList.RemoveNodes(
                                syntax.ParameterList.Parameters.SelectMany(z => z.AttributeLists),
                                SyntaxRemoveOptions.KeepNoTrivia
                            )!
                        )
                       .WithAttributeLists(List<AttributeListSyntax>())
            ;

        if (!isStream)
        {
            newSyntax = newSyntax
               .AddModifiers(Token(SyntaxKind.AsyncKeyword));
        }

        var block = Block();
        var resultName = parameter.Name == "result" ? "r" : "result";
        var mediatorParameter = otherParams.FirstOrDefault(param => SymbolEqualityComparer.Default.Equals(mediator, param.Type));
        var claimsPrincipalParameter = otherParams.FirstOrDefault(param => SymbolEqualityComparer.Default.Equals(claimsPrincipal, param.Type));
        var cancellationTokenParameter = otherParams.FirstOrDefault(param => SymbolEqualityComparer.Default.Equals(cancellationToken, param.Type));
        if (mediatorParameter is null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    GeneratorDiagnostics.ParameterMustExist,
                    parameter.Locations.First(),
                    parameter.Locations.Skip(1),
                    mediator,
                    parameterType.Name
                )
            );
        }

        var claimsPrincipalProperty =
            parameterType
               .GetMembers()
               .FirstOrDefault(
                    param => param switch
                             {
                                 IPropertySymbol { IsImplicitlyDeclared: false } ps => SymbolEqualityComparer.Default.Equals(claimsPrincipal, ps.Type),
                                 IFieldSymbol { IsImplicitlyDeclared: false } fs    => SymbolEqualityComparer.Default.Equals(claimsPrincipal, fs.Type),
                                 _                                                  => false
                             }
                );
        var hasClaimsPrincipal = claimsPrincipalProperty is { };
        if (hasClaimsPrincipal)
        {
            if (claimsPrincipalParameter is null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ParameterMustExist,
                        parameter.Locations.First(),
                        parameter.Locations.Skip(1),
                        claimsPrincipal,
                        parameterType.Name
                    )
                );
            }
        }

        if (( hasClaimsPrincipal && claimsPrincipalParameter is null ) || mediatorParameter is null)
        {
            return null;
        }

        if (mediatorParameter.GetAttribute("HotChocolate.ServiceAttribute") is null)
        {
            var node = newSyntax.ParameterList.Parameters.FirstOrDefault(z => z.Identifier.Text == mediatorParameter.Name)!;
            newSyntax = newSyntax.WithParameterList(
                newSyntax.ParameterList
                         .ReplaceNode(
                              node,
                              node.WithAttributeLists(
                                  SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("HotChocolate.ServiceAttribute")))))
                              )
                          )
            );
        }

        var sendRequestExpression = isStream
            ? streamMediatorRequest(IdentifierName(parameter.Name), cancellationTokenParameter)
            : sendMediatorRequest(IdentifierName(parameter.Name), cancellationTokenParameter);

        if (parameterType.IsRecord)
        {
            var expressions = new List<ExpressionSyntax>();
            if (hasClaimsPrincipal)
            {
                // ReSharper disable NullableWarningSuppressionIsUsed
                expressions.Add(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(claimsPrincipalProperty!.Name),
                        IdentifierName(claimsPrincipalParameter!.Name)
                    )
                );
                // ReSharper enable NullableWarningSuppressionIsUsed
            }

            if (expressions.Any())
            {
                var withExpression = WithExpression(
                    IdentifierName(parameter.Name),
                    InitializerExpression(SyntaxKind.WithInitializerExpression, SeparatedList(expressions))
                );
                sendRequestExpression = isStream
                    ? streamMediatorRequest(withExpression, cancellationTokenParameter)
                    : sendMediatorRequest(withExpression, cancellationTokenParameter);
            }
        }
        else
        {
            if (hasClaimsPrincipal)
            {
                block = block.AddStatements(
                    ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(parameter.Name),
                                IdentifierName(claimsPrincipalProperty!.Name)
                            ),
                            IdentifierName(claimsPrincipalParameter!.Name)
                        )
                    )
                );
            }
        }


        if (isUnit)
        {
            block = block.AddStatements(ExpressionStatement(sendRequestExpression));
        }
        else
        {
            block = block
               .AddStatements(
                    LocalDeclarationStatement(
                        VariableDeclaration(IdentifierName(Identifier(TriviaList(), SyntaxKind.VarKeyword, "var", "var", TriviaList())))
                           .WithVariables(
                                SingletonSeparatedList(VariableDeclarator(Identifier(resultName)).WithInitializer(EqualsValueClause(sendRequestExpression)))
                            )
                    )
                );
        }

        block = block.AddStatements(ReturnStatement(IdentifierName(resultName)));

        return newSyntax
              .WithBody(block.NormalizeWhitespace())
              .WithSemicolonToken(Token(SyntaxKind.None));

        static ExpressionSyntax sendMediatorRequest(ExpressionSyntax nameSyntax, IParameterSymbol? cancellationTokenParameter)
        {
            var arguments = new List<ArgumentSyntax> { Argument(nameSyntax) };
            if (cancellationTokenParameter is { })
            {
                arguments.Add(Argument(IdentifierName(cancellationTokenParameter.Name)));
            }

            return AwaitExpression(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("Mediator"),
                                        IdentifierName("Send")
                                    )
                                )
                               .WithArgumentList(ArgumentList(SeparatedList(arguments))),
                            IdentifierName("ConfigureAwait")
                        )
                    )
                   .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.FalseLiteralExpression
                                    )
                                )
                            )
                        )
                    )
            );
        }

        static ExpressionSyntax streamMediatorRequest(ExpressionSyntax nameSyntax, IParameterSymbol? cancellationTokenParameter)
        {
            var arguments = new List<ArgumentSyntax> { Argument(nameSyntax) };
            if (cancellationTokenParameter is { })
            {
                arguments.Add(Argument(IdentifierName(cancellationTokenParameter.Name)));
            }

            return InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Mediator"),
                        IdentifierName("CreateStream")
                    )
                )
               .WithArgumentList(ArgumentList(SeparatedList(arguments)));
        }
    }

    private void GenerateMethods(
        SourceProductionContext context,
        (ClassDeclarationSyntax syntax, INamedTypeSymbol symbol, SemanticModel semanticModel) valueTuple
    )
    {
        var (syntax, symbol, semanticModel) = valueTuple;
        var claimsPrincipal = semanticModel.Compilation.GetTypeByMetadataName("System.Security.Claims.ClaimsPrincipal")!;
        var mediator = semanticModel.Compilation.GetTypeByMetadataName("MediatR.IMediator")!;
        var cancellationToken = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken")!;
        var members = syntax
                     .Members
                     .OfType<MethodDeclarationSyntax>()
                     .Where(z => z.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
                     .Select(
                          method =>
                          {
                              var methodSymbol = semanticModel.GetDeclaredSymbol(method);
                              // ReSharper disable once UseNullPropagationWhenPossible
                              if (methodSymbol is null)
                              {
                                  context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.TypeMustLiveInSameProject, method.GetLocation()));
                                  return default;
                              }

                              var request = methodSymbol.Parameters.FirstOrDefault(p => p.Type.AllInterfaces.Any(i => i.MetadataName == "IRequest`1"))
                               ?? methodSymbol.Parameters.FirstOrDefault(p => p.Type.AllInterfaces.Any(i => i.MetadataName == "IRequest"));
                              var streamRequest = methodSymbol.Parameters.FirstOrDefault(
                                  p => p.Type.AllInterfaces.Any(i => i.MetadataName == "IStreamRequest`1")
                              );
                              return ( method, symbol: methodSymbol, request: request ?? streamRequest );
                          }
                      )
                     .Where(z => z is { symbol: { }, method: { } })
                     .ToImmutableArray();

        var newClass = syntax
                      .WithMembers(List<MemberDeclarationSyntax>())
                      .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                      .WithAttributeLists(List<AttributeListSyntax>())
                      .WithBaseList(null)
            ;


        foreach (var (method, methodSymbol, request) in members)
        {
            if (request != null)
            {
                var methodBody = GenerateMethod(
                    context,
                    mediator,
                    claimsPrincipal,
                    cancellationToken,
                    method,
                    methodSymbol,
                    request
                );
                if (methodBody is null) continue;
                newClass = newClass.AddMembers(methodBody);
            }
        }

        var additionalUsings = new[] { "MediatR" };

        var usings = syntax
                    .SyntaxTree.GetCompilationUnitRoot()
                    .Usings
                    .AddDistinctUsingStatements(additionalUsings.Where(z => !string.IsNullOrWhiteSpace(z)));

        var cu = CompilationUnit(
                     List<ExternAliasDirectiveSyntax>(),
                     List(usings),
                     List<AttributeListSyntax>(),
                     SingletonList<MemberDeclarationSyntax>(
                         symbol.ContainingNamespace.IsGlobalNamespace
                             ? newClass.ReparentDeclaration(context, syntax)
                             : NamespaceDeclaration(ParseName(symbol.ContainingNamespace.ToDisplayString()))
                                .WithMembers(SingletonList<MemberDeclarationSyntax>(newClass.ReparentDeclaration(context, syntax)))
                     )
                 )
                .WithLeadingTrivia()
                .WithTrailingTrivia()
                .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
                .WithTrailingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)), CarriageReturnLineFeed);

        context.AddSource($"{newClass.Identifier.Text}_Methods.cs", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxProvider = context
                            .SyntaxProvider
                            .CreateSyntaxProvider(
                                 (node, _) => node is ClassDeclarationSyntax cds
                                  && cds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))
                                  && cds.AttributeLists.ContainsAttribute("ExtendObjectType")
                                  && cds.Members.Any(
                                         z => z is MethodDeclarationSyntax && z.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))
                                     ),
                                 (syntaxContext, cancellationToken) =>
                                 (
                                     syntax: (ClassDeclarationSyntax)syntaxContext.Node,
                                     symbol: syntaxContext.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)syntaxContext.Node, cancellationToken)!,
                                     semanticModel: syntaxContext.SemanticModel
                                 )
                             )
                            .Where(z => z.symbol is { })
            ;

        context.RegisterSourceOutput(syntaxProvider, GenerateMethods);
    }
}
