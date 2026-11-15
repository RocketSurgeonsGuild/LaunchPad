using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Minimal.Tests.Helpers;

using RocketType = Sample.Minimal.Tests.Client.RocketType;

namespace Sample.Minimal.Tests.Rockets;

public class ListRocketsTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_List_Rockets()
    {
        var client = Client;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            z.AddRange(faker.Generate(10));

            await z.SaveChangesAsync();
        }

        var response = await client.ListRockets(null).ToListAsync();

        await response.Should().HaveCount(10);
    }

    [Test]
    public async Task Should_List_Specific_Kinds_Of_Rockets()
    {
        var client = Client;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            z.AddRange(faker.UseSeed(100).Generate(10));

            await z.SaveChangesAsync();
        }

        var response = await client.ListRockets(RocketType.AtlasV).ToListAsync();

        await response.Should().HaveCount(5);
    }

    private static readonly Faker Faker = new();
}
