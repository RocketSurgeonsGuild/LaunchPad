extern alias zeroallocserver;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using ProtoFirst = zeroallocserver::Api.Functional.ZeroAlloc.Server.ProtoFirst;
using Server = zeroallocserver::Api.Functional.ZeroAlloc.Server;

namespace Api.Functional.Tests;

/// <summary>
///     Proto-first gRPC services: the ZeroAlloc fixture server's <c>ProtoRocketsService</c> and <c>ProtoLaunchesService</c> derive the service bases
///     Grpc.Tools generated from <c>proto_first.proto</c>, and the generator implements their rpcs through the mediator, mapping the proto messages
///     with ZeroAlloc.Mapping. The tests call them through the Grpc.Tools clients of the same <c>.proto</c>.
/// </summary>
public class ProtoFirstTests
{
    private static async Task<(WebApplication Server, GrpcChannel Channel)> Start()
    {
        var server = await Server.ServerApp.StartAsync(protoFirst: true);
        return (server, GrpcChannel.ForAddress(server.GetTestServer().BaseAddress, new() { HttpHandler = server.GetTestServer().CreateHandler() }));
    }

    [Test]
    public async Task Should_Round_Trip(CancellationToken cancellationToken)
    {
        var (server, channel) = await Start();
        await using var _ = server;
        using var __ = channel;
        var rockets = new ProtoFirst.Rockets.RocketsClient(channel);

        var id = ( await rockets.CreateRocketAsync(new() { Name = "Falcon" }, cancellationToken: cancellationToken) ).Id;
        await Rocket(await rockets.GetRocketAsync(new() { Id = id }, cancellationToken: cancellationToken)).Should().BeEqualTo((id, "Falcon"));

        await Rocket(await rockets.EditRocketAsync(new() { Id = id, Name = "Falcon 9" }, cancellationToken: cancellationToken)).Should().BeEqualTo((id, "Falcon 9"));
        var other = ( await rockets.CreateRocketAsync(new() { Name = "Starship" }, cancellationToken: cancellationToken) ).Id;

        // A server streaming rpc: the stream operation's items, each mapped to the proto message.
        using var list = rockets.ListRockets(new() { Name = "Falcon 9" }, cancellationToken: cancellationToken);
        var listed = new List<(string, string)>();
        await foreach (var rocket in list.ResponseStream.ReadAllAsync(cancellationToken)) listed.Add(Rocket(rocket));
        await listed.Should().BeEquivalentTo([(id, "Falcon 9")], CollectionOrdering.Matching);

        // A void operation answers google.protobuf.Empty; a nullable response that is null answers NOT_FOUND.
        await rockets.DeleteRocketAsync(new() { Id = id }, cancellationToken: cancellationToken);
        var missing = await ( (Func<Task>)( async () => await rockets.GetRocketAsync(new() { Id = id }, cancellationToken: cancellationToken) ) ).Should().Throw<RpcException>();
        await missing.StatusCode.Should().BeEqualTo(StatusCode.NotFound);
        await Rocket(await rockets.GetRocketAsync(new() { Id = other }, cancellationToken: cancellationToken)).Should().BeEqualTo((other, "Starship"));
    }

    [Test]
    public async Task Should_Answer_Union_Cases(CancellationToken cancellationToken)
    {
        var (server, channel) = await Start();
        await using var _ = server;
        using var __ = channel;
        var rockets = new ProtoFirst.Rockets.RocketsClient(channel);
        var launches = new ProtoFirst.Launches.LaunchesClient(channel);
        var falcon = ( await rockets.CreateRocketAsync(new() { Name = "Falcon" }, cancellationToken: cancellationToken) ).Id;
        var grounded = ( await rockets.CreateRocketAsync(new() { Name = "Grounded" }, cancellationToken: cancellationToken) ).Id;
        var unknown = Guid.NewGuid().ToString();

        await ( await launches.LaunchRocketAsync(new() { Id = falcon }, cancellationToken: cancellationToken) ).Name.Should().BeEqualTo("Falcon");

        // RocketNotFound isn't mapped to a proto message: its status and name travel, no case-bin.
        var notFound = await ( (Func<Task>)( async () => await launches.LaunchRocketAsync(new() { Id = unknown }, cancellationToken: cancellationToken) ) ).Should().Throw<RpcException>();
        await notFound.StatusCode.Should().BeEqualTo(StatusCode.NotFound);
        await notFound.Trailers.GetValue("case").Should().BeEqualTo("RocketNotFound");
        await notFound.Trailers.GetValueBytes("case-bin").Should().BeNull();

        // NotReady is mapped to NotReadyError, which the case-bin trailer carries.
        var notReady = await ( (Func<Task>)( async () => await launches.LaunchRocketAsync(new() { Id = grounded }, cancellationToken: cancellationToken) ) ).Should().Throw<RpcException>();
        await notReady.StatusCode.Should().BeEqualTo(StatusCode.FailedPrecondition);
        await notReady.Trailers.GetValue("case").Should().BeEqualTo("NotReady");
        await ProtoFirst.NotReadyError.Parser.ParseFrom(notReady.Trailers.GetValueBytes("case-bin")).Reason.Should().BeEqualTo("grounded");
    }

    [Test]
    public async Task Should_End_A_Stream_Of_Unions_With_Its_Error_Case(CancellationToken cancellationToken)
    {
        var (server, channel) = await Start();
        await using var _ = server;
        using var __ = channel;
        var rockets = new ProtoFirst.Rockets.RocketsClient(channel);
        var launches = new ProtoFirst.Launches.LaunchesClient(channel);
        var falcon = ( await rockets.CreateRocketAsync(new() { Name = "Falcon" }, cancellationToken: cancellationToken) ).Id;
        await rockets.CreateRocketAsync(new() { Name = "Grounded" }, cancellationToken: cancellationToken);

        // proto3 has no null string: the request's name is "Falcon", then "Grounded", each streaming its rockets.
        using var flying = launches.ListLaunches(new() { Name = "Falcon" }, cancellationToken: cancellationToken);
        var received = new List<string>();
        await foreach (var rocket in flying.ResponseStream.ReadAllAsync(cancellationToken)) received.Add(rocket.Id);
        await received.Should().BeEquivalentTo([falcon], CollectionOrdering.Matching);

        using var stuck = launches.ListLaunches(new() { Name = "Grounded" }, cancellationToken: cancellationToken);
        var error = await ( (Func<Task>)( async () =>
            {
                await foreach (var rocket in stuck.ResponseStream.ReadAllAsync(cancellationToken)) received.Add(rocket.Id);
            } ) ).Should().Throw<RpcException>();
        await error.StatusCode.Should().BeEqualTo(StatusCode.FailedPrecondition);
        await error.Trailers.GetValue("case").Should().BeEqualTo("NotReady");
    }

    private static (string Id, string Name) Rocket(ProtoFirst.RocketReply rocket) => (rocket.Id, rocket.Name);
}
