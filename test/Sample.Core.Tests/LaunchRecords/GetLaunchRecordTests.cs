using MediatR;

using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;

using Serilog.Events;

namespace Sample.Core.Tests.LaunchRecords;

public class GetLaunchRecordTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_Get_A_LaunchRecord()
    {
        LaunchRecord record;
        await using (var seedScope = ServiceProvider.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var clock = seedScope.ServiceProvider.GetRequiredService<IClock>();
            var rocket = new ReadyRocket
            {
                Id = RocketId.New(),
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234",
            };

            record = new LaunchRecord
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id,
                ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset(),
                PayloadWeightKg = 100,
            };
            context.Add(rocket);
            context.Add(record);

            await context.SaveChangesAsync(TestContext.CancellationToken);
        }

        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new GetLaunchRecord.Request(record.Id), TestContext.CancellationToken);

        response.Partner.ShouldBe("partner");
        response.Payload.ShouldBe("geo-fence-ftl");
        response.RocketType.ShouldBe(RocketType.Falcon9);
        response.RocketSerialNumber.ShouldBe("12345678901234");
        response.ScheduledLaunchDate.ShouldBe(Instant.FromDateTimeOffset(record.ScheduledLaunchDate));
    }

    [Test]
    public async Task Should_Not_Get_A_Missing_Launch_Record()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Func<Task> action = () => mediator.Send(new GetLaunchRecord.Request(LaunchRecordId.New()), TestContext.CancellationToken);

        await action.ShouldThrowAsync<NotFoundException>();
    }

    private static readonly Faker Faker = new();
}
