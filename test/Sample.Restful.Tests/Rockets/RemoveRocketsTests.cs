using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Restful.Tests.Helpers;

namespace Sample.Restful.Tests.Rockets;

public class RemoveRocketsTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Remove_Rocket()
    {
        var client = Client;
        var faker = new RocketFaker();
        var rocket = faker.Generate();
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            z.Add(rocket);

            await z.SaveChangesAsync().ConfigureAwait(false);
        }

        await client.RemoveRocket(rocket.Id.Value);

        await using var assertScope = ServiceProvider.CreateAsyncScope();
        await assertScope.ServiceProvider.GetRequiredService<RocketDbContext>().Rockets.Should().BeEmpty();
    }

    private static readonly Faker Faker = new();
}
