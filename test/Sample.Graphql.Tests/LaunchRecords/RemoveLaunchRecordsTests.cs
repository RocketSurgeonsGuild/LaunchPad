using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests : GraphQlWebAppFixtureTest<GraphQlAppFixture>
{
    private static readonly Faker Faker = new();

    public RemoveLaunchRecordsTests(ITestOutputHelper outputHelper, GraphQlAppFixture rocketSurgeryWebAppFixture) : base(outputHelper, rocketSurgeryWebAppFixture)
    {
    }

    [Fact]
    public async Task Should_Remove_LaunchRecord()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var id = await ServiceProvider.WithScoped<RocketDbContext>()
                                      .Invoke(
                                           async z =>
                                           {
                                               var faker = new RocketFaker();
                                               var rocket = faker.Generate();
                                               var record = new LaunchRecordFaker(new[] { rocket }.ToList()).Generate();
                                               z.Add(rocket);
                                               z.Add(record);

                                               await z.SaveChangesAsync();
                                               return record.Id;
                                           }
                                       );

        var response = await client.DeleteLaunchRecord.ExecuteAsync(new DeleteLaunchRecordRequest { Id = id.Value });
        response.EnsureNoErrors();

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.LaunchRecords.Should().BeEmpty());
    }
}
