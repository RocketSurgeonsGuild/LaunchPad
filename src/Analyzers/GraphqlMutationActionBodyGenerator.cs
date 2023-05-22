using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.LaunchPad.Analyzers.Composition;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     Generator to automatically implement partial methods for controllers to call mediatr methods
/// </summary>
[Generator]
public class GraphqlMutationActionBodyGenerator : IIncrementalGenerator
{
    /// <summary>
    ///     Same as Pascalize except that the first character is lower case
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string Camelize(string input)
    {
        var word = Pascalize(input);
#pragma warning disable CA1308
        return word.Length > 0 ? string.Concat(word.Substring(0, 1).ToLower(CultureInfo.InvariantCulture), word.Substring(1)) : word;
#pragma warning restore CA1308
    }

    /// <summary>
    ///     By default, pascalize converts strings to UpperCamelCase also removing underscores
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string Pascalize(string input)
    {
        return Regex.Replace(input, "(?:^|_| +)(.)", match => match.Groups[1].Value.ToUpper(CultureInfo.InvariantCulture));
    }

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
        var isUnitResult = symbol.ReturnType is INamedTypeSymbol { Arity: 1 } nts && nts.TypeArguments[0].MetadataName == "ActionResult";
        var isStream = symbol.ReturnType.MetadataName == "IAsyncEnumerable`1";

        var newSyntax = syntax
                       .WithParameterList(
                            syntax.ParameterList.RemoveNodes(
                                syntax.ParameterList.Parameters.SelectMany(z => z.AttributeLists), SyntaxRemoveOptions.KeepNoTrivia
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
        var mediatorParameter = otherParams.FirstOrDefault(parameter => SymbolEqualityComparer.Default.Equals(mediator, parameter.Type));
        var claimsPrincipalParameter = otherParams.FirstOrDefault(parameter => SymbolEqualityComparer.Default.Equals(claimsPrincipal, parameter.Type));
        var cancellationTokenParameter = otherParams.FirstOrDefault(parameter => SymbolEqualityComparer.Default.Equals(cancellationToken, parameter.Type));
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

        var claimsPrincipalProperty = parameterType
                                     .GetMembers()
                                     .FirstOrDefault(
                                          parameter => parameter switch
                                          {
                                              IPropertySymbol { Type: not null, IsImplicitlyDeclared: false } ps => SymbolEqualityComparer.Default.Equals(
                                                  claimsPrincipal, ps.Type
                                              ),
                                              IFieldSymbol { Type: not null, IsImplicitlyDeclared: false } fs => SymbolEqualityComparer.Default.Equals(
                                                  claimsPrincipal, fs.Type
                                              ),
                                              _ => false
                                          }
                                      );
        var hasClaimsPrincipal = claimsPrincipalProperty is not null;
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


        var sendRequestExpression = isStream
            ? streamMediatorRequest(IdentifierName(parameter.Name), cancellationTokenParameter)
            : sendMediatorRequest(IdentifierName(parameter.Name), cancellationTokenParameter);


        if (parameterType.IsRecord)
        {
            var expressions = new List<ExpressionSyntax>();
            if (hasClaimsPrincipal)
            {
                expressions.Add(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(claimsPrincipalProperty!.Name),
                        IdentifierName(claimsPrincipalParameter!.Name)
                    )
                );
            }

            if (cancellationTokenParameter is { })
            {
                expressions.Add(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(claimsPrincipalProperty!.Name),
                        IdentifierName(claimsPrincipalParameter!.Name)
                    )
                );
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
        ((ClassDeclarationSyntax syntax, INamedTypeSymbol symbol, SemanticModel semanticModel) Left, Compilation Right) valueTuple
    )
    {
        var (syntax, symbol, semanticModel) = valueTuple.Left;
        var claimsPrincipal = semanticModel.Compilation.GetTypeByMetadataName("System.Security.Claims.ClaimsPrincipal")!;
        var mediator = semanticModel.Compilation.GetTypeByMetadataName("MediatR.IMediator")!;
        var cancellationToken = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken")!;
        var compilation = valueTuple.Right;
        var matchers = MatcherDefaults.GetMatchers(compilation);
        var members = syntax.Members
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

        var newClass = syntax.WithMembers(List<MemberDeclarationSyntax>())
                             .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                             .WithAttributeLists(List<AttributeListSyntax>())
                             .WithBaseList(null)
            ;


        foreach (var (method, methodSymbol, request) in members)
        {
            if (request != null)
            {
                var methodBody = GenerateMethod(context, mediator, claimsPrincipal, cancellationToken, method, methodSymbol, request);
                if (methodBody is null) continue;
                newClass = newClass.AddMembers(methodBody);
            }
        }

        var additionalUsings = new[] { "MediatR" };

        var usings = syntax.SyntaxTree.GetCompilationUnitRoot().Usings;
        foreach (var additionalUsing in additionalUsings)
        {
            if (usings.Any(z => z.Name.ToString() == additionalUsing)) continue;
            usings = usings.Add(UsingDirective(ParseName(additionalUsing)));
        }

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
                                 (syntaxContext, _) =>
                                 (
                                     syntax: (ClassDeclarationSyntax)syntaxContext.Node,
                                     symbol: syntaxContext.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)syntaxContext.Node, _)!,
                                     semanticModel: syntaxContext.SemanticModel
                                 )
                             )
                            .Where(z => z.symbol is { })
            ;

        context.RegisterSourceOutput(syntaxProvider.Combine(context.CompilationProvider), GenerateMethods);
    }
}
