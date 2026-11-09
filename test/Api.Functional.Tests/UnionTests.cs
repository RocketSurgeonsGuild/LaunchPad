extern alias mediatrserver;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Api.Functional.MediatR.Contracts;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using ProtoBuf.Grpc.Client;
using Launches = mediatrserver::Api.Functional.MediatR.Server;

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
        launched.StatusCode.ShouldBe(HttpStatusCode.OK);
        ( await launched.Content.ReadFromJsonAsync<JsonElement>(cancellationToken) ).GetProperty("name").GetString().ShouldBe("Falcon");

        var notFound = await Problem(http, $"api/launches/{unknown}", HttpStatusCode.NotFound, cancellationToken);
        notFound.GetProperty("case").GetString().ShouldBe("RocketNotFound");
        notFound.GetProperty("status").GetInt32().ShouldBe(404);
        notFound.GetProperty("id").GetGuid().ShouldBe(unknown);

        var notReady = await Problem(http, $"api/launches/{grounded}", HttpStatusCode.UnprocessableEntity, cancellationToken);
        notReady.GetProperty("case").GetString().ShouldBe("NotReady");
        notReady.GetProperty("reason").GetString().ShouldBe("grounded");
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
        values.StatusCode.ShouldBe(HttpStatusCode.OK);
        Lines(await values.Content.ReadAsStringAsync(cancellationToken)).Select(l => l.GetProperty("id").GetGuid()).ShouldBe([falcon]);

        using var first = await http.GetAsync("api/launches?name=Grounded", cancellationToken);
        first.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        first.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
        ( await first.Content.ReadFromJsonAsync<JsonElement>(cancellationToken) ).GetProperty("case").GetString().ShouldBe("NotReady");

        using var later = await http.GetAsync("api/launches", cancellationToken);
        later.StatusCode.ShouldBe(HttpStatusCode.OK);
        var lines = Lines(await later.Content.ReadAsStringAsync(cancellationToken));
        lines.Length.ShouldBe(2);
        lines[0].GetProperty("name").GetString().ShouldBe("Falcon");
        lines[1].GetProperty("case").GetString().ShouldBe("NotReady");
        lines[1].GetProperty("status").GetInt32().ShouldBe(422);
        lines[1].GetProperty("reason").GetString().ShouldBe("grounded");
    }

    // The .proto declares the error cases' messages, so a client generated from it can decode the case-bin trailer.
    [Test]
    public async Task Should_Declare_Error_Cases_In_The_Proto()
    {
        var proto = Launches.LaunchesService.GetProto();

        proto.ShouldContain("message RocketNotFound");
        proto.ShouldContain("message NotReady");
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

        responses.EnumerateObject().Select(r => r.Name).ShouldBe(["200", "400", "404", "422"], ignoreOrder: true);
        responses.GetProperty("422").GetProperty("content").EnumerateObject().Select(c => c.Name).ShouldContain("application/problem+json");
    }

    [Test]
    public async Task Should_Answer_Union_Cases_Over_Grpc(CancellationToken cancellationToken)
    {
        await using var server = await Start(grpc: true);
        using var channel = GrpcChannel.ForAddress(server.GetTestServer().BaseAddress, new() { HttpHandler = server.GetTestServer().CreateHandler() });
        // The code-first contracts the server generated, so the test needs no .proto of its own.
        var rockets = channel.CreateGrpcService<Launches.IRocketsServiceContract>();
        var launches = channel.CreateGrpcService<Launches.ILaunchesServiceContract>();
        var falcon = ( await rockets.CreateRocket(new("Falcon")) ).Id;
        var grounded = ( await rockets.CreateRocket(new("Grounded")) ).Id;
        var unknown = Guid.NewGuid();

        ( await launches.LaunchRocket(new(falcon), new(new CallOptions(cancellationToken: cancellationToken))) ).Name.ShouldBe("Falcon");

        var notFound = await Should.ThrowAsync<RpcException>(async () => await launches.LaunchRocket(new(unknown), new(new CallOptions(cancellationToken: cancellationToken))));
        notFound.StatusCode.ShouldBe(StatusCode.NotFound);
        notFound.Trailers.GetValue("case").ShouldBe("RocketNotFound");
        ProtoBuf.Serializer.Deserialize<RocketNotFound>(notFound.Trailers.GetValueBytes("case-bin")).Id.ShouldBe(unknown);

        var notReady = await Should.ThrowAsync<RpcException>(async () => await launches.LaunchRocket(new(grounded), new(new CallOptions(cancellationToken: cancellationToken))));
        notReady.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
        notReady.Trailers.GetValue("case").ShouldBe("NotReady");
        ProtoBuf.Serializer.Deserialize<NotReady>(notReady.Trailers.GetValueBytes("case-bin")).Reason.ShouldBe("grounded");
    }

    [Test]
    public async Task Should_Answer_Union_Cases_Over_GraphQL(CancellationToken cancellationToken)
    {
        await using var server = await Start(graphql: true);
        using var http = server.GetTestClient();
        var falcon = await Create(http, "Falcon", cancellationToken);
        var grounded = await Create(http, "Grounded", cancellationToken);
        var unknown = Guid.NewGuid();

        ( await Launch(http, falcon, cancellationToken) ).GetProperty("data").GetProperty("launchRocket").GetProperty("name").GetString().ShouldBe("Falcon");

        var notFound = ( await Launch(http, unknown, cancellationToken) ).GetProperty("errors")[0].GetProperty("extensions");
        notFound.GetProperty("code").GetString().ShouldBe("NOT_FOUND");
        notFound.GetProperty("case").GetString().ShouldBe("RocketNotFound");
        notFound.GetProperty("id").GetGuid().ShouldBe(unknown);

        var notReady = ( await Launch(http, grounded, cancellationToken) ).GetProperty("errors")[0].GetProperty("extensions");
        notReady.GetProperty("code").GetString().ShouldBe("UNPROCESSABLE");
        notReady.GetProperty("case").GetString().ShouldBe("NotReady");
        notReady.GetProperty("reason").GetString().ShouldBe("grounded");
    }

    [Test]
    public async Task Should_End_A_Stream_Of_Unions_With_Its_Error_Case_Over_Grpc(CancellationToken cancellationToken)
    {
        await using var server = await Start(grpc: true);
        using var channel = GrpcChannel.ForAddress(server.GetTestServer().BaseAddress, new() { HttpHandler = server.GetTestServer().CreateHandler() });
        var rockets = channel.CreateGrpcService<Launches.IRocketsServiceContract>();
        var launches = channel.CreateGrpcService<Launches.ILaunchesServiceContract>();
        var falcon = ( await rockets.CreateRocket(new("Falcon")) ).Id;
        await rockets.CreateRocket(new("Grounded"));

        var received = new List<Guid>();
        var error = await Should.ThrowAsync<RpcException>(
            async () =>
            {
                await foreach (var rocket in launches.ListLaunches(new(null), new(new CallOptions(cancellationToken: cancellationToken)))) received.Add(rocket.Id);
            }
        );

        received.ShouldBe([falcon]);
        error.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
        error.Trailers.GetValue("case").ShouldBe("NotReady");
    }

    private static JsonElement[] Lines(string ndjson) =>
        [.. ndjson.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => JsonDocument.Parse(line).RootElement)];

    private static async Task<Guid> Create(System.Net.Http.HttpClient http, string name, CancellationToken cancellationToken)
    {
        using var created = await http.PostAsync("api/rockets", new StringContent($$"""{"name":"{{name}}"}""", Encoding.UTF8, "application/json"), cancellationToken);
        created.EnsureSuccessStatusCode();
        return ( await created.Content.ReadFromJsonAsync<JsonElement>(cancellationToken) ).GetProperty("id").GetGuid();
    }

    private static async Task<JsonElement> Problem(System.Net.Http.HttpClient http, string uri, HttpStatusCode status, CancellationToken cancellationToken)
    {
        using var response = await http.PostAsync(uri, EmptyBody(), cancellationToken);
        response.StatusCode.ShouldBe(status);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
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
