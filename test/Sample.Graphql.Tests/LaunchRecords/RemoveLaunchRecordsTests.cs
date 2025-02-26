using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests(ITestContextAccessor outputHelper, GraphQlAppFixture rocketSurgeryWebAppFixture)
    : GraphQlWebAppFixtureTest<GraphQlAppFixture>(outputHelper, rocketSurgeryWebAppFixture)
{
    private static readonly Faker Faker = new();

    [Fact]
    public async Task Should_Remove_LaunchRecord()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var id = await ServiceProvider.WithScoped<RocketDbContext>()
                                      .Invoke(
                                           async (z, ct) =>
                                           {
                                               var faker = new RocketFaker();
                                               var rocket = faker.Generate();
                                               var record = new LaunchRecordFaker(new[] { rocket }.ToList()).Generate();
                                               z.Add(rocket);
                                               z.Add(record);

                                               await z.SaveChangesAsync(ct);
                                               return record.Id;
                                           },
                                           TestContext.CancellationToken
                                       );

        var response = await client.DeleteLaunchRecord.ExecuteAsync(new DeleteLaunchRecordRequest { Id = id.Value },
            cancellationToken: TestContext.CancellationToken);
        response.EnsureNoErrors();

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.LaunchRecords.ShouldBeEmpty());
    }
}
