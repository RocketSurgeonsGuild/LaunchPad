﻿using MediatR;
using NodaTime;
using NodaTime.Extensions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using Serilog.Events;

namespace Sample.Core.Tests.LaunchRecords;

public class UpdateLaunchRecordTests(ITestContextAccessor testContext) : HandleTestHostBase(testContext, LogEventLevel.Verbose)
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
            (mediator, _) => mediator.Send(
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

        response.ScheduledLaunchDate.ShouldBe(launchDate);
        response.PayloadWeightKg.ShouldBe(200);
    }

    private static readonly Faker Faker = new();
}
