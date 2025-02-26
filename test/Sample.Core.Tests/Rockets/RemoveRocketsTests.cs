using MediatR;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;
using Serilog.Events;

namespace Sample.Core.Tests.Rockets;

public class RemoveRocketsTests(ITestContextAccessor testContext) : HandleTestHostBase(testContext, LogEventLevel.Verbose)
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
            mediator => mediator.Send(new DeleteRocket.Request(id))
        );

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.Rockets.ShouldBeEmpty());
    }

    private static readonly Faker Faker = new();
}
