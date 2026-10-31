using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;

using CoreRocketType = Sample.Core.Domain.RocketType;
using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords;

public class UpdateLaunchRecordTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Update_A_LaunchRecord()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
        LaunchRecord record;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var clk = scope.ServiceProvider.GetRequiredService<IClock>();
            var rocket = new ReadyRocket
            {
                Id = RocketId.New(),
                Type = CoreRocketType.Falcon9,
                SerialNumber = "12345678901234",
            };
            context.Add(rocket);

            record = new LaunchRecord
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id,
                Rocket = rocket,
                ScheduledLaunchDate = clk.GetCurrentInstant().ToDateTimeOffset(),
                PayloadWeightKg = 100,
            };
            context.Add(record);

            await context.SaveChangesAsync(TestContext.CancellationToken);
        }

        var launchDate = record.ScheduledLaunchDate.AddSeconds(1).ToTimestamp();
        await client.EditLaunchRecordAsync(
            new()
            {
                Id = record.Id.ToString(),
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = record.RocketId.ToString(),
                ScheduledLaunchDate = launchDate,
                PayloadWeightKg = 200,
            },
            cancellationToken: TestContext.CancellationToken
        );

        var response = await client.GetLaunchRecordsAsync(new() { Id = record.Id.ToString(), }, cancellationToken: TestContext.CancellationToken);

        response.ScheduledLaunchDate.ShouldBe(Timestamp.FromDateTimeOffset(launchDate.ToDateTimeOffset()));
        response.PayloadWeightKg.ShouldBe(200);
    }

    private static readonly Faker Faker = new();
}
