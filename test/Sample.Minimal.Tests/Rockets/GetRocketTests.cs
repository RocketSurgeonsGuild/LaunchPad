using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;
using Sample.Minimal.Tests.Client;
using Sample.Minimal.Tests.Helpers;

using HttpRocketType = Sample.Minimal.Tests.Client.RocketType;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Minimal.Tests.Rockets;

public class GetRocketTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Get_A_Rocket()
    {
        var client = Client;
        var rocket = new ReadyRocket
        {
            Type = RocketType.Falcon9,
            SerialNumber = "12345678901234"
        };
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            z.Add(rocket);

            await z.SaveChangesAsync();
        }

        var response = await client.GetRocket(rocket.Id.Value);

        await response.Type.Should().BeEqualTo(HttpRocketType.Falcon9);
        await response.Sn.Should().BeEqualTo("12345678901234");
    }

    [Test]
    public async Task Should_Not_Get_A_Missing_Rocket()
    {
        var client = Client;

        var (status, problem) = await Failure<ProblemDetails>(() => client.GetRocket(Guid.NewGuid()));
        await status.Should().BeEqualTo(System.Net.HttpStatusCode.NotFound);
        await problem!.Status.Should().BeEqualTo(404);
        await problem.Title.Should().BeEqualTo("Not Found");
    }

    private static readonly Faker Faker = new();
}
