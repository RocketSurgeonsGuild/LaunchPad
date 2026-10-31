using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests : GraphQlWebAppFixtureTest
{
    private static readonly Faker Faker = new();

    [Test]
    public async Task Should_Remove_LaunchRecord()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        LaunchRecordId id;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rocket = faker.Generate();
            var record = new LaunchRecordFaker(new[] { rocket }.ToList()).Generate();
            z.Add(rocket);
            z.Add(record);

            await z.SaveChangesAsync(TestContext.CancellationToken);
            id = record.Id;
        }

        var response = await client.DeleteLaunchRecord.ExecuteAsync(new DeleteLaunchRecordRequest { Id = id.Value },
            cancellationToken: TestContext.CancellationToken);
        response.EnsureNoErrors();

        await using var assertScope = ServiceProvider.CreateAsyncScope();
        assertScope.ServiceProvider.GetRequiredService<RocketDbContext>().LaunchRecords.ShouldBeEmpty();
    }
}
