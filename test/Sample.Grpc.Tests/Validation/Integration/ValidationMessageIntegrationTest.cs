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
        var rpcException = await Should.ThrowAsync<RpcException>(Action);
        rpcException.Status.StatusCode.ShouldBe(StatusCode.InvalidArgument);
        rpcException.Status.Detail.ShouldStartWith("Property Id failed validation.");
    }
}
