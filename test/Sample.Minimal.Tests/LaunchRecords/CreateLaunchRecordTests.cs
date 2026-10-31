using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Sample.Core.Domain;
using Sample.Restful.Client;
using Sample.Minimal.Tests.Helpers;

using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Minimal.Tests.LaunchRecords;

public class CreateLaunchRecordTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Create_A_LaunchRecord()
    {
        var client = new LaunchRecordClient(AlbaHost.Server.CreateClient());
        var clock = ServiceProvider.GetRequiredService<IClock>();
        var rocket = new ReadyRocket
        {
            Type = RocketType.Falcon9,
            SerialNumber = "12345678901234"
        };
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            z.Add(rocket);

            await z.SaveChangesAsync();
        }

        var response = await client.CreateLaunchRecordAsync(
            new CreateLaunchRecordRequest
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id.Value,
                ScheduledLaunchDate = clock.GetCurrentInstant().ToDateTimeOffset(),
                PayloadWeightKg = 100,
            }
        );

        response.Result.Id.ShouldNotBeEmpty();
    }

    private static readonly Faker Faker = new();
}
