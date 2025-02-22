﻿using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Restful.Client;

namespace Sample.Restful.Tests.Rockets;

public class RemoveRocketsTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Remove_Rocket()
    {
        var client = new RocketClient(Factory.CreateClient());
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

        await client.RemoveRocketAsync(id.Value);

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.Rockets.ShouldBeEmpty());
    }

    public RemoveRocketsTests(ITestOutputHelper outputHelper, TestWebHost host) : base(outputHelper, host)
    {
    }

    private static readonly Faker Faker = new();
}
