using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;
using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords;

public class GetLaunchRecordTests(ITestOutputHelper outputHelper, TestWebAppFixture testWebAppFixture)
    : WebAppFixtureTest<TestWebAppFixture>(outputHelper, testWebAppFixture)
{
    [Fact]
    public async Task Should_Get_A_LaunchRecord()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
        var record = await ServiceProvider
                          .WithScoped<RocketDbContext, IClock>()
                          .Invoke(
                               async (context, clock) =>
                               {
                                   var rocket = new ReadyRocket
                                   {
                                       Id = RocketId.New(),
                                       Type = Core.Domain.RocketType.Falcon9,
                                       SerialNumber = "12345678901234",
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

        var response = await client.GetLaunchRecordsAsync(new() { Id = record.Id.ToString(), });

        response.Partner.Should().Be("partner");
        response.Payload.Should().Be("geo-fence-ftl");
        response.RocketType.Should().Be(RocketType.Falcon9);
        response.RocketSerialNumber.Should().Be("12345678901234");
        response.ScheduledLaunchDate.ToDateTimeOffset().Should().BeCloseTo(record.ScheduledLaunchDate, TimeSpan.FromMilliseconds(1));
    }

    private static readonly Faker Faker = new();
}