using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using Serilog.Events;
using ZeroAlloc.Mediator;

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

        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await Assert.That(( await mediator.Send(new GetLaunchRecord.Request(record.Id), TestContext.CancellationToken) ).Value).IsTypeOf<LaunchRecordModel>();

        await response.Partner.Should().BeEqualTo("partner");
        await response.Payload.Should().BeEqualTo("geo-fence-ftl");
        await response.RocketType.Should().BeEqualTo(RocketType.Falcon9);
        await response.RocketSerialNumber.Should().BeEqualTo("12345678901234");
        await response.ScheduledLaunchDate.Should().BeEqualTo(OffsetDateTime.FromDateTimeOffset(record.ScheduledLaunchDate));
    }

    [Test]
    public async Task Should_Not_Get_A_Missing_Launch_Record()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GetLaunchRecord.Request(LaunchRecordId.Create()), TestContext.CancellationToken);

        await result.Value.Should().BeOfType(typeof(NotFound));
    }

    private static readonly Faker Faker = new();
}
