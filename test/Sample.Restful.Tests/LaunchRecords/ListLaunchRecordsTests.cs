using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Restful.Client;
using Sample.Restful.Tests.Helpers;

using RocketType = Sample.Restful.Client.RocketType;

namespace Sample.Restful.Tests.LaunchRecords;

public class ListLaunchRecordsTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_List_LaunchRecords()
    {
        var client = new LaunchRecordClient(AlbaHost.Server.CreateClient());
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rockets = faker.Generate(3);
            var records = new LaunchRecordFaker(rockets).Generate(10);
            z.AddRange(rockets);
            z.AddRange(records);
            await z.SaveChangesAsync();
        }

        var response = await client.ListLaunchRecordsAsync();

        response.Result.ShouldHaveCount(10);
    }

    [Test]
    public async Task Should_List_Specific_Kinds_Of_LaunchRecords()
    {
        var client = new LaunchRecordClient(AlbaHost.Server.CreateClient());
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rockets = faker.UseSeed(100).Generate(3);
            var records = new LaunchRecordFaker(rockets).UseSeed(100).Generate(10);
            z.AddRange(rockets);
            z.AddRange(records);
            await z.SaveChangesAsync();
        }

        var response = await client.ListLaunchRecordsAsync(RocketType.FalconHeavy);
        response.Result.ShouldHaveCount(3);
    }

    private static readonly Faker Faker = new();
}
