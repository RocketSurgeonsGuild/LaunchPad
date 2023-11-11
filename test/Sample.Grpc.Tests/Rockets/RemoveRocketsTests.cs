using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;
using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class RemoveRocketsTests(ITestOutputHelper outputHelper, TestWebAppFixture testWebAppFixture)
    : WebAppFixtureTest<TestWebAppFixture>(outputHelper, testWebAppFixture)
{
    private static readonly Faker Faker = new();

    [Fact]
    public async Task Should_Remove_Rocket()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());
        var id = await ServiceProvider.WithScoped<RocketDbContext>()
                                      .Invoke(
                                           async z =>
                                           {
                                               var faker = new RocketFaker();
                                               var rocket = faker.Generate();
                                               z.Add(rocket);

                                               await z.SaveChangesAsync();
                                               return rocket.Id;
                                           }
                                       );

        await client.DeleteRocketAsync(new DeleteRocketRequest { Id = id.ToString() });

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.Rockets.Should().BeEmpty());
    }
}
