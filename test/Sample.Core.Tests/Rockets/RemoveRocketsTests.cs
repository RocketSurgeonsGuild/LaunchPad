using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;

namespace Sample.Core.Tests.Rockets;

public class RemoveRocketsTests(ITestOutputHelper outputHelper) : HandleTestHostBase(outputHelper, LogLevel.Trace)
{
    [Fact]
    public async Task Should_Remove_Rocket()
    {
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

        await ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(new DeleteRocket.Request { Id = id })
        );

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.Rockets.Should().BeEmpty());
    }

    private static readonly Faker Faker = new();
}
