using Microsoft.Extensions.DependencyInjection;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.Rockets;

public class CreateRocketTests : HandleWebHostBase
{
    public CreateRocketTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task Should_Create_A_Rocket()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
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
        var client = Factory.Services.GetRequiredService<IRocketClient>();
        await client.CreateRocket.ExecuteAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );

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
