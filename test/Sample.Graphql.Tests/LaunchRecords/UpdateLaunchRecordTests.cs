using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Extensions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.LaunchRecords;

public class UpdateLaunchRecordTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Update_A_LaunchRecord()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
        var record = await ServiceProvider.WithScoped<RocketDbContext, IClock>()
                                          .Invoke(
                                               async (context, clk) =>
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
                                                       ScheduledLaunchDate = clk.GetCurrentInstant().ToDateTimeOffset(),
                                                       PayloadWeightKg = 100,
                                                   };
                                                   context.Add(rocket);
                                                   context.Add(record);

                                                   await context.SaveChangesAsync();
                                                   return record;
                                               }
                                           );

        var launchDate = record.ScheduledLaunchDate.AddSeconds(1);
        await client.UpdateLaunchRecord.ExecuteAsync(
            new EditLaunchRecordRequest
            {
                Id = record.Id.Value,
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = record.RocketId.Value,
                ScheduledLaunchDate = launchDate.ToString("O"),
                PayloadWeightKg = 200,
            }
        );

        var response = await client.GetLaunchRecord.ExecuteAsync(record.Id.Value);
        response.EnsureNoErrors();

        response.Data!.LaunchRecords!.Nodes![0].ScheduledLaunchDate.Should().Be(launchDate);
        response.Data.LaunchRecords!.Nodes[0].PayloadWeightKg.Should().Be(200);
    }

    public UpdateLaunchRecordTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new();
}
