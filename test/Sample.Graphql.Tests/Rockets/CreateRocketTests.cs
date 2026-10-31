using Microsoft.Extensions.DependencyInjection;

using Sample.Graphql.Tests.Helpers;

using Serilog.Events;

namespace Sample.Graphql.Tests.Rockets;

public class CreateRocketTests() : GraphQlWebAppFixtureTest(LogEventLevel.Information)
{
    [Test]
    public async Task Should_Create_A_Rocket()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var response = await client.CreateRocket.ExecuteAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            },
            cancellationToken: TestContext.CancellationToken
        );

        response.EnsureNoErrors();
        response.Data!.CreateRocket.Id.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public async Task Should_Throw_If_Rocket_Exists()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var response = await client.CreateRocket.ExecuteAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            },
            cancellationToken: TestContext.CancellationToken
        );
        response.IsErrorResult().ShouldBeFalse();

        var response2 = await client.CreateRocket.ExecuteAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            },
            cancellationToken: TestContext.CancellationToken
        );
        response2.IsErrorResult().ShouldBeTrue();
        response2.Errors[0].Message.ShouldBe("A Rocket already exists with that serial number!");
    }
}
