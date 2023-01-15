using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests.LaunchRecords;

public class ListLaunchRecordsTests : GraphQlWebAppFixtureTest<GraphQlAppFixture>
{
    [Fact]
    public async Task Should_List_LaunchRecords()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      var faker = new RocketFaker();
                                      var rockets = faker.Generate(3);
                                      var records = new LaunchRecordFaker(rockets).Generate(10);
                                      z.AddRange(rockets);
                                      z.AddRange(records);
                                      await z.SaveChangesAsync();
                                  }
                              );

        var response = await client.GetLaunchRecords.ExecuteAsync();
        response.EnsureNoErrors();

        response.Data!.LaunchRecords!.Nodes!.Should().HaveCount(10);
    }


    [Fact]
    public async Task Should_List_Specific_Kinds_Of_LaunchRecords()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      var faker = new RocketFaker();
                                      var rockets = faker.UseSeed(100).Generate(3);
                                      var records = new LaunchRecordFaker(rockets).UseSeed(100).Generate(10);
                                      z.AddRange(rockets);
                                      z.AddRange(records);
                                      await z.SaveChangesAsync();
                                  }
                              );

        var response = await client.GetFilteredLaunchRecords.ExecuteAsync(RocketType.FalconHeavy);
        response.EnsureNoErrors();

        response.Data!.LaunchRecords!.Nodes!.Should().HaveCount(3);
    }

    public ListLaunchRecordsTests(ITestOutputHelper outputHelper, GraphQlAppFixture rocketSurgeryWebAppFixture) : base(outputHelper, rocketSurgeryWebAppFixture)
    {
    }

    private static readonly Faker Faker = new();
}
