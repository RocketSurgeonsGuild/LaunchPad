using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests.LaunchRecords;

public class ListLaunchRecordsTests : GraphQlWebAppFixtureTest
{
    [Test]
    public async Task Should_List_LaunchRecords()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rockets = faker.Generate(3);
            var records = new LaunchRecordFaker(rockets).Generate(10);
            z.AddRange(rockets);
            z.AddRange(records);
            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client.GetLaunchRecords.ExecuteAsync(
            cancellationToken: TestContext.CancellationToken);
        response.EnsureNoErrors();

        response.Data!.LaunchRecords!.Nodes!.Count.ShouldBe(10);
    }


    [Test]
    public async Task Should_List_Specific_Kinds_Of_LaunchRecords()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rockets = faker.UseSeed(100).Generate(3);
            var records = new LaunchRecordFaker(rockets).UseSeed(100).Generate(10);
            z.AddRange(rockets);
            z.AddRange(records);
            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client.GetFilteredLaunchRecords.ExecuteAsync(RocketType.FalconHeavy,
            cancellationToken: TestContext.CancellationToken);
        response.EnsureNoErrors();

        response.Data!.LaunchRecords!.Nodes!.Count.ShouldBe(3);
    }

    private static readonly Faker Faker = new();
}
