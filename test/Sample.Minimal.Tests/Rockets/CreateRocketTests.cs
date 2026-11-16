using Sample.Minimal.Tests.Helpers;

namespace Sample.Minimal.Tests.Rockets;

public class CreateRocketTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Create_A_Rocket()
    {
        var client = Client;

        var response = await client.CreateRocket(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );

        await response.Id.Should().NotBeEqualTo(Guid.Empty);
    }

    [Test]
    public async Task Should_Throw_If_Rocket_Exists()
    {
        var client = Client;
        await client.CreateRocket(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );

        var (_, problem) = await Failure<ProblemDetails>(() => client.CreateRocket(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        ));
        await problem!.Title.Should().BeEqualTo("Rocket Creation Failed");
    }
}
