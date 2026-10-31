using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

using Serilog.Events;

namespace Sample.Core.Tests.Rockets;

public class GetRocketTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_Get_A_Rocket()
    {
        RocketId rocket;
        await using (var seedScope = ServiceProvider.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var seeded = new ReadyRocket
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234",
            };
            context.Add(seeded);

            await context.SaveChangesAsync(TestContext.CancellationToken);
            rocket = seeded.Id;
        }

        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new GetRocket.Request(rocket), TestContext.CancellationToken);

        response.Type.ShouldBe(RocketType.Falcon9);
        response.Sn.ShouldBe("12345678901234");
    }

    [Test]
    public async Task Should_Not_Get_A_Missing_Rocket()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Func<Task> action = () => mediator.Send(new GetRocket.Request(RocketId.New()), TestContext.CancellationToken);

        await action.ShouldThrowAsync<NotFoundException>();
    }

    private static readonly Faker Faker = new();
}
