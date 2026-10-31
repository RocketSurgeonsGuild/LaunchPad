using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;

using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords;

public class CreateLaunchRecordTests : WebAppFixtureTest
{
    private static readonly Faker Faker = new();

    [Test]
    public async Task Should_Create_A_LaunchRecord()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
        var clock = ServiceProvider.GetRequiredService<IClock>();
        var rocket = new ReadyRocket
        {
            Type = Core.Domain.RocketType.Falcon9,
            SerialNumber = "12345678901234"
        };
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            z.Add(rocket);

            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client.CreateLaunchRecordAsync(
            new CreateLaunchRecordRequest
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id.ToString(),
                ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset().ToTimestamp(),
                PayloadWeightKg = 100,
            },
            cancellationToken: TestContext.CancellationToken
        );

        response.Id.ShouldNotBeEmpty();
    }
}
