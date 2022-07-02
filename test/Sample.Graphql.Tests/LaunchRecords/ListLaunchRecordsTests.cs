﻿using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;

namespace Sample.Graphql.Tests.LaunchRecords;

public class ListLaunchRecordsTests : HandleWebHostBase
{
    private static readonly Faker Faker = new();

    public ListLaunchRecordsTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public async Task Should_List_LaunchRecords()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
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

        response.Data!.LaunchRecords!.Items.Should().HaveCount(10);
    }


    [Fact]
    public async Task Should_List_Specific_Kinds_Of_LaunchRecords()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
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

        response.Data!.LaunchRecords!.Items.Should().HaveCount(3);
    }
}
