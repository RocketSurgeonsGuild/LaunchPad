using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Graphql.Tests.Helpers;

using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.Rockets;

public class GetRocketTests : GraphQlWebAppFixtureTest
{
    [Test]
    public async Task Should_Get_A_Rocket()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        RocketId rocket;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var seeded = new ReadyRocket
            {
                Type = CoreRocketType.Falcon9,
                SerialNumber = "12345678901234"
            };
            z.Add(seeded);

            await z.SaveChangesAsync(TestContext.CancellationToken);
            rocket = seeded.Id;
        }

        var response = await client.GetRocket.ExecuteAsync(rocket.Value, cancellationToken: TestContext.CancellationToken);
        response.EnsureNoErrors();

        response.Data!.Rockets!.Nodes![0].Type.ShouldBe(GraphQlRocketType.Falcon9);
        response.Data.Rockets!.Nodes[0].SerialNumber.ShouldBe("12345678901234");
    }

    private static readonly Faker Faker = new();
}
