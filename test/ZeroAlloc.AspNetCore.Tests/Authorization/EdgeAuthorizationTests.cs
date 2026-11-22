using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;

using Joinery.Dovetail;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.Authorization;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.Authorization;
using Rocket.Surgery.LaunchPad.Authorization.Generated;
using Rocket.Surgery.LaunchPad.Foundation;

using ZeroAlloc.Authorization;
using ZeroAlloc.Mediator;
using ZeroAlloc.Results;

namespace ZeroAlloc.AspNetCore.Tests.Authorization;

public class EdgeAuthorizationTests
{
    [Test]
    public async Task A_Caller_With_No_Identity_Is_Challenged_With_The_Unauthorized_Case_And_The_Scopes()
    {
        await using var app = await Start();
        using var client = app.GetTestClient();

        using var response = await client.GetAsync(new Uri("/launches/1", UriKind.Relative), CancellationToken.None);

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.Unauthorized);
        await response.Headers.WwwAuthenticate.ToString().Should().BeEqualTo("Bearer scope=\"launches:read\"");
        await response.Content.Headers.ContentType!.MediaType.Should().BeEqualTo("application/problem+json");
        var problem = JsonNode.Parse(await response.Content.ReadAsStringAsync(CancellationToken.None))!;
        await problem["case"]!.GetValue<string>().Should().BeEqualTo("Unauthorized");
        await problem["status"]!.GetValue<int>().Should().BeEqualTo(401);
        await app.Services.GetRequiredService<Calls>().Handled.Should().BeEqualTo(0);
    }

    [Test]
    public async Task A_Caller_Without_The_Scope_Is_Forbidden_With_The_Forbidden_Case_Before_The_Request_Is_Dispatched()
    {
        await using var app = await Start();
        using var client = app.GetTestClient();

        using var response = await Send(client, "/launches/1", "alice", "launches:write");

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.Forbidden);
        await response.Headers.WwwAuthenticate.ToString().Should().BeEqualTo("Bearer error=\"insufficient_scope\", scope=\"launches:read\"");
        var problem = JsonNode.Parse(await response.Content.ReadAsStringAsync(CancellationToken.None))!;
        await problem["case"]!.GetValue<string>().Should().BeEqualTo("Forbidden");
        await problem["missingScopes"]!.GetValue<string>().Should().BeEqualTo("launches:read");
        await app.Services.GetRequiredService<Calls>().Handled.Should().BeEqualTo(0);
    }

    [Test]
    public async Task A_Policy_Runs_Once_For_The_Edge_And_The_Pipeline()
    {
        await using var app = await Start();
        using var client = app.GetTestClient();

        using var response = await Send(client, "/launches/1", "alice", "launches:read");

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        var calls = app.Services.GetRequiredService<Calls>();
        await calls.Handled.Should().BeEqualTo(1);
        await calls.Policy.Should().BeEqualTo(1);
    }

    [Test]
    public async Task Every_Policy_Of_An_Endpoint_Must_Pass_And_The_Challenge_Names_The_Scopes_Of_Them_All()
    {
        await using var app = await Start();
        using var client = app.GetTestClient();

        using var anonymous = await client.GetAsync(new Uri("/launches/1/cancel", UriKind.Relative), CancellationToken.None);
        using var reader = await Send(client, "/launches/1/cancel", "alice", "launches:read");
        using var both = await Send(client, "/launches/1/cancel", "alice", "launches:read launches:write");

        await anonymous.StatusCode.Should().BeEqualTo(HttpStatusCode.Unauthorized);
        await anonymous.Headers.WwwAuthenticate.ToString().Should().BeEqualTo("Bearer scope=\"launches:read launches:write\"");
        await reader.StatusCode.Should().BeEqualTo(HttpStatusCode.Forbidden);
        await reader.Headers.WwwAuthenticate.ToString().Should().BeEqualTo("Bearer error=\"insufficient_scope\", scope=\"launches:write\"");
        await both.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task A_Group_Of_Alternatives_Requires_A_Caller_At_The_Edge_And_Is_Decided_In_The_Pipeline()
    {
        await using var app = await Start();
        using var client = app.GetTestClient();

        using var anonymous = await client.PostAsync(new Uri("/launches/1/abort", UriKind.Relative), null, CancellationToken.None);
        using var reader = await Send(client, "/launches/1/abort", "alice", "launches:read", HttpMethod.Post);
        using var writer = await Send(client, "/launches/1/abort", "alice", "launches:write", HttpMethod.Post);

        await anonymous.StatusCode.Should().BeEqualTo(HttpStatusCode.Unauthorized);
        await JsonNode.Parse(await anonymous.Content.ReadAsStringAsync(CancellationToken.None))!["case"]!.GetValue<string>().Should().BeEqualTo("Unauthorized");
        await reader.StatusCode.Should().BeEqualTo(HttpStatusCode.Forbidden);
        await JsonNode.Parse(await reader.Content.ReadAsStringAsync(CancellationToken.None))!["case"]!.GetValue<string>().Should().BeEqualTo("Forbidden");
        await writer.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task An_Application_Policy_Is_Left_To_The_Application()
    {
        await using var app = await Start();
        using var client = app.GetTestClient();

        using var anonymous = await client.GetAsync(new Uri("/own", UriKind.Relative), CancellationToken.None);
        using var known = await Send(client, "/own", "alice", null);

        await anonymous.StatusCode.Should().BeEqualTo(HttpStatusCode.Unauthorized);
        await ( await anonymous.Content.ReadAsStringAsync(CancellationToken.None) ).Should().BeEmpty();
        await known.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task A_Grpc_Call_Is_Rejected_With_Its_Status_And_Case_As_Trailers()
    {
        await using var app = await Start();
        using var client = app.GetTestClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/launches/grpc", UriKind.Relative)) { Content = new ByteArrayContent([]) };
        request.Content.Headers.ContentType = new("application/grpc");

        using var response = await client.SendAsync(request, CancellationToken.None);

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await response.Headers.GetValues("grpc-status").Single().Should().BeEqualTo("16");
        await response.Headers.GetValues("case").Single().Should().BeEqualTo("Unauthorized");
        await response.Headers.WwwAuthenticate.ToString().Should().BeEqualTo("Bearer scope=\"launches:read\"");
    }

    [Test]
    public async Task The_Document_States_The_Scopes_Of_An_Operation_And_Declares_The_Scheme_When_Given_Its_Endpoints()
    {
        await using var app = await Start(
            options =>
            {
                options.AuthorizationUrl = new("https://id.example/authorize");
                options.TokenUrl = new("https://id.example/token");
                options.ScopeDescriptions["launches:read"] = "Read launches";
            }
        );
        using var client = app.GetTestClient();

        var document = JsonNode.Parse(await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), CancellationToken.None))!;

        await document["paths"]!["/launches/{id}"]!["get"]!["security"]!.ToJsonString().Should().BeEqualTo("""[{"oauth2":["launches:read"]}]""");
        await document["paths"]!["/launches/{id}/abort"]!["post"]!["security"]!.ToJsonString().Should().BeEqualTo("""[{"oauth2":["launches:write"]},{"oauth2":["launches:admin","launches:read"]}]""");
        await ( document["paths"]!["/own"]!["get"]!["security"] is null ).Should().BeTrue();
        var scheme = document["components"]!["securitySchemes"]!["oauth2"]!;
        await scheme["type"]!.GetValue<string>().Should().BeEqualTo("oauth2");
        var flow = scheme["flows"]!["authorizationCode"]!;
        await flow["authorizationUrl"]!.GetValue<string>().Should().BeEqualTo("https://id.example/authorize");
        await flow["scopes"]!.ToJsonString().Should().BeEqualTo("""{"launches:admin":"launches:admin","launches:read":"Read launches","launches:write":"launches:write"}""");
    }

    [Test]
    public async Task The_Document_Declares_A_Bearer_Scheme_Without_The_Endpoints()
    {
        await using var app = await Start();
        using var client = app.GetTestClient();

        var document = JsonNode.Parse(await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), CancellationToken.None))!;

        var scheme = document["components"]!["securitySchemes"]!["oauth2"]!;
        await scheme["type"]!.GetValue<string>().Should().BeEqualTo("http");
        await scheme["scheme"]!.GetValue<string>().Should().BeEqualTo("bearer");
        await document["paths"]!["/launches/{id}"]!["get"]!["security"]!.ToJsonString().Should().BeEqualTo("""[{"oauth2":["launches:read"]}]""");
    }

    [Test]
    [Arguments(null, "Bearer scope=\"a b\"")]
    [Arguments("Bearer", "Bearer scope=\"a b\"")]
    [Arguments("Bearer error=\"invalid_token\"", "Bearer error=\"invalid_token\", scope=\"a b\"")]
    [Arguments("Basic realm=\"x\"", "Basic realm=\"x\"")]
    public async Task The_Challenge_Keeps_What_The_Scheme_Wrote(string? existing, string expected)
    {
        var decision = new AuthorizationDecision(new Unauthorized(), ["a", "b"]);

        await AuthorizationProblems.Challenge(decision, existing).Should().BeEqualTo(expected);
    }

    [Test]
    public async Task A_Principal_Becomes_A_Security_Context_With_Its_Subject_Roles_And_Claims()
    {
        var identity = new ClaimsIdentity(
            [new("sub", "alice"), new(ClaimTypes.Role, "admin"), new("roles", "pilot"), new("scope", "a"), new("scope", "b")],
            "test"
        );

        var context = ClaimsSecurityContext.From(new(identity));

        await context.Id.Should().BeEqualTo("alice");
        await context.Roles.Should().BeEquivalentTo(["admin", "pilot"]);
        await context.Claims["scope"].Should().BeEqualTo("a b");
        await ( ClaimsSecurityContext.From(new(new ClaimsIdentity())) is AnonymousSecurityContext ).Should().BeTrue();
    }

    private static async Task<HttpResponseMessage> Send(HttpClient client, string path, string user, string? scope, HttpMethod? method = null)
    {
        using var request = new HttpRequestMessage(method ?? HttpMethod.Get, new Uri(path, UriKind.Relative));
        request.Headers.Add("X-User", user);
        if (scope is { }) request.Headers.Add("X-Scope", scope);
        return await client.SendAsync(request, CancellationToken.None);
    }

    private static async Task<WebApplication> Start(Action<OpenApiAuthorizationOptions>? configure = null)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddOpenApi();
        builder.Services.AddMediator();
        // what the joint LaunchPad writes into a project applies: the plans the generator wrote for the requests of this assembly
        builder.Services.AddLaunchPadRequestAuthorization();
        builder.Services.AddSingleton<Calls>();
        builder.Services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, HeaderAuthentication>("Test", null);
        builder.Services.AddAuthorizationBuilder().AddPolicy("Own", policy => policy.RequireAuthenticatedUser());
        new ZeroAllocAuthorizationJoint().Register(
            await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), [])),
            builder.Services
        );
        if (configure is { }) builder.Services.Configure(configure);

        var app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapOpenApi();
        // what the generated servers put on an operation: each policy of the request by name, and a transformer that states its scopes
        app.MapGet("/launches/{id}", async (string id, IMediator mediator) => Answer(await mediator.Send(new GetLaunch(id), CancellationToken.None)))
           .RequireAuthorization("Launches.Read")
           .AddOpenApiOperationTransformer(Scopes<GetLaunch>);
        app.MapGet("/launches/{id}/cancel", async (string id, IMediator mediator) => Answer(await mediator.Send(new CancelLaunch(id), CancellationToken.None)))
           .RequireAuthorization("Launches.Read")
           .RequireAuthorization("Launches.Write")
           .AddOpenApiOperationTransformer(Scopes<CancelLaunch>);
        // a group of alternatives is left to the pipeline: the endpoint only requires a caller
        app.MapPost("/launches/{id}/abort", async (string id, IMediator mediator) => Answer(await mediator.Send(new AbortLaunch(id), CancellationToken.None)))
           .RequireAuthorization()
           .WithMetadata(new System.CodeDom.Compiler.GeneratedCodeAttribute("Rocket.Surgery.LaunchPad.Api", "test"))
           .AddOpenApiOperationTransformer(Scopes<AbortLaunch>);
        app.MapPost("/launches/grpc", () => TypedResults.Ok()).RequireAuthorization("Launches.Read").AddOpenApiOperationTransformer(Scopes<GetLaunch>);
        app.MapGet("/own", () => TypedResults.Ok()).RequireAuthorization("Own");
        await app.StartAsync();
        return app;
    }

    private static Task Scopes<TRequest>(Microsoft.OpenApi.OpenApiOperation operation, Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext context, CancellationToken _)
    {
        OpenApiAuthorization.RequireScopes<TRequest>(operation, context);
        return Task.CompletedTask;
    }

    private static IResult Answer(LaunchResult result) => result.Value switch
    {
        Launch launch => TypedResults.Ok(launch.Id),
        Problem problem => TypedResults.Problem(AuthorizationProblems.Details(problem)),
        _ => TypedResults.StatusCode(500),
    };

    private sealed class HeaderAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers["X-User"].ToString() is not { Length: > 0 } user) return Task.FromResult(AuthenticateResult.NoResult());
            var claims = new List<Claim> { new("sub", user) };
            if (Request.Headers["X-Scope"].ToString() is { Length: > 0 } scope) claims.Add(new("scope", scope));
            return Task.FromResult(AuthenticateResult.Success(new(new(new ClaimsIdentity(claims, Scheme.Name)), Scheme.Name)));
        }
    }
}

public sealed class Calls
{
    private int _handled;
    private int _policy;
    public int Handled => _handled;
    public int Policy => _policy;
    public void Handle() => Interlocked.Increment(ref _handled);
    public void Evaluate() => Interlocked.Increment(ref _policy);
}

[Policy("Launches.Read"), RequiresScopes("launches:read")]
public sealed class LaunchesRead(Calls calls) : IAuthorizationPolicy
{
    public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, CancellationToken ct = default)
    {
        calls.Evaluate();
        return new(UnitResult<AuthorizationFailure>.Success());
    }
}

[Policy("Launches.Write"), RequiresScopes("launches:write")]
public sealed class LaunchesWrite : IAuthorizationPolicy
{
    public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, CancellationToken ct = default) => new(UnitResult<AuthorizationFailure>.Success());
}

[Policy("Launches.Admin"), RequiresScopes("launches:admin", "launches:read")]
public sealed class LaunchesAdmin : IAuthorizationPolicy
{
    public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, CancellationToken ct = default) => new(UnitResult<AuthorizationFailure>.Success());
}

public sealed record Launch(string Id);

public union LaunchResult(Launch, Unauthorized, Forbidden);

[RequirePolicy("Launches.Read")]
public readonly record struct GetLaunch(string Id) : IRequest<LaunchResult>;

[RequirePolicy("Launches.Read")]
[RequirePolicy("Launches.Write")]
public readonly record struct CancelLaunch(string Id) : IRequest<LaunchResult>;

[RequireAnyPolicy("Launches.Write", "Launches.Admin")]
public readonly record struct AbortLaunch(string Id) : IRequest<LaunchResult>;

public class LaunchHandlers(Calls calls) : IRequestHandler<GetLaunch, LaunchResult>, IRequestHandler<CancelLaunch, LaunchResult>, IRequestHandler<AbortLaunch, LaunchResult>
{
    public ValueTask<LaunchResult> Handle(GetLaunch request, CancellationToken ct)
    {
        calls.Handle();
        return new(new Launch(request.Id));
    }

    public ValueTask<LaunchResult> Handle(CancelLaunch request, CancellationToken ct) => new(new Launch(request.Id));
    public ValueTask<LaunchResult> Handle(AbortLaunch request, CancellationToken ct) => new(new Launch(request.Id));
}
