using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;

using Serilog.Events;

namespace Sample.Core.Tests.Rockets;

public class ListRocketsTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_List_Rockets()
    {
        await using (var seedScope = ServiceProvider.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            context.AddRange(faker.Generate(10));

            await context.SaveChangesAsync(TestContext.CancellationToken);
        }

        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.CreateStream(new ListRockets.Request(null), TestContext.CancellationToken).ToListAsync(TestContext.CancellationToken);

        response.Count.ShouldBe(10);
    }

    private static readonly Faker Faker = new();
}
