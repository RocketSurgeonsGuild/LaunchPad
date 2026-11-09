using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Generates an MVC action for every operation a controller host lists, and fills in what the host doesn't declare: <c>[ApiController]</c>,
///     <c>[Route("{RoutePrefix}/{Group}")]</c> and the <c>ControllerBase</c> base class.
/// </summary>
/// <remarks>
///     <para>
///         Actions return MVC results (<c>OkObjectResult</c>, <c>CreatedResult</c>, …), so responses go through MVC's formatters and JSON options
///         like a hand-written controller's, and each action carries <c>[ProducesResponseType]</c> for ApiExplorer and OpenAPI.
///     </para>
///     <para>
///         The mediator is resolved inside the action rather than taken as a parameter: some senders are internal to the project (ZeroAlloc),
///         and a public action can't take an internal parameter type.
///     </para>
/// </remarks>
[Generator]
public sealed class ControllerGenerator : IIncrementalGenerator
{
    private const string Mvc = "global::Microsoft.AspNetCore.Mvc";
    private const string Ct = "__cancellationToken";
    private const string Generated = "global::Rocket.Surgery.LaunchPad.Api.Generated.Mvc";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var hosts = HostPipeline.Hosts(context, HostKind.Controller);
        context.RegisterSourceOutput(hosts, static (output, models) => Emit(output, models));
    }

    private static void Emit(SourceProductionContext output, EquatableArray<HostModel> hosts)
    {
        // The helpers use the transport's library, so only a project with a host of this kind gets them.
        if (!hosts.IsEmpty) output.AddSource("Rocket.Surgery.LaunchPad.Api.Generated.ControllerHelpers.g.cs", ControllerHelpers);
        // Endpoint names are global to an application: an operation listed on two hosts gets its host noun in front.
        var duplicateNames = hosts.SelectMany(h => h.Operations.Select(o => o.EndpointName)).GroupBy(n => n).Where(g => g.Count() > 1).Select(g => g.Key).ToHashSet();
        foreach (var host in hosts)
        {
            foreach (var diagnostic in host.Diagnostics) output.ReportDiagnostic(diagnostic.ToDiagnostic());
            if (host.Profile is null) continue;
            ServerEmit.ReportUnmappedVersions(output, host);
            output.AddSource(host.FullyQualifiedName.Replace("global::", "") + ".Controller.g.cs", Write(host, host.Profile, duplicateNames, output));
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

    private static string Write(HostModel host, ProfileModel profile, HashSet<string> duplicateNames, SourceProductionContext output)
    {
        var actions = new List<MemberDeclarationSyntax>();
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

            actions.Add(Action(host, profile, operation, service, duplicateNames.Contains(operation.EndpointName)));
        }

        // What the host declares stays; the rest is added: [ApiController], [Route], the ControllerBase base.
        var attributes = new List<AttributeListSyntax>();
        if (!host.DeclaresApiController) attributes.Add(Attribute($"{Mvc}.ApiController"));
        if (host.DeclaredRoute is null) attributes.Add(Attribute($"{Mvc}.Route", Arg(Literal(host.BaseRoute))));
        attributes.Add(Attribute("global::Microsoft.AspNetCore.Http.Tags", Arg(Literal(Naming.HostNoun(host.Name)))));
        // Every version the actions are served under; an action with one of its own maps to it (Asp.Versioning.Mvc).
        if (host.HasApiVersioning) attributes.AddRange(ServerEmit.Versions(host).Select(v => Attribute("global::Asp.Versioning.ApiVersion", Arg(Literal(v)))));
        attributes.Add(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Controllers", typeof(ControllerGenerator).Assembly.GetName().Version));
        var controller = Class(host.Name, Modifiers(Accessibility(host.Accessibility), SyntaxKind.PartialKeyword), actions, host.DeclaresBaseType ? [] : [Type($"{Mvc}.ControllerBase")])
           .WithAttributeLists(SyntaxFactory.List(attributes));
        return Render(host.Namespace, [controller]);
    }

    private static MethodDeclarationSyntax Action(HostModel host, ProfileModel profile, OperationModel operation, string service, bool prefixName)
    {
        var send = ServerDispatch.Send(profile, operation, Id("__request"), Id(Ct));
        // A stream is returned synchronously unless producing it awaits (Foundatio's InvokeAsync<IAsyncEnumerable<T>>, Immediate.Handlers).
        var isAsync = operation.Kind != RequestKind.Stream || send.Awaited;
        var result = operation.Kind is RequestKind.Unary or RequestKind.Infer ? Generic($"{Mvc}.ActionResult", Type(operation.ResponseTypeNotNull!)) : Type($"{Mvc}.IActionResult");
        var modifiers = isAsync ? Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword) : Modifiers(SyntaxKind.PublicKeyword);

        var body = ServerEmit.BuildRequest(operation)
                             .Append(Var(ServerDispatch.SenderParameter, GetRequiredService(Type(service), Member(Id("HttpContext"), "RequestServices"))))
                             .Concat(DispatchAndResult(profile, operation, send));
        var name = prefixName ? Naming.HostNoun(host.Name) + operation.EndpointName : operation.EndpointName;
        return Method(modifiers, isAsync ? Task(result) : result, operation.Name, Parameters(ServerEmit.Parameters(operation, null, Ct).Select(p => p.Parameter)), body)
              .WithAttributeLists(SyntaxFactory.List(ActionAttributes(host, operation, name)))
              .Summary($"Handles <c>{ServerEmit.Verb(operation.Verb)} /{Xml(RouteText.Join(host.BaseRoute, operation.Route))}</c> by sending <see cref=\"{Cref(operation.RequestType)}\" />.");
    }

    private static IEnumerable<AttributeListSyntax> ActionAttributes(HostModel host, OperationModel operation, string name)
    {
        yield return Attribute($"{Mvc}.Http{operation.Verb}", Arg(Literal(operation.Route.Trim('/'))));
        yield return Attribute("global::Microsoft.AspNetCore.Routing.EndpointName", Arg(Literal(name)));
        if (operation.Summary is { } summary) yield return Attribute("global::Microsoft.AspNetCore.Http.EndpointSummary", Arg(Literal(summary)));
        if (operation.Description is { } description) yield return Attribute("global::Microsoft.AspNetCore.Http.EndpointDescription", Arg(Literal(description)));
        if (operation.Tags.Length > 0) yield return Attribute("global::Microsoft.AspNetCore.Http.Tags", [.. operation.Tags.Select(t => Arg(Literal(t)))]);
        foreach (var policy in operation.Policies) yield return Attribute(Authorize, Arg("Policy", Literal(policy)));
        if (operation.Roles.Length > 0 || operation.AuthenticationSchemes.Length > 0)
        {
            var settings = new List<AttributeArgumentSyntax>();
            if (operation.Roles.Length > 0) settings.Add(Arg("Roles", Literal(string.Join(",", operation.Roles))));
            if (operation.AuthenticationSchemes.Length > 0) settings.Add(Arg("AuthenticationSchemes", Literal(string.Join(",", operation.AuthenticationSchemes))));
            yield return Attribute(Authorize, [.. settings]);
        }

        if (operation.AllowAnonymous) yield return Attribute("global::Microsoft.AspNetCore.Authorization.AllowAnonymous");
        if (host.HasApiVersioning && ServerEmit.Version(host, operation) is { } version) yield return Attribute("global::Asp.Versioning.MapToApiVersion", Arg(Literal(version)));
        foreach (var produces in Produces(operation)) yield return produces;
    }

    // [ProducesResponseType] for every status the action can answer, so ApiExplorer describes it without reading the body.
    private static IEnumerable<AttributeListSyntax> Produces(OperationModel operation)
    {
        switch (operation.Kind)
        {
            case RequestKind.Stream:
                yield return ProducesResponseType(AsyncEnumerable(Type(operation.ResponseTypeNotNull!)), 200, "application/x-ndjson");
                break;
            case RequestKind.Void:
                yield return ProducesResponseType(null, 204, null);
                break;
            case RequestKind.Unary:
            case RequestKind.Infer:
                yield return ProducesResponseType(Type(operation.ResponseTypeNotNull!), operation.SuccessStatusCode, "application/json");
                if (operation.ResponseNullable) yield return ProducesResponseType(null, 404, null);
                break;
            default:
                break;
        }

        // [ApiController] answers 400 with ValidationProblemDetails when a value can't be bound.
        var binds = operation.HasBody || operation.Members.Any(m => m.Source is BindingSource.Route or BindingSource.Query or BindingSource.Header);
        if (binds) yield return ProducesResponseType(Type($"{Mvc}.ValidationProblemDetails"), 400, "application/problem+json");

        // A union's error cases answer ProblemDetails.
        foreach (var status in operation.Union?.Errors.Select(e => e.StatusCode).Distinct().Where(s => s != 400 || !binds) ?? [])
        {
            yield return ProducesResponseType(Type($"{Mvc}.ProblemDetails"), status, "application/problem+json");
        }
    }

    private static AttributeListSyntax ProducesResponseType(TypeSyntax? type, int status, string? contentType)
    {
        var arguments = new List<AttributeArgumentSyntax>();
        if (type is { }) arguments.Add(Arg(TypeOf(type)));
        arguments.Add(Arg(Literal(status)));
        if (contentType is { }) arguments.Add(Arg(Literal(contentType)));
        return Attribute($"{Mvc}.ProducesResponseType", [.. arguments]);
    }

    private static IEnumerable<StatementSyntax> DispatchAndResult(ProfileModel profile, OperationModel operation, (ExpressionSyntax Call, bool Awaited) send)
    {
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
            yield return Return(New(Generic($"{Generated}.NdjsonUnionResult", Type(operation.StreamItemType!), Type(operation.ResponseTypeNotNull!)), ServerDispatch.Stream(send), map));
            yield break;
        }

        if (operation.Union is { })
        {
            yield return Var("__response", Await(send.Call));
            foreach (var statement in ServerEmit.UnionResult(
                         operation,
                         value => [Return(value is null ? New(Type($"{Mvc}.NoContentResult")) : Success(operation, value, value))],
                         (error, value) => [Return(Invoke(Member($"{Generated}.ApiProblems", "Problem"), new ExpressionSyntax[] { Literal(error.StatusCode), Literal(error.Name) }.Concat(ServerEmit.ErrorMembers(error, value))))],
                         operation.ResponseNullable ? () => [Return(New(Type($"{Mvc}.NotFoundResult")))] : null
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
                yield return Return(New(Type($"{Mvc}.NoContentResult")));
                yield break;
            case RequestKind.Unary:
            case RequestKind.Infer:
            default:
                break;
        }

        yield return Var("__response", Await(send.Call));
        var value = operation.ResponseIsNullableValueType ? Member(Id("__response"), "Value") : Id("__response");
        var success = Success(operation, value, Id("__response"));
        yield return Return(operation.ResponseNullable ? Conditional(IsNull(Id("__response")), New(Type($"{Mvc}.NotFoundResult")), success) : success);
    }

    private static ExpressionSyntax Success(OperationModel operation, ExpressionSyntax value, ExpressionSyntax response) => operation.SuccessStatusCode switch
    {
        201 when operation.CreatedLocationRoute is { } route => New(Type($"{Mvc}.CreatedResult"), ServerEmit.LocationSyntax(route, operation, Generated, response), value),
        201 => New(Type($"{Mvc}.ObjectResult"), [value], [("StatusCode", Literal(201))]),
        202 => New(Type($"{Mvc}.AcceptedResult"), Cast(Type("string?"), Null), value),
        _ => New(Type($"{Mvc}.OkObjectResult"), value),
    };

    private const string Authorize = "global::Microsoft.AspNetCore.Authorization.Authorize";

    private const string ControllerHelpers = """
// <auto-generated/>
#nullable enable
#pragma warning disable
namespace Rocket.Surgery.LaunchPad.Api.Generated.Mvc
{
    /// <summary>Formats a route value: invariant culture, enums by name, booleans in lowercase.</summary>
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

    /// <summary>
    ///     Answers an error case of a union response: ProblemDetails with the case's status, its name in the <c>case</c> extension and its members
    ///     as extensions (<c>name, value</c> pairs), written through MVC's formatters.
    /// </summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiProblems
    {
        // ObjectResult writes a ProblemDetails value as application/problem+json.
        public static global::Microsoft.AspNetCore.Mvc.ObjectResult Problem(int status, string caseName, params object?[] members) =>
            new global::Microsoft.AspNetCore.Mvc.ObjectResult(Details(status, caseName, members)) { StatusCode = status };

        public static global::Microsoft.AspNetCore.Mvc.ProblemDetails Details(int status, string caseName, params object?[] members)
        {
            var problem = new global::Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = status,
                Title = global::Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(status),
            };
            problem.Extensions["case"] = caseName;
            for (var i = 0; i + 1 < members.Length; i += 2)
            {
                problem.Extensions[(string)members[i]!] = members[i + 1];
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
    ///     Writes a stream of unions as newline-delimited JSON with MVC's JSON options: each value case as a line, flushed per item. The first
    ///     error case ends the stream: before anything was written it answers its status as ProblemDetails; afterwards it is the last line.
    /// </summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal sealed class NdjsonUnionResult<TItem, T> : global::Microsoft.AspNetCore.Mvc.IActionResult
    {
        private static readonly byte[] NewLine = { (byte)'\n' };
        private readonly global::System.Collections.Generic.IAsyncEnumerable<TItem> _items;
        private readonly global::System.Func<TItem, ApiStreamItem<T>> _map;

        public NdjsonUnionResult(global::System.Collections.Generic.IAsyncEnumerable<TItem> items, global::System.Func<TItem, ApiStreamItem<T>> map)
        {
            _items = items;
            _map = map;
        }

        public async global::System.Threading.Tasks.Task ExecuteResultAsync(global::Microsoft.AspNetCore.Mvc.ActionContext context)
        {
            var httpContext = context.HttpContext;
            var options = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
                         .GetRequiredService<global::Microsoft.Extensions.Options.IOptions<global::Microsoft.AspNetCore.Mvc.JsonOptions>>(httpContext.RequestServices)
                         .Value.JsonSerializerOptions;
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
                        await new global::Microsoft.AspNetCore.Mvc.ObjectResult(problem) { StatusCode = problem.Status }.ExecuteResultAsync(context).ConfigureAwait(false);
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
    }

    /// <summary>Writes a stream operation as newline-delimited JSON with MVC's JSON options, flushing after every item.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal sealed class NdjsonResult<T> : global::Microsoft.AspNetCore.Mvc.IActionResult
    {
        private static readonly byte[] NewLine = { (byte)'\n' };
        private readonly global::System.Collections.Generic.IAsyncEnumerable<T> _items;

        public NdjsonResult(global::System.Collections.Generic.IAsyncEnumerable<T> items) => _items = items;

        public async global::System.Threading.Tasks.Task ExecuteResultAsync(global::Microsoft.AspNetCore.Mvc.ActionContext context)
        {
            var httpContext = context.HttpContext;
            var options = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
                         .GetRequiredService<global::Microsoft.Extensions.Options.IOptions<global::Microsoft.AspNetCore.Mvc.JsonOptions>>(httpContext.RequestServices)
                         .Value.JsonSerializerOptions;
            var typeInfo = (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<T>)options.GetTypeInfo(typeof(T));
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
    }
}
""";
}
