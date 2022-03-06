#if NET6_0_OR_GREATER
using Bogus;
using FluentAssertions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Restful.Client;
using Xunit;
using Xunit.Abstractions;
using RocketType = Sample.Restful.Client.RocketType;

namespace Sample.Restful.Tests.LaunchRecords;

public class ListLaunchRecordsTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_List_LaunchRecords()
    {
        var client = new LaunchRecordClient(Factory.CreateClient());
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

        var response = await client.ListLaunchRecordsAsync();

        response.Result.Should().HaveCount(10);
    }

    [Fact]
    public async Task Should_List_Specific_Kinds_Of_LaunchRecords()
    {
        var client = new LaunchRecordClient(Factory.CreateClient());
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

        var response = await client.ListLaunchRecordsAsync(RocketType.FalconHeavy);
        response.Result.Should().HaveCount(3);
    }

    public ListLaunchRecordsTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new();
}
#endif
