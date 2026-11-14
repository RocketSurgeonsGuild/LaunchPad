using Grpc.Core;

using Sample.Grpc.Tests.Helpers;

namespace Sample.Grpc.Tests.Validation.Integration;

public class ValidationMessageIntegrationTest : WebAppFixtureTest
{
    [Test]
    public async Task Should_ThrowInvalidArgument_When_NameOfMessageIsEmpty()
    {
        // Given
        var client = new Grpc.Rockets.RocketsClient(AlbaHost.Server.CreateGrpcChannel());

        // When
        async Task Action()
        {
            await client.GetRocketsAsync(
                new GetRocketRequest
                {
                    Id = ""
                }
            );
        }

        // Then
        var rpcException = await ( (Func<Task>)Action ).Should().Throw<RpcException>();
        await rpcException.Status.StatusCode.Should().BeEqualTo(StatusCode.InvalidArgument);
        await rpcException.Status.Detail.Should().StartWith("Property Id failed validation.");
    }
}
