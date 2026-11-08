extern alias channelmediatorclient;
extern alias channelmediatorgraphqlclient;
extern alias channelmediatorgrpcclient;
extern alias channelmediatorserver;
extern alias foundatioclient;
extern alias foundatiographqlclient;
extern alias foundatiogrpcclient;
extern alias foundatioserver;
extern alias immediatehandlersclient;
extern alias immediatehandlersgraphqlclient;
extern alias immediatehandlersgrpcclient;
extern alias immediatehandlersserver;
extern alias mediatorclient;
extern alias mediatorgraphqlclient;
extern alias mediatorgrpcclient;
extern alias mediatorserver;
extern alias mediatrclient;
extern alias mediatrgraphqlclient;
extern alias mediatrgrpcclient;
extern alias mediatrserver;
extern alias zeroallocclient;
extern alias zeroallocgraphqlclient;
extern alias zeroallocgrpcclient;
extern alias zeroallocserver;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Functional.Tests;

/// <summary>
///     For each mediator: a Minimal API server built by the generator, and a client application whose mediator reaches that server through the
///     generated HTTP client and handlers. Every call below is the client's own <c>mediator.Send(...)</c>.
/// </summary>
public class RoundTripTests
{
    private static readonly Dictionary<string, (Func<bool, bool, bool, Task<WebApplication>> Server, Func<HttpMessageHandler, ServiceProvider> Client, Func<HttpMessageHandler, ServiceProvider> GrpcClient, Func<HttpMessageHandler, ServiceProvider> GraphQLClient)> Applications = new()
    {
        ["MediatR"] = (mediatrserver::Api.Functional.MediatR.Server.ServerApp.StartAsync, mediatrclient::Api.Functional.MediatR.Client.ClientApp.Create, mediatrgrpcclient::Api.Functional.MediatR.GrpcClient.ClientApp.Create, mediatrgraphqlclient::Api.Functional.MediatR.GraphQLClient.ClientApp.Create),
        ["Mediator"] = (mediatorserver::Api.Functional.Mediator.Server.ServerApp.StartAsync, mediatorclient::Api.Functional.Mediator.Client.ClientApp.Create, mediatorgrpcclient::Api.Functional.Mediator.GrpcClient.ClientApp.Create, mediatorgraphqlclient::Api.Functional.Mediator.GraphQLClient.ClientApp.Create),
        ["ChannelMediator"] = (channelmediatorserver::Api.Functional.ChannelMediator.Server.ServerApp.StartAsync, channelmediatorclient::Api.Functional.ChannelMediator.Client.ClientApp.Create, channelmediatorgrpcclient::Api.Functional.ChannelMediator.GrpcClient.ClientApp.Create, channelmediatorgraphqlclient::Api.Functional.ChannelMediator.GraphQLClient.ClientApp.Create),
        ["ImmediateHandlers"] = (immediatehandlersserver::Api.Functional.ImmediateHandlers.Server.ServerApp.StartAsync, immediatehandlersclient::Api.Functional.ImmediateHandlers.Client.ClientApp.Create, immediatehandlersgrpcclient::Api.Functional.ImmediateHandlers.GrpcClient.ClientApp.Create, immediatehandlersgraphqlclient::Api.Functional.ImmediateHandlers.GraphQLClient.ClientApp.Create),
        ["Foundatio"] = (foundatioserver::Api.Functional.Foundatio.Server.ServerApp.StartAsync, foundatioclient::Api.Functional.Foundatio.Client.ClientApp.Create, foundatiogrpcclient::Api.Functional.Foundatio.GrpcClient.ClientApp.Create, foundatiographqlclient::Api.Functional.Foundatio.GraphQLClient.ClientApp.Create),
        ["ZeroAlloc"] = (zeroallocserver::Api.Functional.ZeroAlloc.Server.ServerApp.StartAsync, zeroallocclient::Api.Functional.ZeroAlloc.Client.ClientApp.Create, zeroallocgrpcclient::Api.Functional.ZeroAlloc.GrpcClient.ClientApp.Create, zeroallocgraphqlclient::Api.Functional.ZeroAlloc.GraphQLClient.ClientApp.Create),
    };

    [Test]
    [Arguments("MediatR", false)]
    [Arguments("MediatR", true)]
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
    public async Task Should_Round_Trip_Through_The_Client_Mediator(string mediator, bool controllers, CancellationToken cancellationToken)
    {
        var (startServer, createClient, _, _) = Applications[mediator];
        await using var server = await startServer(controllers, false, false);
        await using var client = createClient(server.GetTestServer().CreateHandler());
        var rockets = client.GetRequiredService<IRocketsDriver>();

        var id = await rockets.CreateRocket("Falcon", cancellationToken);
        ( await rockets.GetRocket(id, cancellationToken) ).ShouldBe(new Rocket(id, "Falcon"));

        ( await rockets.EditRocket(id, "Falcon 9", cancellationToken) ).ShouldBe(new Rocket(id, "Falcon 9"));
        var other = await rockets.CreateRocket("Starship", cancellationToken);
        var listed = new List<Rocket>();
        await foreach (var rocket in rockets.ListRockets("Falcon 9", cancellationToken)) listed.Add(rocket);
        listed.ShouldBe([new Rocket(id, "Falcon 9")]);

        await rockets.DeleteRocket(id, cancellationToken);
        ( await rockets.GetRocket(id, cancellationToken) ).ShouldBeNull();
        ( await rockets.GetRocket(other, cancellationToken) ).ShouldBe(new Rocket(other, "Starship"));
    }

    [Test]
    [Arguments("MediatR", false)]
    [Arguments("MediatR", true)]
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
        var (startServer, _, _, _) = Applications[mediator];
        await using var server = await startServer(controllers, false, false);
        using var http = server.GetTestClient();

        using var response = await http.PostAsync("api/rockets", new StringContent("""{"name":"Falcon"}""", System.Text.Encoding.UTF8, "application/json"), cancellationToken);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull().ToString().ShouldStartWith("/api/rockets/");
        using var location = await http.GetAsync(response.Headers.Location, cancellationToken);
        location.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    }

    [Test]
    public async Task Should_Explain_A_Foundatio_Registry_Built_Without_The_Client_Handlers()
    {
        await using var server = await foundatioserver::Api.Functional.Foundatio.Server.ServerApp.StartAsync(false, false, false);

        var exception = Should.Throw<InvalidOperationException>(() => foundatioclient::Api.Functional.Foundatio.Client.LateRegistration.Create(server.GetTestServer().CreateHandler()));

        exception.Message.ShouldStartWith("Foundatio's handler registry was built without RocketsClient's handlers. Call services.AddRocketsClient() before services.AddMediator()");
    }

    [Test]
    [Arguments("MediatR")]
    [Arguments("Mediator")]
    [Arguments("ChannelMediator")]
    [Arguments("ImmediateHandlers")]
    [Arguments("Foundatio")]
    [Arguments("ZeroAlloc")]
    public async Task Should_Round_Trip_Over_Grpc(string mediator, CancellationToken cancellationToken)
    {
        var (startServer, _, createClient, _) = Applications[mediator];
        await using var server = await startServer(false, true, false);
        await using var client = createClient(server.GetTestServer().CreateHandler());
        var rockets = client.GetRequiredService<IRocketsDriver>();

        var id = await rockets.CreateRocket("Falcon", cancellationToken);
        ( await rockets.GetRocket(id, cancellationToken) ).ShouldBe(new Rocket(id, "Falcon"));
        ( await rockets.EditRocket(id, "Falcon 9", cancellationToken) ).ShouldBe(new Rocket(id, "Falcon 9"));
        var listed = new List<Rocket>();
        await foreach (var rocket in rockets.ListRockets("Falcon 9", cancellationToken)) listed.Add(rocket);
        listed.ShouldBe([new Rocket(id, "Falcon 9")]);
        await rockets.DeleteRocket(id, cancellationToken);
        ( await rockets.GetRocket(id, cancellationToken) ).ShouldBeNull();
    }

    [Test]
    [Arguments("MediatR")]
    [Arguments("Mediator")]
    [Arguments("ChannelMediator")]
    [Arguments("ImmediateHandlers")]
    [Arguments("Foundatio")]
    [Arguments("ZeroAlloc")]
    public async Task Should_Send_Mutations_Over_GraphQL(string mediator, CancellationToken cancellationToken)
    {
        var (startServer, _, _, createClient) = Applications[mediator];
        await using var server = await startServer(false, false, true);
        await using var client = createClient(server.GetTestServer().CreateHandler());
        var rockets = client.GetRequiredService<IRocketsDriver>();
        using var http = server.GetTestClient();

        var id = await rockets.CreateRocket("Falcon", cancellationToken);
        ( await rockets.EditRocket(id, "Falcon 9", cancellationToken) ).ShouldBe(new Rocket(id, "Falcon 9"));
        ( await http.GetStringAsync($"api/rockets/{id}", cancellationToken) ).ShouldContain("Falcon 9");

        await rockets.DeleteRocket(id, cancellationToken);
        ( await http.GetAsync($"api/rockets/{id}", cancellationToken) ).StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }
}
