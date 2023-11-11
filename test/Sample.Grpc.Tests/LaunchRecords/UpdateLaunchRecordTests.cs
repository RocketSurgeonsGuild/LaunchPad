using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;
using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords;

public class UpdateLaunchRecordTests(ITestOutputHelper outputHelper, TestWebAppFixture webAppFixture)
    : WebAppFixtureTest<TestWebAppFixture>(outputHelper, webAppFixture)
{
    [Fact]
    public async Task Should_Update_A_LaunchRecord()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
        var clock = ServiceProvider.GetRequiredService<IClock>();
        var record = await ServiceProvider.WithScoped<RocketDbContext, IClock>()
                                          .Invoke(
                                               async (context, clk) =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Id = RocketId.New(),
                                                       Type = Core.Domain.RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   context.Add(rocket);

                                                   var record = new LaunchRecord
                                                   {
                                                       Partner = "partner",
                                                       Payload = "geo-fence-ftl",
                                                       RocketId = rocket.Id,
                                                       Rocket = rocket,
                                                       ScheduledLaunchDate = clk.GetCurrentInstant().ToDateTimeOffset(),
                                                       PayloadWeightKg = 100,
                                                   };
                                                   context.Add(record);

                                                   await context.SaveChangesAsync();
                                                   return record;
                                               }
                                           );

        var launchDate = record.ScheduledLaunchDate.AddSeconds(1).ToTimestamp();
        await client.EditLaunchRecordAsync(
            new UpdateLaunchRecordRequest
            {
                Id = record.Id.ToString(),
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = record.RocketId.ToString(),
                ScheduledLaunchDate = launchDate,
                PayloadWeightKg = 200,
            }
        );

        var response = await client.GetLaunchRecordsAsync(new GetLaunchRecordRequest { Id = record.Id.ToString() });

        response.ScheduledLaunchDate.Should().Be(launchDate);
        response.PayloadWeightKg.Should().Be(200);
    }

    private static readonly Faker Faker = new();
}
