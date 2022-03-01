using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Graphql.Tests.Rockets;

public class RemoveRocketsTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Remove_Rocket()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
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

    public RemoveRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new();
}
