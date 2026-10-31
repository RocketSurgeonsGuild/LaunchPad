using Microsoft.Extensions.DependencyInjection;

using NodaTime;
using NodaTime.Extensions;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Graphql.Tests.Helpers;

using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.LaunchRecords;

public class UpdateLaunchRecordTests : GraphQlWebAppFixtureTest
{
    [Test]
    public async Task Should_Update_A_LaunchRecord()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        LaunchRecord record;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var clk = scope.ServiceProvider.GetRequiredService<IClock>();
            var rocket = new ReadyRocket
            {
                Id = RocketId.New(),
                Type = CoreRocketType.Falcon9,
                SerialNumber = "12345678901234"
            };

            record = new LaunchRecord
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id,
                ScheduledLaunchDate = clk.GetCurrentInstant().ToDateTimeOffset(),
                PayloadWeightKg = 100,
            };
            context.Add(rocket);
            context.Add(record);

            await context.SaveChangesAsync(TestContext.CancellationToken);
        }

        var launchDate = record.ScheduledLaunchDate.AddSeconds(1);
        await client.UpdateLaunchRecord.ExecuteAsync(
            new EditLaunchRecordRequest
            {
                Id = record.Id.Value,
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = record.RocketId.Value,
                ScheduledLaunchDate = launchDate.ToInstant(),
                PayloadWeightKg = 200,
            }, TestContext.CancellationToken
        );

        var response = await client.GetLaunchRecord.ExecuteAsync(record.Id.Value, TestContext.CancellationToken);
        response.EnsureNoErrors();

        response.Data!.LaunchRecords!.Nodes![0].ScheduledLaunchDate.ShouldBe(launchDate);
        response.Data.LaunchRecords!.Nodes[0].PayloadWeightKg.ShouldBe(200);
    }

    private static readonly Faker Faker = new();
}
