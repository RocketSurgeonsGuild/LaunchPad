using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Extensions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;

namespace Sample.Core.Tests.LaunchRecords;

public class UpdateLaunchRecordTests : HandleTestHostBase
{
    [Fact]
    public async Task Should_Update_A_LaunchRecord()
    {
        var record = await ServiceProvider.WithScoped<RocketDbContext, IClock>()
                                          .Invoke(
                                               async (context, clock) =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Id = RocketId.New(),
                                                       Type = RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };

                                                   var record = new LaunchRecord
                                                   {
                                                       Partner = "partner",
                                                       Payload = "geo-fence-ftl",
                                                       RocketId = rocket.Id,
                                                       ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset(),
                                                       PayloadWeightKg = 100,
                                                   };
                                                   context.Add(rocket);
                                                   context.Add(record);

                                                   await context.SaveChangesAsync();
                                                   return record;
                                               }
                                           );

        var launchDate = record.ScheduledLaunchDate.ToInstant().Plus(Duration.FromSeconds(1));
        var response = await ServiceProvider.WithScoped<IMediator, IClock>().Invoke(
            (mediator, clock) => mediator.Send(
                new EditLaunchRecord.Request
                {
                    Id = record.Id,
                    Partner = "partner",
                    Payload = "geo-fence-ftl",
                    RocketId = record.RocketId,
                    ScheduledLaunchDate = launchDate,
                    PayloadWeightKg = 200,
                }
            )
        );

        response.ScheduledLaunchDate.Should().Be(launchDate);
        response.PayloadWeightKg.Should().Be(200);
    }

    public UpdateLaunchRecordTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace)
    {
    }

    private static readonly Faker Faker = new();
}
