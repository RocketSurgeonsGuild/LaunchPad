using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Restful.Client;
using Sample.Restful.Tests.Helpers;

using RocketType = Sample.Restful.Client.RocketType;

namespace Sample.Restful.Tests.Rockets;

public class ListRocketsTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_List_Rockets()
    {
        var client = new RocketClient(AlbaHost.Server.CreateClient());
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            z.AddRange(faker.Generate(10));

            await z.SaveChangesAsync();
        }

        var response = await client.ListRocketsAsync();

        response.Result.ShouldHaveCount(10);
    }

    [Test]
    public async Task Should_List_Specific_Kinds_Of_Rockets()
    {
        var client = new RocketClient(AlbaHost.Server.CreateClient());
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            z.AddRange(faker.UseSeed(100).Generate(10));

            await z.SaveChangesAsync();
        }

        var response = await client.ListRocketsAsync(RocketType.AtlasV);

        response.Result.ShouldHaveCount(5);
    }

    private static readonly Faker Faker = new();
}
