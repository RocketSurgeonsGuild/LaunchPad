using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.Rockets;

public class GetRocketTests(ITestOutputHelper outputHelper, GraphQlAppFixture rocketSurgeryWebAppFixture)
    : GraphQlWebAppFixtureTest<GraphQlAppFixture>(outputHelper, rocketSurgeryWebAppFixture)
{
    [Fact]
    public async Task Should_Get_A_Rocket()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = CoreRocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket.Id;
                                               }
                                           );

        var response = await client.GetRocket.ExecuteAsync(rocket.Value);
        response.EnsureNoErrors();

        response.Data!.Rockets!.Nodes![0].Type.Should().Be(RocketType.Falcon9);
        response.Data.Rockets!.Nodes[0].SerialNumber.Should().Be("12345678901234");
    }

    private static readonly Faker Faker = new();
}
