using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Extensions;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using Serilog.Events;
using ZeroAlloc.Mediator;

namespace Sample.Core.Tests.LaunchRecords;

public class UpdateLaunchRecordTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    public async Task Should_Not_Patch_A_Missing_Launch_Record()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new EditLaunchRecord.PatchRequest(LaunchRecordId.Create()).ResetChanges(), TestContext.CancellationToken);

        await result.Value.Should().BeOfType(typeof(NotFound));
    }

    [Test]
    public async Task Should_Update_A_LaunchRecord()
    {
        LaunchRecord record;
        await using (var seedScope = ServiceProvider.CreateAsyncScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var clock = seedScope.ServiceProvider.GetRequiredService<IClock>();
            var rocket = new ReadyRocket
            {
                Id = RocketId.Create(),
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

        var launchDate = record.ScheduledLaunchDate.ToInstant().Plus(Duration.FromSeconds(1)).WithOffset(Offset.Zero);
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await Assert.That(( await mediator.Send(
            new EditLaunchRecord.Request
            {
                Id = record.Id,
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = record.RocketId,
                ScheduledLaunchDate = launchDate,
                PayloadWeightKg = 200,
            },
            TestContext.CancellationToken
        ) ).Value).IsTypeOf<LaunchRecordModel>();

        await response.ScheduledLaunchDate.Should().BeEqualTo(launchDate);
        await response.PayloadWeightKg.Should().BeEqualTo(200);
    }

    private static readonly Faker Faker = new();
}
