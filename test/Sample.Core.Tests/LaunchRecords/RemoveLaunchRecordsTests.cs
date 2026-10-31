using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;

using Serilog.Events;

namespace Sample.Core.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_Remove_LaunchRecord()
    {
        LaunchRecordId id;
        await using (var seedScope = ServiceProvider.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rocket = faker.Generate();
            var record = new LaunchRecordFaker(new[] { rocket, }.ToList()).Generate();
            context.Add(rocket);
            context.Add(record);

            await context.SaveChangesAsync(TestContext.CancellationToken);
            id = record.Id;
        }

        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new DeleteLaunchRecord.Request(id), TestContext.CancellationToken);
        }

        await using var assertScope = ServiceProvider.CreateAsyncScope();
        assertScope.ServiceProvider.GetRequiredService<RocketDbContext>().LaunchRecords.ShouldBeEmpty();
    }

    [Test]
    public async Task Should_Not_Be_Authorized_On_Given_Record()
    {
        var id = new LaunchRecordId(new("bad361de-a6d5-425a-9cf6-f9b2dd236be6"));
        await using (var seedScope = ServiceProvider.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rocket = faker.Generate();
            var record = new LaunchRecordFaker(new[] { rocket, }.ToList()).Generate();
            context.Add(rocket);
            record.Id = id;
            context.Add(record);

            await context.SaveChangesAsync(TestContext.CancellationToken);
        }

        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Func<Task> action = () => mediator.Send(new DeleteLaunchRecord.Request(id), TestContext.CancellationToken);
        await action.ShouldThrowAsync<NotAuthorizedException>();
    }

    private static readonly Faker Faker = new();
}
