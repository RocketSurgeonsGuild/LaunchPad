using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;

using Serilog.Events;

namespace Sample.Core.Tests.LaunchRecords;

public class ListLaunchRecordsTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_List_LaunchRecords()
    {
        await using (var seedScope = ServiceProvider.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rockets = faker.Generate(3);
            var records = new LaunchRecordFaker(rockets).Generate(10);
            context.AddRange(rockets);
            context.AddRange(records);
            await context.SaveChangesAsync(TestContext.CancellationToken);
        }

        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.CreateStream(new ListLaunchRecords.Request(null), TestContext.CancellationToken).ToListAsync(TestContext.CancellationToken);

        response.Count.ShouldBe(10);
    }

    private static readonly Faker Faker = new();
}
