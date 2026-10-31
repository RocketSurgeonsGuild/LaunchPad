using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests.Rockets;

public class RemoveRocketsTests : GraphQlWebAppFixtureTest
{
    private static readonly Faker Faker = new();

    [Test]
    public async Task Should_Remove_Rocket()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        RocketId id;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rocket = faker.Generate();
            z.Add(rocket);

            await z.SaveChangesAsync(TestContext.CancellationToken);
            id = rocket.Id;
        }

        await client.DeleteRocket.ExecuteAsync(new DeleteRocketRequest { Id = id.Value }, TestContext.CancellationToken);

        await using var assertScope = ServiceProvider.CreateAsyncScope();
        assertScope.ServiceProvider.GetRequiredService<RocketDbContext>().Rockets.ShouldBeEmpty();
    }
}
