using Grpc.Core;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Validation;
using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class ListRocketsTests : HandleGrpcHostBase
{
    private static readonly Faker Faker = new();

    public ListRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public async Task Should_List_Rockets()
    {
        var client = new R.RocketsClient(Factory.CreateGrpcChannel());
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      var faker = new RocketFaker();
                                      z.AddRange(faker.Generate(10));

                                      await z.SaveChangesAsync();
                                  }
                              );

        var response = await client.ListRockets(new ListRocketsRequest()).ResponseStream.ReadAllAsync().ToListAsync();

        response.Should().HaveCount(10);
    }

    [Fact]
    public async Task Should_List_Specific_Kinds_Of_Rockets()
    {
        var client = new R.RocketsClient(Factory.CreateGrpcChannel());
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      var faker = new RocketFaker();
                                      z.AddRange(faker.UseSeed(100).Generate(10));

                                      await z.SaveChangesAsync();
                                  }
                              );

        var response = await client.ListRockets(
            new ListRocketsRequest
            {
                RocketType = new NullableRocketType
                {
                    Data = RocketType.AtlasV
                }
            }
        ).ResponseStream.ReadAllAsync().ToListAsync();

        response.Should().HaveCount(5);
    }
}
