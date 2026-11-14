using Microsoft.Extensions.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using Serilog.Events;
using ZeroAlloc.Mediator;

namespace Sample.Core.Tests.Rockets;

public class RemoveRocketsTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_Remove_Rocket()
    {
        RocketId id;
        await using (var seedScope = ServiceProvider.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rocket = faker.Generate();
            context.Add(rocket);

            await context.SaveChangesAsync(TestContext.CancellationToken);
            id = rocket.Id;
        }

        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await ( await mediator.Send(new DeleteRocket.Request(id), TestContext.CancellationToken) ).Value.Should().BeOfType(typeof(Unit));
        }

        await using var assertScope = ServiceProvider.CreateAsyncScope();
        await assertScope.ServiceProvider.GetRequiredService<RocketDbContext>().Rockets.Should().BeEmpty();
    }

    private static readonly Faker Faker = new();
}
