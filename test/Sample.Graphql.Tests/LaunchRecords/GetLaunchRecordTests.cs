﻿using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Graphql.Tests.Helpers;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.LaunchRecords;

public class GetLaunchRecordTests(ITestOutputHelper outputHelper, GraphQlAppFixture rocketSurgeryWebAppFixture)
    : GraphQlWebAppFixtureTest<GraphQlAppFixture>(outputHelper, rocketSurgeryWebAppFixture)
{
    [Fact]
    public async Task Should_Get_A_LaunchRecord()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var record = await ServiceProvider.WithScoped<RocketDbContext, IClock>()
                                          .Invoke(
                                               async (context, clock) =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Id = RocketId.New(),
                                                       Type = CoreRocketType.Falcon9,
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

        var response = await client.GetLaunchRecord.ExecuteAsync(record.Id.Value);
        response.EnsureNoErrors();

        response.Data!.LaunchRecords!.Nodes![0].Partner.ShouldBe("partner");
        response.Data.LaunchRecords!.Nodes[0].Payload.ShouldBe("geo-fence-ftl");
        response.Data.LaunchRecords!.Nodes[0].Rocket.Type.ShouldBe(RocketType.Falcon9);
        response.Data.LaunchRecords!.Nodes[0].Rocket.SerialNumber.ShouldBe("12345678901234");
        response.Data.LaunchRecords!.Nodes[0].ScheduledLaunchDate.ShouldBe(record.ScheduledLaunchDate);
    }

    private static readonly Faker Faker = new();
}
