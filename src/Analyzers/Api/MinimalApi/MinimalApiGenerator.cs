using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Generates a <c>Map{Host}()</c> extension for every Minimal API operation host: a route group under <c>/{RoutePrefix}/{Group}</c> with one
///     endpoint per listed operation, dispatched through the mediator profile.
/// </summary>
/// <remarks>
///     <para>
///         The Request Delegate Generator cannot see generated source, so a lambda passed to <c>MapGet</c> here would fall back to the reflection-based
///         request delegate factory and break trimming and native AOT. Instead each operation gets what the Request Delegate Generator would have
///         written: a typed handler method, and a <c>RequestDelegate</c> that binds the route, query, headers, services and body itself (with
///         <c>TryParse</c> and <c>JsonTypeInfo</c>), calls the handler and executes its result.
///     </para>
///     <para>
///         The handler's <see cref="System.Reflection.MethodInfo" /> and its result type's <c>IEndpointMetadataProvider</c> metadata are added to the
///         endpoint, so ApiExplorer and OpenAPI describe it exactly as they would a hand-written typed lambda.
///     </para>
/// </remarks>
[Generator]
public sealed class MinimalApiGenerator : IIncrementalGenerator
{
    private const string Http = "global::Microsoft.AspNetCore.Http";
    private const string Results = Http + ".HttpResults";
    private const string Ct = "__cancellationToken";
    // Each transport package emits its helpers into its own namespace, so a project can reference several of them.
    private const string Generated = "global::Rocket.Surgery.LaunchPad.Api.Generated.MinimalApi";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var hosts = HostPipeline.Hosts(context, HostKind.MinimalApi);
        context.RegisterSourceOutput(hosts, static (output, models) => Emit(output, models));
        context.RegisterSourceOutput(hosts.Combine(JsonMetadataCoverage.Provider(context)), static (output, pair) => JsonMetadataCoverage.Report(output, pair.Left, pair.Right));
    }

    private static void Emit(SourceProductionContext output, EquatableArray<HostModel> hosts)
    {
        // The helpers use the transport's library, so only a project with a host of this kind gets them.
        if (!hosts.IsEmpty) output.AddSource("Rocket.Surgery.LaunchPad.Api.Generated.ServerHelpers.g.cs", ServerHelpers);
        // Endpoint names are global to an application: an operation listed on two hosts gets its host noun in front.
        var duplicateNames = hosts.SelectMany(h => h.Operations.Select(o => o.EndpointName)).GroupBy(n => n).Where(g => g.Count() > 1).Select(g => g.Key).ToHashSet();
        foreach (var host in hosts)
        {
            foreach (var diagnostic in host.Diagnostics) output.ReportDiagnostic(diagnostic.ToDiagnostic());
            if (host.Profile is null) continue;
            ServerEmit.ReportUnmappedVersions(output, host);
            output.AddSource(host.FullyQualifiedName.Replace("global::", "") + ".MinimalApi.g.cs", Write(host, host.Profile, duplicateNames, output));
        }

        foreach (var duplicate in hosts.SelectMany(h => h.Operations.Select(o => (Host: h, Operation: o)))
                                       .GroupBy(x => (x.Operation.Verb, Route: "/" + RouteText.Join(x.Host.BaseRoute, x.Operation.Route).ToLowerInvariant()))
                                       .Where(g => g.Select(x => x.Host.Name).Distinct().Count() > 1))
        {
            var items = duplicate.ToArray();
            output.ReportDiagnostic(
                DiagnosticInfo.Create(ApiDiagnostics.DuplicateRoute, items[1].Operation.Location, ServerEmit.Verb(duplicate.Key.Verb), duplicate.Key.Route, items[0].Host.Name, items[1].Host.Name)
                              .ToDiagnostic()
            );
        }
    }

    private const string Endpoints = "global::Microsoft.AspNetCore.Builder.RoutingEndpointConventionBuilderExtensions";
    private const string OpenApi = "global::Microsoft.AspNetCore.Http.OpenApiRouteHandlerBuilderExtensions";
    private const string Authorization = "global::Microsoft.AspNetCore.Builder.AuthorizationEndpointConventionBuilderExtensions";

    private static string Write(HostModel host, ProfileModel profile, HashSet<string> duplicateNames, SourceProductionContext output)
    {
        var mapped = new List<(OperationModel Operation, string Service)>();
        foreach (var operation in host.Operations)
        {
            if (ServerDispatch.ServiceType(profile, operation) is not { } service)
            {
                output.ReportDiagnostic(
                    DiagnosticInfo.Create(ApiDiagnostics.FeatureSkipped, operation.Location, operation.Name, host.Name, $"the {profile.Name} profile cannot dispatch {ServerEmit.KindName(operation.Kind)} operations")
                                  .ToDiagnostic()
                );
                continue;
            }

            mapped.Add((operation, service));
        }

        var map = Method(
                               Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword),
                               Type("global::Microsoft.AspNetCore.Routing.RouteGroupBuilder"),
                               $"Map{host.Name}",
                               Parameters(This(Type("global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder"), "endpoints")),
                               new[]
                                   {
                                       ThrowIfNull("endpoints"),
                                       Var("group", Invoke(Member(Id("endpoints"), "MapGroup"), Literal("/" + host.BaseRoute))),
                                       Statement(Invoke(Member(OpenApi, "WithTags"), Id("group"), Literal(Naming.HostNoun(host.Name)))),
                                   }
                                  .Concat(VersionSet(host))
                                  .Concat(mapped.Select(m => Mapping(host, profile, m.Operation, m.Service, duplicateNames.Contains(m.Operation.EndpointName))))
                                  .Append(Return(Id("group")))
                           )
                          .Documented(
                               $"<summary>\n    Maps the operations of <see cref=\"{host.Name}\" /> under <c>/{Xml(host.BaseRoute)}</c>.\n</summary>\n<returns>The route group, to add metadata or conventions to every operation.</returns>"
                           );
        var members = new List<MemberDeclarationSyntax> { map };
        foreach (var (operation, service) in mapped)
        {
            members.Add(Handler(host, profile, operation, service));
            members.Add(RequestDelegate(profile, operation, service));
            members.Add(
                PartialHook($"Configure{operation.Name}", true, Parameter(Type("global::Microsoft.AspNetCore.Builder.IEndpointConventionBuilder"), "endpoint"))
                   .Summary($"Customises the generated <c>{operation.Name}</c> endpoint. Implement it in your part of <see cref=\"{host.Name}\" />.")
            );
        }

        var endpoints = Class(host.Name, Modifiers(Accessibility(host.Accessibility), SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword), members)
           .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.MinimalApi", typeof(MinimalApiGenerator).Assembly.GetName().Version));
        // MapGroup and MapMethods are called as extensions: HotChocolate.AspNetCore declares a class with the same full name as theirs.
        return Render(host.Namespace, [endpoints], ["Microsoft.AspNetCore.Builder"]);
    }

    // var __versionSet = endpoints.NewApiVersionSet().HasApiVersion(…)….Build(); for a host with versions, when Asp.Versioning is referenced.
    private static IEnumerable<StatementSyntax> VersionSet(HostModel host)
    {
        var versions = ServerEmit.Versions(host);
        if (!host.HasApiVersioning || versions.Count == 0) yield break;
        var set = versions.Aggregate((ExpressionSyntax)Invoke(Member(Id("endpoints"), "NewApiVersionSet")), (builder, v) => Invoke(Member(builder, "HasApiVersion"), ServerEmit.ApiVersion(v)));
        yield return Var("__versionSet", Invoke(Member(set, "Build")));
    }

    // One endpoint: MapMethods over the generated request delegate, plus the metadata ApiExplorer and OpenAPI read from a typed handler.
    private static StatementSyntax Mapping(HostModel host, ProfileModel profile, OperationModel operation, string service, bool prefixName)
    {
        var (result, _) = ReturnType(profile, operation);
        var endpoint = Id("endpoint");
        ExpressionSyntax WithMetadata(ExpressionSyntax metadata) => Invoke(Member(Endpoints, "WithMetadata"), endpoint, metadata);
        var statements = new List<StatementSyntax>
        {
            Var(
                "endpoint",
                Invoke(
                    Member(Id("group"), "MapMethods"),
                    Literal(operation.Route.Length == 0 ? "" : "/" + operation.Route.Trim('/')),
                    NewArray(Type("string"), [Literal(ServerEmit.Verb(operation.Verb))]),
                    New(Type($"{Http}.RequestDelegate"), Id($"__{operation.Name}RequestDelegate"))
                )
            ),
            // The typed handler is what ApiExplorer and OpenAPI read: its parameters, their binding attributes and its result type.
            Var(
                "method",
                NullForgiving(
                    Invoke(
                        Member(TypeOf(SyntaxFactory.IdentifierName(host.Name)), "GetMethod"),
                        NameOf($"__{operation.Name}"),
                        BitwiseOr(Member("global::System.Reflection.BindingFlags", "NonPublic"), Member("global::System.Reflection.BindingFlags", "Static"))
                    )
                )
            ),
            Statement(WithMetadata(Id("method"))),
            Statement(
                Invoke(
                    Member(endpoint, "Add"),
                    Lambda("builder", Invoke(GenericMember(Name($"{Generated}.ApiEndpointMetadata"), "Populate", result), Id("method"), Id("builder")))
                )
            ),
            // ApiExplorer reads each parameter's binding from IParameterBindingMetadata, which the Request Delegate Generator would have added.
            Var("parameters", Invoke(Member(Id("method"), "GetParameters"))),
        };
        var index = 0;
        foreach (var parameter in ServerEmit.Parameters(operation, service, Ct))
        {
            var binding = New(
                Type($"{Generated}.ApiParameterBindingMetadata"),
                Index(Id("parameters"), Literal(index)),
                parameter.HasTryParse ? True : False,
                parameter.IsOptional ? True : False
            );
            statements.Add(Statement(WithMetadata(binding)));
            index++;
        }

        if (operation.HasBody) statements.Add(Statement(WithMetadata(New(Type($"{Generated}.ApiAcceptsMetadata"), TypeOf(Type(operation.WireType))))));

        // The request delegate answers 400 when a body, route, query or header value can't be bound.
        var binds = operation.HasBody || operation.Members.Any(m => m.Source is BindingSource.Route or BindingSource.Query or BindingSource.Header);
        if (binds) statements.Add(Statement(WithMetadata(ProblemMetadata(400))));

        // A union's error cases answer ProblemDetails; ProblemHttpResult describes no status of its own.
        foreach (var status in operation.Union?.Errors.Select(e => e.StatusCode).Distinct().Where(s => s != 400 || !binds) ?? [])
        {
            statements.Add(Statement(WithMetadata(ProblemMetadata(status))));
        }

        var name = prefixName ? Naming.HostNoun(host.Name) + operation.EndpointName : operation.EndpointName;
        statements.Add(Statement(Invoke(Member(Endpoints, "WithName"), endpoint, Literal(name))));
        if (operation.Summary is { } summary) statements.Add(Statement(Invoke(Member(OpenApi, "WithSummary"), endpoint, Literal(summary))));
        if (operation.Description is { } description) statements.Add(Statement(Invoke(Member(OpenApi, "WithDescription"), endpoint, Literal(description))));
        if (!operation.Tags.IsEmpty) statements.Add(Statement(Invoke(Member(OpenApi, "WithTags"), new[] { (ExpressionSyntax)endpoint }.Concat(operation.Tags.Select(t => (ExpressionSyntax)Literal(t))))));
        foreach (var policy in operation.Policies) statements.Add(Statement(Invoke(Member(Authorization, "RequireAuthorization"), endpoint, Literal(policy))));
        if (!operation.Roles.IsEmpty || !operation.AuthenticationSchemes.IsEmpty)
        {
            var settings = new List<(string, ExpressionSyntax)>();
            if (!operation.Roles.IsEmpty) settings.Add(("Roles", Literal(string.Join(",", operation.Roles))));
            if (!operation.AuthenticationSchemes.IsEmpty) settings.Add(("AuthenticationSchemes", Literal(string.Join(",", operation.AuthenticationSchemes))));
            statements.Add(Statement(Invoke(Member(Authorization, "RequireAuthorization"), endpoint, New(Type("global::Microsoft.AspNetCore.Authorization.AuthorizeAttribute"), [], settings))));
        }

        if (operation.AllowAnonymous) statements.Add(Statement(Invoke(Member(Authorization, "AllowAnonymous"), endpoint)));
        if (host.HasApiVersioning && ServerEmit.Versions(host).Count > 0)
        {
            // Every endpoint of the host belongs to its version set; one without a version of its own serves all of them.
            statements.Add(Statement(Invoke(Member(endpoint, "WithApiVersionSet"), Id("__versionSet"))));
            if (ServerEmit.Version(host, operation) is { } version) statements.Add(Statement(Invoke(Member(endpoint, "MapToApiVersion"), ServerEmit.ApiVersion(version))));
        }

        statements.Add(Statement(Invoke(Id($"Configure{operation.Name}"), endpoint)));
        return Block(statements);
    }

    private static ExpressionSyntax ProblemMetadata(int status) =>
        New(
            Type("global::Microsoft.AspNetCore.Http.ProducesResponseTypeMetadata"),
            Literal(status),
            TypeOf(Type("global::Microsoft.AspNetCore.Mvc.ProblemDetails")),
            NewArray(Type("string"), [Literal("application/problem+json")])
        );

    // The typed handler: bound values in, the mediator call, a typed result out. Its signature is what OpenAPI describes.
    private static MethodDeclarationSyntax Handler(HostModel host, ProfileModel profile, OperationModel operation, string service)
    {
        var (result, isAsync) = ReturnType(profile, operation);
        var modifiers = isAsync ? Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.AsyncKeyword) : Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword);
        return Method(
                            modifiers,
                            isAsync ? Task(result) : result,
                            $"__{operation.Name}",
                            Parameters(ServerEmit.Parameters(operation, service, Ct).Select(p => p.Parameter)),
                            ServerEmit.BuildRequest(operation).Concat(DispatchAndResult(profile, operation))
                        )
                       .Summary($"Handles <c>{ServerEmit.Verb(operation.Verb)} /{Xml(RouteText.Join(host.BaseRoute, operation.Route))}</c>.");
    }

    // What the Request Delegate Generator would have written: bind every parameter from the HttpContext, call the handler, execute the result.
    private static MethodDeclarationSyntax RequestDelegate(ProfileModel profile, OperationModel operation, string service)
    {
        var request = Member(Id("httpContext"), "Request");
        var body = new List<StatementSyntax> { Var(Ct, Member(Id("httpContext"), "RequestAborted")) };
        foreach (var member in operation.Members)
        {
            switch (member.Source)
            {
                case BindingSource.Route:
                    body.AddRange(Parse(member, ConditionalInvoke(Index(Member(request, "RouteValues"), Literal(member.WireName)), "ToString"), "route value"));
                    break;
                case BindingSource.Query when !operation.HasBody:
                    body.AddRange(Parse(member, Index(Member(request, "Query"), Literal(member.WireName)), "query string value"));
                    break;
                case BindingSource.Header:
                    body.AddRange(Parse(member, Index(Member(request, "Headers"), Literal(member.WireName)), "header"));
                    break;
                case BindingSource.Service:
                    body.Add(Var(ServerEmit.Parameter(member), GetRequiredService(Type(member.TypeWithoutNullable), Member(Id("httpContext"), "RequestServices"))));
                    break;
                case BindingSource.Query:
                case BindingSource.Body:
                case BindingSource.Ignored:
                default:
                    break;
            }
        }

        if (operation.HasBody)
        {
            var read = Invoke(GenericMember(Name($"{Generated}.ApiBinding"), "ReadBodyAsync", Type(operation.WireType)), Id("httpContext"), Id(Ct));
            body.Add(Deconstruct(["__read", "__body"], Await(read)));
            body.Add(If(Not(Id("__read")), Return()));
        }

        body.Add(Var(ServerDispatch.SenderParameter, GetRequiredService(Type(service), Member(Id("httpContext"), "RequestServices"))));
        var (_, isAsync) = ReturnType(profile, operation);
        var call = Invoke(Id($"__{operation.Name}"), ServerEmit.Parameters(operation, service, Ct).Select(p => (ExpressionSyntax)Id(p.Parameter.Identifier.ValueText)));
        body.Add(Var("__result", isAsync ? Await(call) : call));
        body.Add(Statement(Await(Invoke(Member(Id("__result"), "ExecuteAsync"), Id("httpContext")))));
        return Method(
            Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.AsyncKeyword),
            Task(),
            $"__{operation.Name}RequestDelegate",
            Parameters(Parameter(Type($"{Http}.HttpContext"), "httpContext")),
            body
        );
    }

    // Parses one route, query or header value into a local named after the member. A missing required value or an unparsable one is a 400.
    private static IEnumerable<StatementSyntax> Parse(MemberModel member, ExpressionSyntax source, string what)
    {
        var name = ServerEmit.Parameter(member);
        var raw = "__raw_" + member.Name;
        var badRequest = Block(Statement(Await(Invoke(Member($"{Generated}.ApiBinding", "BadRequestAsync"), Id("httpContext"), Literal(member.WireName), Literal(what)))), Return());
        if (member.IsCollection)
        {
            var list = name + "__list";
            yield return Local(Type("global::Microsoft.Extensions.Primitives.StringValues"), raw, source);
            yield return Var(list, New(Generic("global::System.Collections.Generic.List", Type(member.ParseType!)), Member(Id(raw), "Count")));
            yield return ForEach(
                "__item",
                Id(raw),
                Block(If(Or(IsNull(Id("__item")), Not(TryParse(member, Id("__item"), "__parsed"))), badRequest), Statement(Invoke(Member(Id(list), "Add"), Id("__parsed"))))
            );
            yield return Var(name, Invoke(Member(Id(list), "ToArray")));
            yield break;
        }

        yield return Local(Type("string?"), raw, source);
        var isEmpty = Invoke(Member(Type("string"), "IsNullOrEmpty"), Id(raw));
        if (member.ParseKind == ParseKind.String)
        {
            if (member.IsNullable)
            {
                yield return Var(name, Conditional(isEmpty, Null, Id(raw)));
            }
            else
            {
                yield return If(isEmpty, badRequest);
                yield return Var(name, Id(raw));
            }

            yield break;
        }

        var parsed = "__parsed_" + member.Name;
        yield return Local(Type(member.Type), name, Default);
        yield return If(
            Not(isEmpty),
            Block(If(Not(TryParse(member, Id(raw), parsed)), badRequest), Statement(Assign(Id(name), Id(parsed)))),
            member.IsNullable ? null : badRequest
        );
    }

    private static ExpressionSyntax TryParse(MemberModel member, ExpressionSyntax value, string result) => member.ParseKind switch
    {
        ParseKind.Enum => Invoke(GenericMember(Name("global::System.Enum"), "TryParse", Type(member.ParseType!)), [SyntaxFactory.Argument(value), SyntaxFactory.Argument(True), OutVar(result)]),
        ParseKind.Uri => Invoke(Member("global::System.Uri", "TryCreate"), [SyntaxFactory.Argument(value), SyntaxFactory.Argument(Member("global::System.UriKind", "RelativeOrAbsolute")), OutVar(result)]),
        ParseKind.Parsable or ParseKind.TryParseWithProvider => Invoke(
            Member(Type(member.ParseType!), "TryParse"),
            [SyntaxFactory.Argument(value), SyntaxFactory.Argument(Member("global::System.Globalization.CultureInfo", "InvariantCulture")), OutVar(result)]
        ),
        ParseKind.TryParse => Invoke(Member(Type(member.ParseType!), "TryParse"), [SyntaxFactory.Argument(value), OutVar(result)]),
        ParseKind.String => Parenthesized(IsVar(value, result)),
        ParseKind.None => Parenthesized(And(IsVar(value, result), False)),
        _ => throw new ArgumentOutOfRangeException(nameof(member)),
    };

    private static IEnumerable<StatementSyntax> DispatchAndResult(ProfileModel profile, OperationModel operation)
    {
        var send = ServerDispatch.Send(profile, operation, Id("__request"), Id(Ct));
        var typedResults = Name($"{Http}.TypedResults");
        if (operation is { Union: { }, Kind: RequestKind.Stream })
        {
            // Each item is mapped to its value or its problem; the result streams values and ends with the first problem.
            var item = Generic($"{Generated}.ApiStreamItem", Type(operation.ResponseTypeNotNull!));
            var map = SyntaxFactory.SimpleLambdaExpression(SyntaxFactory.Parameter(SyntaxFactory.Identifier("__item")))
                                   .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                                   .WithBlock(
                                        Block(
                                            ServerEmit.UnionResult(
                                                operation,
                                                value => [Return(Invoke(Member(item, "Of"), value ?? NullForgiving(Default)))],
                                                (error, value) =>
                                                [
                                                    Return(
                                                        Invoke(
                                                            Member(item, "Error"),
                                                            Invoke(Member($"{Generated}.ApiProblems", "Details"), new ExpressionSyntax[] { Literal(error.StatusCode), Literal(error.Name) }.Concat(ServerEmit.ErrorMembers(error, value)))
                                                        )
                                                    ),
                                                ],
                                                operation.ResponseNullable ? () => [Return(Invoke(Member(item, "Of"), NullForgiving(Default)))] : null,
                                                "__item"
                                            )
                                        )
                                    );
            yield return Return(New(StreamUnionResult(operation), ServerDispatch.Stream(send), map));
            yield break;
        }

        if (operation.Union is { })
        {
            yield return Var("__response", Await(send.Call));
            foreach (var statement in ServerEmit.UnionResult(
                         operation,
                         value => [Return(value is null ? Invoke(Member(typedResults, "NoContent")) : Success(operation, typedResults, value))],
                         (error, value) => [Return(Invoke(Member($"{Generated}.ApiProblems", "Problem"), new ExpressionSyntax[] { Literal(error.StatusCode), Literal(error.Name) }.Concat(ServerEmit.ErrorMembers(error, value))))],
                         operation.ResponseNullable ? () => [Return(Invoke(Member(typedResults, "NotFound")))] : null
                     ))
            {
                yield return statement;
            }

            yield break;
        }

        switch (operation.Kind)
        {
            case RequestKind.Stream:
                yield return Return(New(Generic($"{Generated}.NdjsonResult", Type(operation.ResponseTypeNotNull!)), ServerDispatch.Stream(send)));
                yield break;
            case RequestKind.Void:
                yield return Statement(Await(send.Call));
                yield return Return(Invoke(Member(typedResults, "NoContent")));
                yield break;
            case RequestKind.Unary:
            case RequestKind.Infer:
            default:
                break;
        }

        yield return Var("__response", Await(send.Call));
        var value = operation.ResponseIsNullableValueType ? Member(Id("__response"), "Value") : Id("__response");
        var success = Success(operation, typedResults, value, Id("__response"));
        yield return Return(operation.ResponseNullable ? Conditional(IsNull(Id("__response")), Invoke(Member(typedResults, "NotFound")), success) : success);
    }

    // The success result: 200, 201 (with a Location when the created resource's GET is known) or 202, carrying the value.
    private static ExpressionSyntax Success(OperationModel operation, NameSyntax typedResults, ExpressionSyntax value, ExpressionSyntax? response = null) =>
        operation.SuccessStatusCode switch
        {
            201 when operation.CreatedLocationRoute is { } route => Invoke(Member(typedResults, "Created"), ServerEmit.LocationSyntax(route, operation, Generated, response ?? value), value),
            201 => Invoke(Member(typedResults, "Created"), Cast(Type("string?"), Null), value),
            202 => Invoke(Member(typedResults, "Accepted"), Cast(Type("string?"), Null), value),
            _ => Invoke(Member(typedResults, "Ok"), value),
        };

    /// <summary>The handler's result type (an <c>IResult</c>) and whether the handler is async (then it returns a Task of it).</summary>
    private static (TypeSyntax Result, bool IsAsync) ReturnType(ProfileModel profile, OperationModel operation)
    {
        // A stream of unions: values as NDJSON, ended by the first error case's problem.
        if (operation is { Union: { }, Kind: RequestKind.Stream }) return (StreamUnionResult(operation), ServerDispatch.Send(profile, operation, Id("__request"), Id(Ct)).Awaited);

        // A union: its value case's result, 404 for a nullable value case, and ProblemDetails for its error cases.
        if (operation.Union is { })
        {
            var results = new List<TypeSyntax> { operation.Kind == RequestKind.Void ? Type($"{Results}.NoContent") : SuccessType(operation) };
            if (operation.ResponseNullable) results.Add(Type($"{Results}.NotFound"));
            results.Add(Type($"{Results}.ProblemHttpResult"));
            return (Generic($"{Results}.Results", results), true);
        }

        switch (operation.Kind)
        {
            case RequestKind.Stream:
                // A stream whose call awaits (Foundatio's InvokeAsync<IAsyncEnumerable<T>>) is produced asynchronously.
                return (Generic($"{Generated}.NdjsonResult", Type(operation.ResponseTypeNotNull!)), ServerDispatch.Send(profile, operation, Id("__request"), Id(Ct)).Awaited);
            case RequestKind.Void:
                return (Type($"{Results}.NoContent"), true);
            case RequestKind.Unary:
            case RequestKind.Infer:
            default:
                break;
        }

        var success = SuccessType(operation);
        return (operation.ResponseNullable ? Generic($"{Results}.Results", success, Type($"{Results}.NotFound")) : success, true);
    }

    private static TypeSyntax StreamUnionResult(OperationModel operation) =>
        Generic($"{Generated}.NdjsonUnionResult", Type(operation.StreamItemType!), Type(operation.ResponseTypeNotNull!));

    private static TypeSyntax SuccessType(OperationModel operation) =>
        Generic(
            operation.SuccessStatusCode switch
            {
                201 => $"{Results}.Created",
                202 => $"{Results}.Accepted",
                _ => $"{Results}.Ok",
            },
            Type(operation.ResponseTypeNotNull!)
        );

    private const string ServerHelpers = """
// <auto-generated/>
#nullable enable
#pragma warning disable
namespace Rocket.Surgery.LaunchPad.Api.Generated.MinimalApi
{
    /// <summary>Formats a route, query or header value: invariant culture, enums by name, booleans in lowercase.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiFormat
    {
        public static string Value(object? value) => value switch
        {
            null => "",
            bool b => b ? "true" : "false",
            global::System.Enum e => e.ToString(),
            global::System.IFormattable f => f.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture),
            _ => value.ToString() ?? "",
        };
    }

    /// <summary>Binding helpers for the generated request delegates.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiBinding
    {
        /// <summary>The application's Minimal API JSON options (ConfigureHttpJsonOptions); native AOT apps add their JsonSerializerContext there.</summary>
        public static global::System.Text.Json.JsonSerializerOptions JsonOptions(global::Microsoft.AspNetCore.Http.HttpContext httpContext) =>
            global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
               .GetRequiredService<global::Microsoft.Extensions.Options.IOptions<global::Microsoft.AspNetCore.Http.Json.JsonOptions>>(httpContext.RequestServices)
               .Value.SerializerOptions;

        /// <summary>
        ///     Reads the JSON body, or writes 415 (not JSON) or 400 (missing or malformed) and returns <c>Read = false</c>. A flag rather than
        ///     null, because a request may be a struct.
        /// </summary>
        public static async global::System.Threading.Tasks.ValueTask<(bool Read, T Body)> ReadBodyAsync<T>(global::Microsoft.AspNetCore.Http.HttpContext httpContext, global::System.Threading.CancellationToken cancellationToken)
            where T : notnull
        {
            if (!global::Microsoft.AspNetCore.Http.HttpRequestJsonExtensions.HasJsonContentType(httpContext.Request))
            {
                httpContext.Response.StatusCode = global::Microsoft.AspNetCore.Http.StatusCodes.Status415UnsupportedMediaType;
                return (false, default!);
            }

            try
            {
                var typeInfo = (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<T>)JsonOptions(httpContext).GetTypeInfo(typeof(T));
                var body = await global::Microsoft.AspNetCore.Http.HttpRequestJsonExtensions.ReadFromJsonAsync(httpContext.Request, typeInfo, cancellationToken).ConfigureAwait(false);
                if (body is not null) return (true, body);
            }
            catch (global::System.Text.Json.JsonException)
            {
            }

            await BadRequestAsync(httpContext, "body", "request body").ConfigureAwait(false);
            return (false, default!);
        }

        /// <summary>
        ///     Answers 400, like ASP.NET Core's own binding failures, with a ProblemDetails body when the application registered
        ///     IProblemDetailsService (AddProblemDetails); its writer needs no JSON metadata from the application, so this stays AOT-safe.
        /// </summary>
        public static async global::System.Threading.Tasks.Task BadRequestAsync(global::Microsoft.AspNetCore.Http.HttpContext httpContext, string name, string what)
        {
            httpContext.Response.StatusCode = global::Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest;
            if (httpContext.RequestServices.GetService(typeof(global::Microsoft.AspNetCore.Http.IProblemDetailsService)) is global::Microsoft.AspNetCore.Http.IProblemDetailsService problems)
            {
                await problems.TryWriteAsync(
                    new global::Microsoft.AspNetCore.Http.ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        ProblemDetails =
                        {
                            Status = global::Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest,
                            Title = "Bad request",
                            Detail = $"The {what} '{name}' is missing or not valid.",
                        },
                    }
                ).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    ///     Answers an error case of a union response: ProblemDetails with the case's status, its name in the <c>case</c> extension and its members
    ///     as extensions (<c>name, value</c> pairs); a case with problem data fills detail, title, type and instance from it, written by ASP.NET Core like any other problem.
    /// </summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiProblems
    {
        public static global::Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult Problem(int status, string caseName, params object?[] members) =>
            global::Microsoft.AspNetCore.Http.TypedResults.Problem(Details(status, caseName, members));

        public static global::Microsoft.AspNetCore.Mvc.ProblemDetails Details(int status, string caseName, params object?[] members)
        {
            var problem = new global::Microsoft.AspNetCore.Mvc.ProblemDetails { Status = status };
            problem.Extensions["case"] = caseName;
            for (var i = 0; i + 1 < members.Length; i += 2)
            {
                // A case with problem data (Rocket.Surgery.LaunchPad.Foundation.IProblemDetailsData) passes it under reserved keys.
                switch ((string)members[i]!)
                {
                    case "$detail": problem.Detail = members[i + 1] as string; break;
                    case "$title": problem.Title = members[i + 1] as string ?? problem.Title; break;
                    case "$type": problem.Type = members[i + 1] as string; break;
                    case "$instance": problem.Instance = members[i + 1] as string; break;
                    case "$properties":
                        foreach (var property in members[i + 1] as global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<string, object?>> ?? [])
                        {
                            problem.Extensions[property.Key] = property.Value;
                        }

                        break;
                    default: problem.Extensions[(string)members[i]!] = members[i + 1]; break;
                }
            }

            return problem;
        }
    }

    /// <summary>One item of a stream of unions: its value case, or its error case as a problem.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal readonly struct ApiStreamItem<T>
    {
        private ApiStreamItem(T value, global::Microsoft.AspNetCore.Mvc.ProblemDetails? problem)
        {
            Value = value;
            Problem = problem;
        }

        public T Value { get; }

        public global::Microsoft.AspNetCore.Mvc.ProblemDetails? Problem { get; }

        public static ApiStreamItem<T> Of(T value) => new(value, null);

        public static ApiStreamItem<T> Error(global::Microsoft.AspNetCore.Mvc.ProblemDetails problem) => new(default!, problem);
    }

    /// <summary>
    ///     Writes a stream of unions as newline-delimited JSON: each value case as a line, flushed per item. The first error case ends the
    ///     stream: before anything was written it answers its status as ProblemDetails; afterwards it is the last line, a problem object.
    /// </summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal sealed class NdjsonUnionResult<TItem, T> : global::Microsoft.AspNetCore.Http.IResult, global::Microsoft.AspNetCore.Http.Metadata.IEndpointMetadataProvider
    {
        private static readonly byte[] NewLine = { (byte)'\n' };
        private readonly global::System.Collections.Generic.IAsyncEnumerable<TItem> _items;
        private readonly global::System.Func<TItem, ApiStreamItem<T>> _map;

        public NdjsonUnionResult(global::System.Collections.Generic.IAsyncEnumerable<TItem> items, global::System.Func<TItem, ApiStreamItem<T>> map)
        {
            _items = items;
            _map = map;
        }

        public async global::System.Threading.Tasks.Task ExecuteAsync(global::Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            var options = ApiBinding.JsonOptions(httpContext);
            var typeInfo = (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<T>)options.GetTypeInfo(typeof(T));
            var cancellationToken = httpContext.RequestAborted;
            var body = httpContext.Response.Body;
            var started = false;
            await foreach (var item in global::System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation(_items, cancellationToken).ConfigureAwait(false))
            {
                var mapped = _map(item);
                if (mapped.Problem is { } problem)
                {
                    if (!started)
                    {
                        await global::Microsoft.AspNetCore.Http.TypedResults.Problem(problem).ExecuteAsync(httpContext).ConfigureAwait(false);
                        return;
                    }

                    var problemInfo = (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Microsoft.AspNetCore.Mvc.ProblemDetails>)options.GetTypeInfo(typeof(global::Microsoft.AspNetCore.Mvc.ProblemDetails));
                    await global::System.Text.Json.JsonSerializer.SerializeAsync(body, problem, problemInfo, cancellationToken).ConfigureAwait(false);
                    await body.WriteAsync(NewLine, cancellationToken).ConfigureAwait(false);
                    await body.FlushAsync(cancellationToken).ConfigureAwait(false);
                    return;
                }

                if (!started)
                {
                    httpContext.Response.ContentType = "application/x-ndjson";
                    started = true;
                }

                await global::System.Text.Json.JsonSerializer.SerializeAsync(body, mapped.Value, typeInfo, cancellationToken).ConfigureAwait(false);
                await body.WriteAsync(NewLine, cancellationToken).ConfigureAwait(false);
                await body.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            if (!started) httpContext.Response.ContentType = "application/x-ndjson";
        }

        public static void PopulateMetadata(global::System.Reflection.MethodInfo method, global::Microsoft.AspNetCore.Builder.EndpointBuilder builder) =>
            builder.Metadata.Add(new global::Microsoft.AspNetCore.Http.ProducesResponseTypeMetadata(200, typeof(global::System.Collections.Generic.IAsyncEnumerable<T>), new[] { "application/x-ndjson" }));
    }

    /// <summary>Adds what a result type knows about itself (status codes, content types) to an endpoint, as typed lambdas would.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiEndpointMetadata
    {
        public static void Populate<T>(global::System.Reflection.MethodInfo method, global::Microsoft.AspNetCore.Builder.EndpointBuilder builder)
            where T : global::Microsoft.AspNetCore.Http.Metadata.IEndpointMetadataProvider => T.PopulateMetadata(method, builder);
    }

    /// <summary>Describes how one handler parameter is bound, for ApiExplorer and OpenAPI.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal sealed class ApiParameterBindingMetadata : global::Microsoft.AspNetCore.Http.Metadata.IParameterBindingMetadata
    {
        public ApiParameterBindingMetadata(global::System.Reflection.ParameterInfo parameterInfo, bool hasTryParse, bool isOptional)
        {
            ParameterInfo = parameterInfo;
            HasTryParse = hasTryParse;
            IsOptional = isOptional;
        }

        public string Name => ParameterInfo.Name!;

        public bool HasTryParse { get; }

        public bool HasBindAsync => false;

        public global::System.Reflection.ParameterInfo ParameterInfo { get; }

        public bool IsOptional { get; }
    }

    /// <summary>Tells ApiExplorer that an endpoint reads a JSON body of the given type.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal sealed class ApiAcceptsMetadata : global::Microsoft.AspNetCore.Http.Metadata.IAcceptsMetadata
    {
        public ApiAcceptsMetadata(global::System.Type requestType) => RequestType = requestType;

        public global::System.Collections.Generic.IReadOnlyList<string> ContentTypes { get; } = new[] { "application/json" };

        public global::System.Type? RequestType { get; }

        public bool IsOptional => false;
    }

    /// <summary>Writes a stream operation as newline-delimited JSON, flushing after every item.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal sealed class NdjsonResult<T> : global::Microsoft.AspNetCore.Http.IResult, global::Microsoft.AspNetCore.Http.Metadata.IEndpointMetadataProvider
    {
        private static readonly byte[] NewLine = { (byte)'\n' };
        private readonly global::System.Collections.Generic.IAsyncEnumerable<T> _items;

        public NdjsonResult(global::System.Collections.Generic.IAsyncEnumerable<T> items) => _items = items;

        public async global::System.Threading.Tasks.Task ExecuteAsync(global::Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            var typeInfo = (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<T>)ApiBinding.JsonOptions(httpContext).GetTypeInfo(typeof(T));
            var cancellationToken = httpContext.RequestAborted;
            httpContext.Response.ContentType = "application/x-ndjson";
            var body = httpContext.Response.Body;
            await foreach (var item in global::System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation(_items, cancellationToken).ConfigureAwait(false))
            {
                await global::System.Text.Json.JsonSerializer.SerializeAsync(body, item, typeInfo, cancellationToken).ConfigureAwait(false);
                await body.WriteAsync(NewLine, cancellationToken).ConfigureAwait(false);
                await body.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public static void PopulateMetadata(global::System.Reflection.MethodInfo method, global::Microsoft.AspNetCore.Builder.EndpointBuilder builder) =>
            builder.Metadata.Add(new global::Microsoft.AspNetCore.Http.ProducesResponseTypeMetadata(200, typeof(global::System.Collections.Generic.IAsyncEnumerable<T>), new[] { "application/x-ndjson" }));
    }
}
""";
}
