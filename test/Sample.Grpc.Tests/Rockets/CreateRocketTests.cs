using Grpc.Core;

using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;

using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class CreateRocketTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Create_A_Rocket()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());

        var response = await client.CreateRocketAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            },
            cancellationToken: TestContext.CancellationToken
        );

        await response.Id.Should().NotBeEmpty();
    }

    [Test]
    public async Task Should_Throw_If_Rocket_Exists()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());
        await client.CreateRocketAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            },
            cancellationToken: TestContext.CancellationToken
        );

        var action = async () => await client.CreateRocketAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            },
            cancellationToken: TestContext.CancellationToken
        );
        var r = await action.Should().Throw<RpcException>();
        // the handler answers Conflict, which a gRPC call ends with as FailedPrecondition and the case's detail
        await r!.Status.StatusCode.Should().BeEqualTo(StatusCode.FailedPrecondition);
        await r.Status.Detail.Should().BeEqualTo("A Rocket already exists with that serial number!");
    }
}
