using FluentValidation.Results;

using Grpc.Core;

using Rocket.Surgery.LaunchPad.Grpc.Validation;

using Sample.Grpc.Tests.Helpers;

namespace Sample.Grpc.Tests.Validation.Integration;

public class CustomMessageHandlerIntegrationTest : WebAppFixtureTest
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
        rpcException.Status.Detail.ShouldBe("Property Id failed validation.");
    }

    private sealed class CustomMessageHandler : IValidatorErrorMessageHandler
    {
        public Task<string> HandleAsync(IEnumerable<ValidationFailure> failures) => Task.FromResult("Validation Error!");

        public string Handle(IEnumerable<ValidationFailure> failures) => "Validation Error!";
    }
}
