extern alias channelmediatorserver;
extern alias foundatioserver;
extern alias immediatehandlersserver;
extern alias mediatorserver;
extern alias zeroallocserver;
using System.Net;
using System.Text;
using System.Text.Json;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;
using StrawberryShake;
using Http = Api.Functional.Clients.Http;
using Proto = Rockets;

namespace Api.Functional.Tests;

/// <summary>
///     For each mediator, a server built by the generators, reached through the clients an application would generate from its specs: Kiota
///     (Chirograph's OpenApiClient) from the OpenAPI document, Grpc.Tools from the <c>.proto</c> (<c>RocketsService.GetProto()</c>) and StrawberryShake from the GraphQL
///     schema. The clients are generated once, from the ZeroAlloc.Mediator server's specs; <see cref="Should_Expose_The_Same_Specs" /> checks that every
///     other server exposes the same ones.
/// </summary>
public class RoundTripTests
{
    private const string Reference = "ZeroAlloc";

    // Only the ZeroAlloc server, the reference, maps the versioned hosts.
    private static readonly Dictionary<string, (Func<bool, bool, bool, bool, Task<WebApplication>> Start, Func<string> Proto)> Servers = new()
    {
        ["Mediator"] = ((controllers, grpc, graphql, _) => mediatorserver::Api.Functional.Mediator.Server.ServerApp.StartAsync(controllers, grpc, graphql), mediatorserver::Api.Functional.Mediator.Server.RocketsService.GetProto),
        ["ChannelMediator"] = ((controllers, grpc, graphql, _) => channelmediatorserver::Api.Functional.ChannelMediator.Server.ServerApp.StartAsync(controllers, grpc, graphql), channelmediatorserver::Api.Functional.ChannelMediator.Server.RocketsService.GetProto),
        ["ImmediateHandlers"] = ((controllers, grpc, graphql, _) => immediatehandlersserver::Api.Functional.ImmediateHandlers.Server.ServerApp.StartAsync(controllers, grpc, graphql), immediatehandlersserver::Api.Functional.ImmediateHandlers.Server.RocketsService.GetProto),
        ["Foundatio"] = ((controllers, grpc, graphql, _) => foundatioserver::Api.Functional.Foundatio.Server.ServerApp.StartAsync(controllers, grpc, graphql), foundatioserver::Api.Functional.Foundatio.Server.RocketsService.GetProto),
        ["ZeroAlloc"] = ((controllers, grpc, graphql, versioned) => zeroallocserver::Api.Functional.ZeroAlloc.Server.ServerApp.StartAsync(controllers, grpc, graphql, versioned), zeroallocserver::Api.Functional.ZeroAlloc.Server.RocketsService.GetProto),
    };

    [Test]
    [Arguments("Mediator", false)]
    [Arguments("Mediator", true)]
    [Arguments("ChannelMediator", false)]
    [Arguments("ChannelMediator", true)]
    [Arguments("ImmediateHandlers", false)]
    [Arguments("ImmediateHandlers", true)]
    [Arguments("Foundatio", false)]
    [Arguments("Foundatio", true)]
    [Arguments("ZeroAlloc", false)]
    [Arguments("ZeroAlloc", true)]
    public async Task Should_Round_Trip_Over_Http(string mediator, bool controllers, CancellationToken cancellationToken)
    {
        await using var server = await Servers[mediator].Start(controllers, false, false, false);
        var testServer = server.GetTestServer();
        // the registration Chirograph generates for the client (AddRocketsClient), sent to the test server
        var services = new ServiceCollection();
        // called through the alias: a using of the generated namespace looks unused to dotnet format, whose design-time build generates nothing
        Http.RocketsClientServiceCollectionExtensions
            .AddRocketsClient(services, options => options.BaseAddress = testServer.BaseAddress.ToString())
            .ConfigurePrimaryHttpMessageHandler(testServer.CreateHandler);
        await using var provider = services.BuildServiceProvider();
        var rockets = provider.GetRequiredService<Http.RocketsClient>().Api.Rockets;

        var id = ( await rockets.PostAsync(new() { Name = "Falcon" }, cancellationToken: cancellationToken) )!.Id!.Value;
        await Rocket(await rockets[id].GetAsync(cancellationToken: cancellationToken)).Should().BeEqualTo((id, "Falcon"));

        await Rocket(await rockets[id].PutAsync(new() { Name = "Falcon 9" }, cancellationToken: cancellationToken)).Should().BeEqualTo((id, "Falcon 9"));
        var other = ( await rockets.PostAsync(new() { Name = "Starship" }, cancellationToken: cancellationToken) )!.Id!.Value;
        // ListRockets streams application/x-ndjson, which Kiota hands over as the stream: one JSON rocket per line.
        await using var stream = await rockets.GetAsync(request => request.QueryParameters.Name = "Falcon 9", cancellationToken);
        using var reader = new StreamReader(stream!);
        var listed = ( await reader.ReadToEndAsync(cancellationToken) )
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => Rocket(JsonSerializer.Deserialize<Http.Models.RocketModel>(line, JsonSerializerOptions.Web)));
        await listed.Should().BeEquivalentTo([(id, "Falcon 9")], CollectionOrdering.Matching);

        await rockets[id].DeleteAsync(cancellationToken: cancellationToken);
        await ( await ( (Func<Task>)( () => rockets[id].GetAsync(cancellationToken: cancellationToken) ) ).Should().Throw<ApiException>() ).ResponseStatusCode.Should().BeEqualTo(404);
        await Rocket(await rockets[other].GetAsync(cancellationToken: cancellationToken)).Should().BeEqualTo((other, "Starship"));
    }

    [Test]
    [Arguments("Mediator", false)]
    [Arguments("Mediator", true)]
    [Arguments("ChannelMediator", false)]
    [Arguments("ChannelMediator", true)]
    [Arguments("ImmediateHandlers", false)]
    [Arguments("ImmediateHandlers", true)]
    [Arguments("Foundatio", false)]
    [Arguments("Foundatio", true)]
    [Arguments("ZeroAlloc", false)]
    [Arguments("ZeroAlloc", true)]
    public async Task Should_Answer_Created_With_A_Location(string mediator, bool controllers, CancellationToken cancellationToken)
    {
        await using var server = await Servers[mediator].Start(controllers, false, false, false);
        using var http = server.GetTestClient();

        using var response = await http.PostAsync("api/rockets", new StringContent("""{"name":"Falcon"}""", Encoding.UTF8, "application/json"), cancellationToken);

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.Created);
        await ( await response.Headers.Location.Should().NotBeNull() )!.ToString().Should().StartWith("/api/rockets/");
        using var location = await http.GetAsync(response.Headers.Location, cancellationToken);
        await location.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
    }

    [Test]
    [Arguments("Mediator")]
    [Arguments("ChannelMediator")]
    [Arguments("ImmediateHandlers")]
    [Arguments("Foundatio")]
    [Arguments("ZeroAlloc")]
    public async Task Should_Round_Trip_Over_Grpc(string mediator, CancellationToken cancellationToken)
    {
        await using var server = await Servers[mediator].Start(false, true, false, false);
        using var channel = GrpcChannel.ForAddress(server.GetTestServer().BaseAddress, new() { HttpHandler = server.GetTestServer().CreateHandler() });
        var rockets = new Proto.Rockets.RocketsClient(channel);

        var id = ( await rockets.CreateRocketAsync(new() { Name = "Falcon" }, cancellationToken: cancellationToken) ).Id;
        await Rocket(await rockets.GetRocketAsync(new() { Id = id }, cancellationToken: cancellationToken)).Should().BeEqualTo((Guid.Parse(id), "Falcon"));
        await Rocket(await rockets.EditRocketAsync(new() { Id = id, Name = "Falcon 9" }, cancellationToken: cancellationToken)).Should().BeEqualTo((Guid.Parse(id), "Falcon 9"));
        using var list = rockets.ListRockets(new() { Name = "Falcon 9" }, cancellationToken: cancellationToken);
        await ( await list.ResponseStream.ReadAllAsync(cancellationToken).Select(Rocket).ToListAsync(cancellationToken) ).Should().BeEquivalentTo([(Guid.Parse(id), "Falcon 9")], CollectionOrdering.Matching);

        await rockets.DeleteRocketAsync(new() { Id = id }, cancellationToken: cancellationToken);
        await ( await ( (Func<Task>)( async () => await rockets.GetRocketAsync(new() { Id = id }, cancellationToken: cancellationToken) ) ).Should().Throw<RpcException>() ).StatusCode.Should().BeEqualTo(StatusCode.NotFound);
    }

    [Test]
    [Arguments("Mediator")]
    [Arguments("ChannelMediator")]
    [Arguments("ImmediateHandlers")]
    [Arguments("Foundatio")]
    [Arguments("ZeroAlloc")]
    public async Task Should_Send_Mutations_Over_GraphQL(string mediator, CancellationToken cancellationToken)
    {
        await using var server = await Servers[mediator].Start(false, false, true, false);
        var services = new ServiceCollection();
        services.AddRocketsGraphQLClient()
                .ConfigureHttpClient(
                     client => client.BaseAddress = new(server.GetTestServer().BaseAddress, "graphql"),
                     builder => builder.ConfigurePrimaryHttpMessageHandler(() => server.GetTestServer().CreateHandler())
                 );
        await using var provider = services.BuildServiceProvider();
        // StrawberryShake's types are written into obj by a build target, so `dotnet format` (which loads the solution without building) can't
        // see them and removes a using of their namespace; qualified names don't depend on one.
        var rockets = provider.GetRequiredService<GraphQLClient.IRocketsGraphQLClient>();
        using var http = server.GetTestClient();

        var created = await rockets.CreateRocket.ExecuteAsync(new GraphQLClient.CreateRocketRequest { Name = "Falcon" }, cancellationToken);
        created.EnsureNoErrors();
        var id = created.Data!.CreateRocket.Id;
        var edited = await rockets.EditRocket.ExecuteAsync(new GraphQLClient.EditRocketRequest { Id = id, Name = "Falcon 9" }, cancellationToken);
        edited.EnsureNoErrors();
        await (edited.Data!.EditRocket.Id, edited.Data.EditRocket.Name).Should().BeEqualTo((id, "Falcon 9"));
        await ( await http.GetStringAsync($"api/rockets/{id}", cancellationToken) ).Should().Contain("Falcon 9");

        var deleted = await rockets.DeleteRocket.ExecuteAsync(new GraphQLClient.DeleteRocketRequest { Id = id }, cancellationToken);
        deleted.EnsureNoErrors();
        await ( await http.GetAsync($"api/rockets/{id}", cancellationToken) ).StatusCode.Should().BeEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Should_Map_Api_Versions(bool controllers, CancellationToken cancellationToken)
    {
        await using var server = await Servers[Reference].Start(controllers, false, false, true);
        using var http = server.GetTestClient();

        // CreateVersionedRocket is in 1.0 and GetVersionedRocket in 2.0.
        using var created = await http.PostAsync("api/versioned-rockets/rocket?api-version=1.0", new StringContent("""{"name":"Falcon"}""", Encoding.UTF8, "application/json"), cancellationToken);
        created.EnsureSuccessStatusCode();
        var id = JsonElement.Parse(await created.Content.ReadAsStringAsync(cancellationToken)).GetProperty("id").GetGuid();
        await ( await http.GetStringAsync($"api/versioned-rockets/{id}/rocket?api-version=2.0", cancellationToken) ).Should().Contain("Falcon");

        // GetVersionedRocket isn't in 1.0, 3.0 isn't a version at all, and a request without a version names none: Asp.Versioning answers 400 to each.
        await ( await http.GetAsync($"api/versioned-rockets/{id}/rocket?api-version=1.0", cancellationToken) ).StatusCode.Should().BeEqualTo(HttpStatusCode.BadRequest);
        await ( await http.GetAsync($"api/versioned-rockets/{id}/rocket?api-version=3.0", cancellationToken) ).StatusCode.Should().BeEqualTo(HttpStatusCode.BadRequest);
        await ( await http.GetAsync($"api/versioned-rockets/{id}/rocket", cancellationToken) ).StatusCode.Should().BeEqualTo(HttpStatusCode.BadRequest);
    }

    /// <summary>The clients above were generated from the ZeroAlloc.Mediator server's specs; each mediator's server must expose the same ones.</summary>
    [Test]
    [Arguments("Mediator")]
    [Arguments("ChannelMediator")]
    [Arguments("ImmediateHandlers")]
    [Arguments("Foundatio")]
    public async Task Should_Expose_The_Same_Specs(string mediator, CancellationToken cancellationToken)
    {
        await ( await OpenApi(mediator, false, cancellationToken) ).Should().BeEqualTo(await OpenApi(Reference, false, cancellationToken));
        await ( await OpenApi(mediator, true, cancellationToken) ).Should().BeEqualTo(await OpenApi(Reference, true, cancellationToken));
        await ( await GraphQLSchema(mediator, cancellationToken) ).Should().BeEqualTo(await GraphQLSchema(Reference, cancellationToken));
        await Servers[mediator].Proto().Should().BeEqualTo(Servers[Reference].Proto());
    }

    private static async Task<string> OpenApi(string mediator, bool controllers, CancellationToken cancellationToken)
    {
        await using var server = await Servers[mediator].Start(controllers, false, false, false);
        using var http = server.GetTestClient();
        return await http.GetStringAsync("openapi/v1.json", cancellationToken);
    }

    private static async Task<string> GraphQLSchema(string mediator, CancellationToken cancellationToken)
    {
        await using var server = await Servers[mediator].Start(false, false, true, false);
        using var http = server.GetTestClient();
        return await http.GetStringAsync("graphql?sdl", cancellationToken);
    }

    private static (Guid Id, string? Name) Rocket(Http.Models.RocketModel? rocket) => (rocket!.Id!.Value, rocket.Name);
    private static (Guid Id, string Name) Rocket(Proto.RocketModel rocket) => (Guid.Parse(rocket.Id), rocket.Name);
}
