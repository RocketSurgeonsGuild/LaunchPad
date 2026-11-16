using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Restful.Tests.Helpers;

// the filter's own enum: .NET 11 documents a query parameter's enum inline, with the C# names, rather than as a reference to RocketType
using RocketType = Sample.Restful.Tests.Client.RocketType2;

namespace Sample.Restful.Tests.LaunchRecords;

public class ListLaunchRecordsTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_List_LaunchRecords()
    {
        var client = Client;
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

        var response = await client.ListLaunchRecords(null).ToListAsync();

        await response.Should().HaveCount(10);
    }

    [Test]
    public async Task Should_List_Specific_Kinds_Of_LaunchRecords()
    {
        var client = Client;
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

        var response = await client.ListLaunchRecords(RocketType.FalconHeavy).ToListAsync();
        await response.Should().HaveCount(3);
    }

    private static readonly Faker Faker = new();
}
