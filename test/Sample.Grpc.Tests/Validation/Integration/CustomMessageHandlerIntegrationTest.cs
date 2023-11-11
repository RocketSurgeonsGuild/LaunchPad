using FluentValidation.Results;
using Grpc.Core;
using Rocket.Surgery.LaunchPad.Grpc.Validation;
using Sample.Grpc.Tests.Helpers;

namespace Sample.Grpc.Tests.Validation.Integration;

public class CustomMessageHandlerIntegrationTest(ITestOutputHelper testOutputHelper, TestWebAppFixture factory)
    : WebAppFixtureTest<TestWebAppFixture>(testOutputHelper, factory)
{
    [Fact]
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
        var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
        Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
        Assert.Equal("Property Id failed validation.", rpcException.Status.Detail);
    }

    private sealed class CustomMessageHandler : IValidatorErrorMessageHandler
    {
        public Task<string> HandleAsync(IEnumerable<ValidationFailure> failures)
        {
            return Task.FromResult("Validation Error!");
        }

        public string Handle(IEnumerable<ValidationFailure> failures)
        {
            return "Validation Error!";
        }
    }
}
