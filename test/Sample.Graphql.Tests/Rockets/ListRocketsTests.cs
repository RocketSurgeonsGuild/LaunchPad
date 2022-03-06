using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;

namespace Sample.Graphql.Tests.Rockets;

public class ListRocketsTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_List_Rockets()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
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

        response.Data.Rockets.Items.Should().HaveCount(10);
    }

    [Fact]
    public async Task Should_List_Specific_Kinds_Of_Rockets()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
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

        response.Data.Rockets.Items.Should().HaveCount(5);
    }

    public ListRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new();
}
