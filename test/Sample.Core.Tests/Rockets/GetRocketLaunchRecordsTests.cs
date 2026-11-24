using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using Serilog.Events;
using ZeroAlloc.Mediator;

namespace Sample.Core.Tests.Rockets;

public class GetRocketLaunchRecordsTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_Answer_Not_Found_As_The_Only_Item_For_A_Missing_Rocket()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var items = await mediator
                         .CreateStream(new GetRocketLaunchRecords.Request { Id = RocketId.Create(), }, TestContext.CancellationToken)
                         .ToListAsync(TestContext.CancellationToken);

        var item = await items.Should().HaveSingleItem();
        await item.Value.Should().BeOfType(typeof(NotFound));
    }

    [Test]
    public async Task Should_Stream_No_Items_For_A_Rocket_Without_Launch_Records()
    {
        RocketId id;
        await using (var seed = ServiceProvider.CreateAsyncScope())
        {
            var context = seed.ServiceProvider.GetRequiredService<RocketDbContext>();
            var rocket = new ReadyRocket { Type = RocketType.Falcon9, SerialNumber = "12345678901234", };
            context.Add(rocket);
            await context.SaveChangesAsync(TestContext.CancellationToken);
            id = rocket.Id;
        }

        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var items = await mediator
                         .CreateStream(new GetRocketLaunchRecords.Request { Id = id, }, TestContext.CancellationToken)
                         .ToListAsync(TestContext.CancellationToken);

        await items.Should().BeEmpty();
    }
}
