using System.Collections.Immutable;
using System.Text;
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
public class ControllerActionBodyGenerator : IIncrementalGenerator
{
    private static MethodDeclarationSyntax? GenerateMethod(
        SourceProductionContext context,
        IPropertySymbol[] controllerBaseProperties,
        IRestfulApiMethodMatcher? matcher,
        IReadOnlyDictionary<RestfulApiMethod, int> statusCodeMap,
        MethodDeclarationSyntax syntax,
        IMethodSymbol symbol,
        IParameterSymbol parameter,
        ImmutableArray<(MethodDeclarationSyntax method, IMethodSymbol symbol, IRestfulApiMethodMatcher? matcher, IParameterSymbol? request)> members
    )
    {
        var otherParams = symbol.Parameters.Remove(parameter);
        var parameterType = (INamedTypeSymbol)parameter.Type;
        var isUnit = parameterType.AllInterfaces.Any(z => z.MetadataName == "IRequest");
        var isUnitResult = symbol.ReturnType is INamedTypeSymbol { Arity: 1, } nts && nts.TypeArguments[0].MetadataName == "ActionResult";
        var isStream = symbol.ReturnType.MetadataName == "IAsyncEnumerable`1";

        // ReSharper disable once NullableWarningSuppressionIsUsed
        var newSyntax = syntax
                       .WithParameterList(
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
        var sendRequestExpression = isStream ? streamMediatorRequest(IdentifierName(parameter.Name)) : sendMediatorRequest(IdentifierName(parameter.Name));

        if (otherParams.Length > 0)
        {
            var declaredParam = newSyntax.ParameterList.Parameters.Single(z => z.Identifier.Text == parameter.Name);
            var parameterProperties = parameterType
                                     .GetMembers()
                                     .Join(
                                          otherParams,
                                          z => z.Name,
                                          z => z.Name,
                                          static (member, parameter) => ( member, parameter ),
                                          StringComparer.OrdinalIgnoreCase
                                      )
                                     .ToArray();
            var controllerAttachedProperties = parameterType
                                              .GetMembers()
                                              .SelectMany(
                                                   par =>
                                                   {
                                                       return par switch
                                                              {
                                                                  IPropertySymbol { IsImplicitlyDeclared: false, } ps => controllerBaseProperties.Where(
                                                                      p => SymbolEqualityComparer.Default.Equals(p.Type, ps.Type)
                                                                  ),
                                                                  IFieldSymbol { IsImplicitlyDeclared: false, } fs => controllerBaseProperties.Where(
                                                                      p => SymbolEqualityComparer.Default.Equals(p.Type, fs.Type)
                                                                  ),
                                                                  _ => Enumerable.Empty<IPropertySymbol>(),
                                                              };
                                                   },
                                                   static (member, controllerProperty) => ( member, controllerProperty )
                                               )
                                              .ToArray();
            var failed = false;
            if (!parameterType.IsRecord)
            {
                foreach (( var member, _ ) in parameterProperties)
                {
                    if (member is not IPropertySymbol { SetMethod.IsInitOnly: true, }) continue;
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GeneratorDiagnostics.PropertyMustBeSettableForTheRequest,
                            member.Locations.First(),
                            member.Locations.Skip(1),
                            member.Name,
                            parameterType.Name
                        )
                    );
                    failed = true;
                }

                foreach (( var member, _ ) in controllerAttachedProperties)
                {
                    if (member is not IPropertySymbol { SetMethod.IsInitOnly: true, }) continue;
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GeneratorDiagnostics.PropertyMustBeSettableForTheRequest,
                            member.Locations.First(),
                            member.Locations.Skip(1),
                            member.Name,
                            parameterType.Name
                        )
                    );
                    failed = true;
                }
            }

            var mapping = parameterType.MemberNames.ToLookup(z => z, StringComparer.OrdinalIgnoreCase);
            foreach (var param in otherParams)
            {
                if (!mapping[param.Name].Any())
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GeneratorDiagnostics.ParameterMustBeAPropertyOfTheRequest,
                            param.Locations.First(),
                            param.Locations.Skip(1),
                            param.Name,
                            parameterType.Name
                        )
                    );
                    failed = true;
                }
                else if (parameterType.GetMembers(mapping[param.Name].First()).First() is IPropertySymbol property)
                {
                    if (!SymbolEqualityComparer.IncludeNullability.Equals(property.Type, param.Type))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                GeneratorDiagnostics.ParameterMustBeSameTypeAsTheRelatedProperty,
                                param.Locations.First(),
                                param.Locations.Skip(1),
                                param.Name,
                                param.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                                parameterType.Name,
                                property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
                            )
                        );
                        failed = true;
                    }
                }
            }

            if (failed)
            {
                return null;
            }

            var bindingMembers = parameterType
                                .GetMembers()
                                .Where(z => z is IPropertySymbol { IsImplicitlyDeclared: false, } ps && ps.Name != "EqualityContract")
                                .Select(z => z.Name)
                                .Except(parameterProperties.Select(z => z.member.Name))
                                .Except(controllerAttachedProperties.Select(z => z.member.Name))
                                .ToArray();

            var newParam = declaredParam.AddAttributeLists(
                AttributeList(
                    SeparatedList(
                        new[]
                        {
                            Attribute(
                                IdentifierName("Bind"),
                                AttributeArgumentList(
                                    SeparatedList(
                                        bindingMembers
                                           .Select(z => AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(z))))
                                    )
                                )
                            ),
                        }
                    )
                )
            );

            newSyntax = newSyntax.WithParameterList(newSyntax.ParameterList.ReplaceNode(declaredParam, newParam));


            if (parameterType.IsRecord)
            {
                if (parameterProperties.Any() || controllerAttachedProperties.Any())
                {
                    var withExpression = WithExpression(
                        IdentifierName(parameter.Name),
                        InitializerExpression(
                            SyntaxKind.WithInitializerExpression,
                            SeparatedList<ExpressionSyntax>(
                                parameterProperties
                                   .Select(
                                        tuple => AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            IdentifierName(tuple.member.Name),
                                            IdentifierName(tuple.parameter.Name)
                                        )
                                    )
                                   .Concat(
                                        controllerAttachedProperties.Select(
                                            s => AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                IdentifierName(s.member.Name),
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    ThisExpression(),
                                                    IdentifierName(s.controllerProperty.Name)
                                                )
                                            )
                                        )
                                    )
                            )
                        )
                    );
                    sendRequestExpression = isStream ? streamMediatorRequest(withExpression) : sendMediatorRequest(withExpression);
                }
            }
            else
            {
                foreach (( var member, var s ) in parameterProperties)
                {
                    block = block.AddStatements(
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(parameter.Name),
                                    IdentifierName(member.Name)
                                ),
                                IdentifierName(s.Name)
                            )
                        )
                    );
                }

                foreach (var s in controllerAttachedProperties)
                {
                    block = block.AddStatements(
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(parameter.Name),
                                    IdentifierName(s.member.Name)
                                ),
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    ThisExpression(),
                                    IdentifierName(s.controllerProperty.Name)
                                )
                            )
                        )
                    );
                }
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

        var knownStatusCodes = symbol
                              .GetAttributes()
                              .Where(z => z.AttributeClass?.Name == "ProducesResponseTypeAttribute")
                              .SelectMany(
                                   z =>
                                   {
                                       var namedArguments = z
                                                           .NamedArguments
                                                           .Where(
                                                                x => x is
                                                                {
                                                                    Key: "StatusCode", Value: { Kind: TypedConstantKind.Primitive, Value: int, },
                                                                }
                                                            )
                                                            // ReSharper disable once NullableWarningSuppressionIsUsed
                                                           .Select(c => (int)c.Value.Value!);
                                       var constructorArguments = z
                                                                 .ConstructorArguments.Where(c => c is { Kind: TypedConstantKind.Primitive, Value: int, })
                                                                  // ReSharper disable once NullableWarningSuppressionIsUsed
                                                                 .Select(c => (int)c.Value!);

                                       return namedArguments.Concat(constructorArguments);
                                   }
                               )
                              .ToImmutableHashSet();

        var hasSuccess = knownStatusCodes.Any(z => z is >= 200 and < 300);
        var hasDefault = symbol.GetAttribute("ProducesDefaultResponseTypeAttribute") is { };

        {
            var attributes = parameter.GetAttributes().ToLookup(z => z.AttributeClass?.Name ?? "");
            var declaredParam = newSyntax.ParameterList.Parameters.Single(z => z.Identifier.Text == parameter.Name);
            var newParam = declaredParam;

            if (matcher is { })
            {
                switch (matcher)
                {
                    case { Method: RestfulApiMethod.List, }:
                        {
                            if (!attributes["FromQueryAttribute"].Any()) newParam = newParam.AddAttributeLists(createSimpleAttribute("FromQuery"));
                            break;
                        }

                    case { Method: RestfulApiMethod.Update, }:
                    case { Method: RestfulApiMethod.Create, }:
                        {
                            if (!attributes["BindRequiredAttribute"].Any()) newParam = newParam.AddAttributeLists(createSimpleAttribute("BindRequired"));
                            if (!attributes["FromBodyAttribute"].Any()) newParam = newParam.AddAttributeLists(createSimpleAttribute("FromBody"));
                            break;
                        }

                    case { Method: RestfulApiMethod.Read, }:
                    case { Method: RestfulApiMethod.Delete, }:
                        {
                            if (!attributes["BindRequiredAttribute"].Any()) newParam = newParam.AddAttributeLists(createSimpleAttribute("BindRequired"));
                            if (!attributes["FromRouteAttribute"].Any()) newParam = newParam.AddAttributeLists(createSimpleAttribute("FromRoute"));
                            break;
                        }
                }
            }

            newSyntax = newSyntax.WithParameterList(newSyntax.ParameterList.ReplaceNode(declaredParam, newParam));
        }

        if (!hasDefault)
        {
            newSyntax = newSyntax.AddAttributeLists(createSimpleAttribute("ProducesDefaultResponseType"));
        }

        var statusCode = hasSuccess ? knownStatusCodes.First(z => z is >= 200 and < 300) : StatusCodes.Status200OK;

        if (!hasSuccess)
        {
            if (isUnitResult)
            {
                statusCode = StatusCodes.Status204NoContent;
            }
            else if (matcher is { })
            {
                statusCode = statusCodeMap[matcher.Method];
            }

            newSyntax = newSyntax.AddAttributeLists(produces(statusCode));
        }

        if (!knownStatusCodes.Contains(StatusCodes.Status404NotFound) && matcher?.Method != RestfulApiMethod.List)
        {
            newSyntax = newSyntax.AddAttributeLists(produces(StatusCodes.Status404NotFound, "ProblemDetails"));
        }

        if (!knownStatusCodes.Contains(StatusCodes.Status400BadRequest))
        {
            newSyntax = newSyntax.AddAttributeLists(produces(StatusCodes.Status400BadRequest, "ProblemDetails"));
        }

        if (!knownStatusCodes.Contains(StatusCodes.Status422UnprocessableEntity))
        {
            newSyntax = newSyntax.AddAttributeLists(
                produces(StatusCodes.Status422UnprocessableEntity, "FluentValidationProblemDetails")
            );
        }


        if (isStream)
        {
            block = block.AddStatements(ReturnStatement(IdentifierName(resultName)));
        }
        else
        {
            if (symbol.GetAttribute("CreatedAttribute") is { ConstructorArguments: { Length: 1, }, } created)
            {
                // ReSharper disable once NullableWarningSuppressionIsUsed
                var actionName = (string)created.ConstructorArguments[0].Value!;
                var relatedMember = members.Single(z => z.symbol.Name == actionName);
                var routeValues = getRouteValues(parameterType, resultName, relatedMember);

                block = block.AddStatements(
                    ReturnStatement(routeResult("CreatedAtActionResult", resultName, actionName, routeValues))
                );
            }
            else if (symbol.GetAttribute("AcceptedAttribute") is { ConstructorArguments: { Length: 1, }, } accepted)
            {
                // ReSharper disable once NullableWarningSuppressionIsUsed
                var actionName = (string)accepted.ConstructorArguments[0].Value!;
                var relatedMember = members.Single(z => z.symbol.Name == actionName);
                var routeValues = getRouteValues(parameterType, resultName, relatedMember);

                block = block.AddStatements(
                    ReturnStatement(
                        routeResult("AcceptedAtActionResult", resultName, actionName, routeValues)
                    )
                );
            }
            else
            {
                block = block.AddStatements(ReturnStatement(isUnitResult ? statusCodeResult(statusCode) : objectResult(resultName, statusCode)));
            }
        }

        return newSyntax
              .WithBody(block.NormalizeWhitespace())
              .WithSemicolonToken(Token(SyntaxKind.None));

        static ImmutableArray<string> availableRouteParameters(
            (MethodDeclarationSyntax method, IMethodSymbol symbol, IRestfulApiMethodMatcher? matcher, IParameterSymbol? request) relatedMember
        )
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed
            var parameterNames = relatedMember.symbol.Parameters.Remove(relatedMember.request!).Select(z => z.Name);
            parameterNames = parameterNames.Concat(
                relatedMember
                   .request?.Type.GetMembers()
                   .Where(z => z is IPropertySymbol { IsImplicitlyDeclared: false, } ps && ps.Name != "EqualityContract")
                   .Select(z => z.Name)
             ?? Enumerable.Empty<string>()
            );
            return parameterNames.Select(Helpers.Camelize).Distinct().ToImmutableArray();
        }

        static ExpressionSyntax getRouteValues(
            INamedTypeSymbol parameterType,
            string resultName,
            (MethodDeclarationSyntax method, IMethodSymbol symbol, IRestfulApiMethodMatcher? matcher, IParameterSymbol? request) relatedMember
        )
        {
            var parameterNames = availableRouteParameters(relatedMember);
            var response = parameterType.AllInterfaces.First(z => z is { Name: "IRequest", Arity: 1, }).TypeArguments[0];

            var properties = response.GetMembers().Select(z => z.Name);
            var routeValues = parameterNames.Join(
                properties,
                z => z,
                z => z,
                (right, left) => AnonymousObjectMemberDeclarator(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(resultName),
                            IdentifierName(left)
                        )
                    )
                   .WithNameEquals(
                        NameEquals(
                            IdentifierName(right)
                        )
                    ),
                StringComparer.OrdinalIgnoreCase
            );
            return AnonymousObjectCreationExpression(SeparatedList(routeValues));
        }

        static ExpressionSyntax objectResult(string contentName, int statusCode)
        {
            return ObjectCreationExpression(IdentifierName("ObjectResult"))
                  .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName(contentName)))))
                  .WithInitializer(
                       InitializerExpression(
                           SyntaxKind.ObjectInitializerExpression,
                           SingletonSeparatedList<ExpressionSyntax>(
                               AssignmentExpression(
                                   SyntaxKind.SimpleAssignmentExpression,
                                   IdentifierName("StatusCode"),
                                   LiteralExpression(
                                       SyntaxKind.NumericLiteralExpression,
                                       Literal(statusCode)
                                   )
                               )
                           )
                       )
                   );
        }

        static ExpressionSyntax routeResult(
            string resultType,
            string resultName,
            string actionName,
            ExpressionSyntax routeValues,
            string? controllerName = null
        )
        {
            return ObjectCreationExpression(IdentifierName(resultType))
               .WithArgumentList(
                    ArgumentList(
                        SeparatedList(
                            new[]
                            {
                                Argument(
                                    LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(actionName))
                                ),
                                Argument(
                                    controllerName is null
                                        ? LiteralExpression(SyntaxKind.NullLiteralExpression)
                                        : LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(controllerName))
                                ),
                                Argument(routeValues),
                                Argument(IdentifierName(resultName)),
                            }
                        )
                    )
                );
        }

        static ExpressionSyntax statusCodeResult(int statusCode)
        {
            return ObjectCreationExpression(IdentifierName("StatusCodeResult"))
               .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    Literal(statusCode)
                                )
                            )
                        )
                    )
                );
        }

        static AttributeListSyntax produces(int statusCode, string? responseType = null)
        {
            if (responseType is { })
            {
                return AttributeList(
                    SingletonSeparatedList(
                        Attribute(
                            IdentifierName("ProducesResponseType"),
                            AttributeArgumentList(
                                SeparatedList(
                                    new[]
                                    {
                                        AttributeArgument(TypeOfExpression(ParseName(responseType))),
                                        AttributeArgument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(statusCode))),
                                    }
                                )
                            )
                        )
                    )
                );
            }

            return AttributeList(
                SingletonSeparatedList(
                    Attribute(
                        IdentifierName("ProducesResponseType"),
                        AttributeArgumentList(
                            SingletonSeparatedList(
                                AttributeArgument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(statusCode)))
                            )
                        )
                    )
                )
            );
        }

        static AttributeListSyntax createSimpleAttribute(string name)
        {
            return AttributeList(
                SingletonSeparatedList(
                    Attribute(
                        IdentifierName(name)
                    )
                )
            );
        }


        static ExpressionSyntax sendMediatorRequest(ExpressionSyntax nameSyntax)
        {
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
                               .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList(
                                            new[]
                                            {
                                                Argument(nameSyntax),
                                                Argument(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("HttpContext"),
                                                        IdentifierName("RequestAborted")
                                                    )
                                                ),
                                            }
                                        )
                                    )
                                ),
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


        static ExpressionSyntax streamMediatorRequest(ExpressionSyntax nameSyntax)
        {
            return InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Mediator"),
                        IdentifierName("CreateStream")
                    )
                )
               .WithArgumentList(
                    ArgumentList(
                        SeparatedList(
                            new[]
                            {
                                Argument(nameSyntax),
                                Argument(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("HttpContext"),
                                        IdentifierName("RequestAborted")
                                    )
                                ),
                            }
                        )
                    )
                );
        }
    }

    private void GenerateMethods(
        SourceProductionContext context,
        ((ClassDeclarationSyntax syntax, INamedTypeSymbol symbol, SemanticModel semanticModel) Left, Compilation Right) valueTuple
    )
    {
        ( var syntax, var symbol, var semanticModel ) = valueTuple.Left;
        // ReSharper disable NullableWarningSuppressionIsUsed
        var controllerBase = semanticModel.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ControllerBase")!;
        // ReSharper enable NullableWarningSuppressionIsUsed
        var controllerBaseProperties =
            controllerBase.GetMembers().OfType<IPropertySymbol>().Where(z => z.DeclaredAccessibility == Accessibility.Public).ToArray();
        var compilation = valueTuple.Right;
        var matchers = MatcherDefaults.GetMatchers(compilation);
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

                              var actionModel = new ActionModel(compilation, methodSymbol.Name, methodSymbol);
                              var matcher = matchers.FirstOrDefault(z => z.IsMatch(actionModel));
                              var request = methodSymbol.Parameters.FirstOrDefault(p => p.Type.AllInterfaces.Any(i => i.MetadataName == "IRequest`1"))
                               ?? methodSymbol.Parameters.FirstOrDefault(p => p.Type.AllInterfaces.Any(i => i.MetadataName == "IRequest"));
                              var streamRequest = methodSymbol.Parameters.FirstOrDefault(
                                  p => p.Type.AllInterfaces.Any(i => i.MetadataName == "IStreamRequest`1")
                              );
                              return ( method, symbol: methodSymbol, matcher, request: request ?? streamRequest );
                          }
                      )
                     .Where(z => z is { symbol: { }, method: { }, })
                     .ToImmutableArray();

        var newClass = syntax
                      .WithMembers(List<MemberDeclarationSyntax>())
                      .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                      .WithAttributeLists(List<AttributeListSyntax>())
                      .WithBaseList(null)
            ;


        foreach (( var method, var methodSymbol, var matcher, var request ) in members)
        {
            if (request == null) continue;
            var methodBody = GenerateMethod(
                context,
                controllerBaseProperties,
                matcher,
                MatcherDefaults.MethodStatusCodeMap,
                method,
                methodSymbol,
                request,
                members
            );
            if (methodBody is null) continue;
            newClass = newClass.AddMembers(methodBody);
        }

        var additionalUsings = new[]
        {
            "Microsoft.AspNetCore.Mvc",
            "FluentValidation.AspNetCore",
            "Rocket.Surgery.LaunchPad.AspNetCore.Validation",
        };

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

        context.AddSourceRelativeTo(syntax, "ControllerMethods", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxProvider = context
                            .SyntaxProvider
                            .CreateSyntaxProvider(
                                 (node, _) => node is ClassDeclarationSyntax cds
                                  && cds.BaseList?.Types.FirstOrDefault()?.Type.GetSyntaxName() == "RestfulApiController"
                                  && cds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))
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

        context.RegisterPostInitializationOutput(
            initializationContext =>
            {
                initializationContext.AddSource(
                    "Attributes.cs",
                    @"
using System;

namespace Rocket.Surgery.LaunchPad.AspNetCore
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.Runtime.CompilerServices.CompilerGenerated]
    [AttributeUsage(AttributeTargets.Method)]
    sealed class CreatedAttribute : Attribute
    {
        public CreatedAttribute(string methodName){ MethodName = methodName; }
        public string MethodName { get; }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.Runtime.CompilerServices.CompilerGenerated]
    [AttributeUsage(AttributeTargets.Method)]
    sealed class AcceptedAttribute : Attribute
    {
        public AcceptedAttribute(string methodName){ MethodName = methodName; }
        public string MethodName { get; }
    }
}"
                );
            }
        );

        context.RegisterSourceOutput(syntaxProvider.Combine(context.CompilationProvider), GenerateMethods);
    }
}
