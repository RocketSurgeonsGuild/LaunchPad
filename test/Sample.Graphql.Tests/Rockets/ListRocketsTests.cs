﻿using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests.Rockets;

public class ListRocketsTests(ITestOutputHelper outputHelper, GraphQlAppFixture rocketSurgeryWebAppFixture)
    : GraphQlWebAppFixtureTest<GraphQlAppFixture>(outputHelper, rocketSurgeryWebAppFixture)
{
    [Fact]
    public async Task Should_List_Rockets()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      var faker = new RocketFaker();
                                      z.AddRange(faker.Generate(10));

                                      await z.SaveChangesAsync();
                                  }
                              );

        var response = await client.GetRockets.ExecuteAsync();
        response.EnsureNoErrors();

        response.Data!.Rockets!.Nodes!.Should().HaveCount(10);
    }

    [Fact]
    public async Task Should_List_Specific_Kinds_Of_Rockets()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      var faker = new RocketFaker();
                                      z.AddRange(faker.UseSeed(100).Generate(10));

                                      await z.SaveChangesAsync();
                                  }
                              );

        var response = await client.GetFilteredRockets.ExecuteAsync(RocketType.AtlasV);
        response.EnsureNoErrors();

        response.Data!.Rockets!.Nodes!.Should().HaveCount(5);
    }

    private static readonly Faker Faker = new();
}
