using Microsoft.Extensions.DependencyInjection;
using Sample.Graphql.Tests.Helpers;
using Serilog.Events;

namespace Sample.Graphql.Tests.Rockets;

public class CreateRocketTests(ITestOutputHelper testOutputHelper, GraphQlAppFixture rocketSurgeryWebAppFixture)
    : GraphQlWebAppFixtureTest<GraphQlAppFixture>(testOutputHelper, rocketSurgeryWebAppFixture, LogEventLevel.Information)
{
    [Fact]
    public async Task Should_Create_A_Rocket()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var response = await client.CreateRocket.ExecuteAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );

        response.EnsureNoErrors();
        response.Data!.CreateRocket.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_Throw_If_Rocket_Exists()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var response = await client.CreateRocket.ExecuteAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );
        response.IsErrorResult().Should().BeFalse();

        var response2 = await client.CreateRocket.ExecuteAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );
        response2.IsErrorResult().Should().BeTrue();
        response2.Errors[0].Message.Should().Be("A Rocket already exists with that serial number!");
    }
}
