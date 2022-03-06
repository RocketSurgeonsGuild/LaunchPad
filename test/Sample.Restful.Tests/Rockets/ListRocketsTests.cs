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

namespace Sample.Restful.Tests.Rockets;

public class ListRocketsTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_List_Rockets()
    {
        var client = new RocketClient(Factory.CreateClient());
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      var faker = new RocketFaker();
                                      z.AddRange(faker.Generate(10));

                                      await z.SaveChangesAsync();
                                  }
                              );

        var response = await client.ListRocketsAsync();

        response.Result.Should().HaveCount(10);
    }

    [Fact]
    public async Task Should_List_Specific_Kinds_Of_Rockets()
    {
        var client = new RocketClient(Factory.CreateClient());
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      var faker = new RocketFaker();
                                      z.AddRange(faker.UseSeed(100).Generate(10));

                                      await z.SaveChangesAsync();
                                  }
                              );

        var response = await client.ListRocketsAsync(RocketType.AtlasV);

        response.Result.Should().HaveCount(5);
    }

    public ListRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new();
}
#endif
