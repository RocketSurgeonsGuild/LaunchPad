using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;
using Grpc.Core;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Rocket.Surgery.LaunchPad.Grpc.Validation;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Grpc.Tests.Validation.Integration;

public class CustomMessageHandlerIntegrationTest : LoggerTest, IClassFixture<TestWebHost>
{
    [Fact]
    public async Task Should_ThrowInvalidArgument_When_NameOfMessageIsEmpty()
    {
        // Given
        var client = new Grpc.Rockets.RocketsClient(_factory.CreateGrpcChannel());

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
        Assert.Equal("Validation Error!", rpcException.Status.Detail);
    }

    public CustomMessageHandlerIntegrationTest(ITestOutputHelper testOutputHelper, TestWebHost factory) : base(testOutputHelper)
    {
        _factory = factory
                  .ConfigureLoggerFactory(LoggerFactory)
                  .ConfigureHostBuilder(
                       options => options.ConfigureServices(services => { services.AddSingleton<IValidatorErrorMessageHandler>(new CustomMessageHandler()); })
                   );
    }

    private readonly ConventionTestWebHost<Startup> _factory;

    private class CustomMessageHandler : IValidatorErrorMessageHandler
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
