extern alias zeroallocserver;
using System.Net;
using System.Text;
using System.Text.Json;
using Api.Functional.ZeroAlloc.Contracts;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using ProtoBuf.Grpc.Client;
using Launches = zeroallocserver::Api.Functional.ZeroAlloc.Server;

namespace Api.Functional.Tests;

/// <summary>
///     A handler that returns a union (<c>LaunchRocketResult</c>: the rocket, <c>RocketNotFound</c> or <c>NotReady</c>) on every transport: the value
///     case answers as a plain response, each error case in the transport's own error shape, named by its case.
/// </summary>
public class UnionTests
{
    private static Task<WebApplication> Start(bool controllers = false, bool grpc = false, bool graphql = false) =>
        Launches.ServerApp.StartAsync(controllers, grpc, graphql, versioned: false, unions: true);

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Should_Answer_Union_Cases_Over_Http(bool controllers, CancellationToken cancellationToken)
    {
        await using var server = await Start(controllers);
        using var http = server.GetTestClient();
        var falcon = await Create(http, "Falcon", cancellationToken);
        var grounded = await Create(http, "Grounded", cancellationToken);
        var unknown = Guid.NewGuid();

        using var launched = await http.PostAsync($"api/launches/{falcon}", EmptyBody(), cancellationToken);
        await launched.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await ( await launched.Content.ReadFromJsonAsync<JsonElement>(cancellationToken) ).GetProperty("name").GetString().Should().BeEqualTo("Falcon");

        var notFound = await Problem(http, $"api/launches/{unknown}", HttpStatusCode.NotFound, cancellationToken);
        await notFound.GetProperty("case").GetString().Should().BeEqualTo("RocketNotFound");
        await notFound.GetProperty("status").GetInt32().Should().BeEqualTo(404);
        await notFound.GetProperty("id").GetGuid().Should().BeEqualTo(unknown);

        var notReady = await Problem(http, $"api/launches/{grounded}", HttpStatusCode.UnprocessableEntity, cancellationToken);
        await notReady.GetProperty("case").GetString().Should().BeEqualTo("NotReady");
        await notReady.GetProperty("reason").GetString().Should().BeEqualTo("grounded");
    }

    // A stream of unions streams its value cases and ends with the first error case: as the response's problem before anything was written,
    // as the last line after.
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Should_End_A_Stream_Of_Unions_With_Its_Error_Case_Over_Http(bool controllers, CancellationToken cancellationToken)
    {
        await using var server = await Start(controllers);
        using var http = server.GetTestClient();
        var falcon = await Create(http, "Falcon", cancellationToken);
        await Create(http, "Grounded", cancellationToken);

        using var values = await http.GetAsync("api/launches?name=Falcon", cancellationToken);
        await values.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await Lines(await values.Content.ReadAsStringAsync(cancellationToken)).Select(l => l.GetProperty("id").GetGuid()).Should().BeEquivalentTo([falcon], CollectionOrdering.Matching);

        using var first = await http.GetAsync("api/launches?name=Grounded", cancellationToken);
        await first.StatusCode.Should().BeEqualTo(HttpStatusCode.UnprocessableEntity);
        await first.Content.Headers.ContentType?.MediaType.Should().BeEqualTo("application/problem+json");
        await ( await first.Content.ReadFromJsonAsync<JsonElement>(cancellationToken) ).GetProperty("case").GetString().Should().BeEqualTo("NotReady");

        using var later = await http.GetAsync("api/launches", cancellationToken);
        await later.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        var lines = Lines(await later.Content.ReadAsStringAsync(cancellationToken));
        await lines.Length.Should().BeEqualTo(2);
        await lines[0].GetProperty("name").GetString().Should().BeEqualTo("Falcon");
        await lines[1].GetProperty("case").GetString().Should().BeEqualTo("NotReady");
        await lines[1].GetProperty("status").GetInt32().Should().BeEqualTo(422);
        await lines[1].GetProperty("reason").GetString().Should().BeEqualTo("grounded");
    }

    // The .proto declares the error cases' messages, so a client generated from it can decode the case-bin trailer.
    [Test]
    public async Task Should_Declare_Error_Cases_In_The_Proto()
    {
        var proto = Launches.LaunchesService.GetProto();

        await proto.Should().Contain("message RocketNotFound");
        await proto.Should().Contain("message NotReady");
        await Task.CompletedTask;
    }

    // The OpenAPI document describes each error case's status as a ProblemDetails response, so clients generated from it see them.
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Should_Describe_Union_Cases_In_OpenApi(bool controllers, CancellationToken cancellationToken)
    {
        await using var server = await Start(controllers);
        using var http = server.GetTestClient();

        var document = await http.GetFromJsonAsync<JsonElement>("openapi/v1.json", cancellationToken);
        var responses = document.GetProperty("paths").GetProperty("/api/launches/{id}").GetProperty("post").GetProperty("responses");

        await responses.EnumerateObject().Select(r => r.Name).Should().BeEquivalentTo(["200", "400", "404", "422"]);
        await responses.GetProperty("422").GetProperty("content").EnumerateObject().Select(c => c.Name).Should().Contain("application/problem+json");
    }

    [Test]
    public async Task Should_Answer_Union_Cases_Over_Grpc(CancellationToken cancellationToken)
    {
        await using var server = await Start(grpc: true);
        using var channel = GrpcChannel.ForAddress(server.GetTestServer().BaseAddress, new() { HttpHandler = server.GetTestServer().CreateHandler() });
        // The code-first contracts the server generated, so the test needs no .proto of its own.
        var rockets = channel.CreateGrpcService<Launches.IRocketsServiceContract>();
        var launches = channel.CreateGrpcService<Launches.ILaunchesServiceContract>();
        var falcon = ( await rockets.CreateRocket(new CreateRocket.Request("Falcon")) ).Id;
        var grounded = ( await rockets.CreateRocket(new CreateRocket.Request("Grounded")) ).Id;
        var unknown = Guid.NewGuid();

        await ( await launches.LaunchRocket(new LaunchRocket.Request(falcon), new(new CallOptions(cancellationToken: cancellationToken))) ).Name.Should().BeEqualTo("Falcon");

        var notFound = await ( (Func<Task>)( async () => await launches.LaunchRocket(new LaunchRocket.Request(unknown), new(new CallOptions(cancellationToken: cancellationToken))) ) ).Should().Throw<RpcException>();
        await notFound.StatusCode.Should().BeEqualTo(StatusCode.NotFound);
        await notFound.Trailers.GetValue("case").Should().BeEqualTo("RocketNotFound");
        await ProtoBuf.Serializer.Deserialize<RocketNotFound>(notFound.Trailers.GetValueBytes("case-bin")).Id.Should().BeEqualTo(unknown);

        var notReady = await ( (Func<Task>)( async () => await launches.LaunchRocket(new LaunchRocket.Request(grounded), new(new CallOptions(cancellationToken: cancellationToken))) ) ).Should().Throw<RpcException>();
        await notReady.StatusCode.Should().BeEqualTo(StatusCode.FailedPrecondition);
        await notReady.Trailers.GetValue("case").Should().BeEqualTo("NotReady");
        await ProtoBuf.Serializer.Deserialize<NotReady>(notReady.Trailers.GetValueBytes("case-bin")).Reason.Should().BeEqualTo("grounded");
    }

    [Test]
    public async Task Should_Answer_Union_Cases_Over_GraphQL(CancellationToken cancellationToken)
    {
        await using var server = await Start(graphql: true);
        using var http = server.GetTestClient();
        var falcon = await Create(http, "Falcon", cancellationToken);
        var grounded = await Create(http, "Grounded", cancellationToken);
        var unknown = Guid.NewGuid();

        await ( await Launch(http, falcon, cancellationToken) ).GetProperty("data").GetProperty("launchRocket").GetProperty("name").GetString().Should().BeEqualTo("Falcon");

        var notFound = ( await Launch(http, unknown, cancellationToken) ).GetProperty("errors")[0].GetProperty("extensions");
        await notFound.GetProperty("code").GetString().Should().BeEqualTo("NOT_FOUND");
        await notFound.GetProperty("case").GetString().Should().BeEqualTo("RocketNotFound");
        await notFound.GetProperty("id").GetGuid().Should().BeEqualTo(unknown);

        var notReady = ( await Launch(http, grounded, cancellationToken) ).GetProperty("errors")[0].GetProperty("extensions");
        await notReady.GetProperty("code").GetString().Should().BeEqualTo("UNPROCESSABLE");
        await notReady.GetProperty("case").GetString().Should().BeEqualTo("NotReady");
        await notReady.GetProperty("reason").GetString().Should().BeEqualTo("grounded");
    }

    [Test]
    public async Task Should_End_A_Stream_Of_Unions_With_Its_Error_Case_Over_Grpc(CancellationToken cancellationToken)
    {
        await using var server = await Start(grpc: true);
        using var channel = GrpcChannel.ForAddress(server.GetTestServer().BaseAddress, new() { HttpHandler = server.GetTestServer().CreateHandler() });
        var rockets = channel.CreateGrpcService<Launches.IRocketsServiceContract>();
        var launches = channel.CreateGrpcService<Launches.ILaunchesServiceContract>();
        var falcon = ( await rockets.CreateRocket(new CreateRocket.Request("Falcon")) ).Id;
        await rockets.CreateRocket(new CreateRocket.Request("Grounded"));

        var received = new List<Guid>();
        var error = await ( (Func<Task>)( async () =>
            {
                await foreach (var rocket in launches.ListLaunches(new ListLaunches.Request(null), new(new CallOptions(cancellationToken: cancellationToken)))) received.Add(rocket.Id);
            } ) ).Should().Throw<RpcException>();

        await received.Should().BeEquivalentTo([falcon], CollectionOrdering.Matching);
        await error.StatusCode.Should().BeEqualTo(StatusCode.FailedPrecondition);
        await error.Trailers.GetValue("case").Should().BeEqualTo("NotReady");
    }

    private static JsonElement[] Lines(string ndjson) =>
        [.. ndjson.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => JsonElement.Parse(line))];

    private static async Task<Guid> Create(System.Net.Http.HttpClient http, string name, CancellationToken cancellationToken)
    {
        using var created = await http.PostAsync("api/rockets", new StringContent($$"""{"name":"{{name}}"}""", Encoding.UTF8, "application/json"), cancellationToken);
        created.EnsureSuccessStatusCode();
        return ( await created.Content.ReadFromJsonAsync<JsonElement>(cancellationToken) ).GetProperty("id").GetGuid();
    }

    private static async Task<JsonElement> Problem(System.Net.Http.HttpClient http, string uri, HttpStatusCode status, CancellationToken cancellationToken)
    {
        using var response = await http.PostAsync(uri, EmptyBody(), cancellationToken);
        await response.StatusCode.Should().BeEqualTo(status);
        await response.Content.Headers.ContentType?.MediaType.Should().BeEqualTo("application/problem+json");
        return await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
    }

    // LaunchRocket is a POST: its body is the request, and the route's id is applied on top.
    private static StringContent EmptyBody() => new("{}", Encoding.UTF8, "application/json");

    private static async Task<JsonElement> Launch(System.Net.Http.HttpClient http, Guid id, CancellationToken cancellationToken)
    {
        var body = JsonSerializer.Serialize(
            new
            {
                query = "mutation($request: LaunchRocketRequest!) { launchRocket(request: $request) { id name } }",
                variables = new { request = new { id } },
            }
        );
        using var response = await http.PostAsync("graphql", new StringContent(body, Encoding.UTF8, "application/json"), cancellationToken);
        return await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
    }
}
