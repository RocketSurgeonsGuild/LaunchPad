using Microsoft.Extensions.DependencyInjection;

using NodaTime;
using NodaTime.Extensions;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Restful.Client;
using Sample.Minimal.Tests.Helpers;

using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Minimal.Tests.LaunchRecords;

public class UpdateLaunchRecordTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Update_A_LaunchRecord()
    {
        var client = new LaunchRecordClient(AlbaHost.Server.CreateClient());
        LaunchRecord record;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var clock = scope.ServiceProvider.GetRequiredService<IClock>();
            var rocket = new ReadyRocket
            {
                Id = RocketId.New(),
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
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

            await context.SaveChangesAsync();
        }

        var launchDate = record.ScheduledLaunchDate.AddSeconds(1);
        await client.EditLaunchRecordAsync(
            record.Id.Value,
            new EditLaunchRecordRequest
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = record.RocketId.Value,
                ScheduledLaunchDate = launchDate,
                PayloadWeightKg = 200,
            }
        );

        var response = await client.GetLaunchRecordAsync(record.Id.Value);

        response.Result.ScheduledLaunchDate.ShouldBe(launchDate);
        response.Result.PayloadWeightKg.ShouldBe(200);
    }

    private static readonly Faker Faker = new();
}
