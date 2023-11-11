using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;
using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords;

public class CreateLaunchRecordTests(ITestOutputHelper outputHelper, TestWebAppFixture testWebAppFixture)
    : WebAppFixtureTest<TestWebAppFixture>(outputHelper, testWebAppFixture)
{
    private static readonly Faker Faker = new();

    [Fact]
    public async Task Should_Create_A_LaunchRecord()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
        var clock = ServiceProvider.GetRequiredService<IClock>();
        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = Core.Domain.RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket;
                                               }
                                           );


        var response = await client.CreateLaunchRecordAsync(
            new CreateLaunchRecordRequest
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id.ToString(),
                ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset().ToTimestamp(),
                PayloadWeightKg = 100,
            }
        );

        response.Id.Should().NotBeEmpty();
    }
}
