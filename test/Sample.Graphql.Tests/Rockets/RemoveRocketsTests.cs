using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests.Rockets;

public class RemoveRocketsTests : GraphQlWebAppFixtureTest<GraphQlAppFixture>
{
    private static readonly Faker Faker = new();

    public RemoveRocketsTests(ITestOutputHelper outputHelper, GraphQlAppFixture rocketSurgeryWebAppFixture) : base(outputHelper, rocketSurgeryWebAppFixture)
    {
    }

    [Fact]
    public async Task Should_Remove_Rocket()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var id = await ServiceProvider.WithScoped<RocketDbContext>()
                                      .Invoke(
                                           async z =>
                                           {
                                               var faker = new RocketFaker();
                                               var rocket = faker.Generate();
                                               z.Add(rocket);

                                               await z.SaveChangesAsync().ConfigureAwait(false);
                                               return rocket.Id;
                                           }
                                       );

        await client.DeleteRocket.ExecuteAsync(new DeleteRocketRequest { Id = id.Value });

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.Rockets.Should().BeEmpty());
    }
}
