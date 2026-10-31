using MediatR;

using Microsoft.Extensions.DependencyInjection;

using NodaTime;
using NodaTime.Extensions;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;

using Serilog.Events;

namespace Sample.Core.Tests.LaunchRecords;

public class UpdateLaunchRecordTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
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

        var launchDate = record.ScheduledLaunchDate.ToInstant().Plus(Duration.FromSeconds(1));
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.Send(
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
        );

        response.ScheduledLaunchDate.ShouldBe(launchDate);
        response.PayloadWeightKg.ShouldBe(200);
    }

    private static readonly Faker Faker = new();
}
