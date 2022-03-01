using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.LaunchRecords;

public class GetLaunchRecordTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Get_A_LaunchRecord()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
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

        response.Data.LaunchRecords.Items[0].Partner.Should().Be("partner");
        response.Data.LaunchRecords.Items[0].Payload.Should().Be("geo-fence-ftl");
        response.Data.LaunchRecords.Items[0].Rocket.Type.Should().Be(RocketType.Falcon9);
        response.Data.LaunchRecords.Items[0].Rocket.SerialNumber.Should().Be("12345678901234");
        response.Data.LaunchRecords.Items[0].ScheduledLaunchDate.Should().Be(record.ScheduledLaunchDate);
    }

    public GetLaunchRecordTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new();
}
