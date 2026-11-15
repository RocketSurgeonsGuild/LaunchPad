using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Minimal.Tests.Client;
using Sample.Minimal.Tests.Helpers;

using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Minimal.Tests.LaunchRecords;

public class UpdateLaunchRecordTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Update_A_LaunchRecord()
    {
        var client = Client;
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
        await client.EditLaunchRecord(
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

        var response = await client.GetLaunchRecord(record.Id.Value);

        await response.ScheduledLaunchDate.Should().BeEqualTo(launchDate);
        await response.PayloadWeightKg.Should().BeEqualTo(200);
    }

    private static readonly Faker Faker = new();
}
