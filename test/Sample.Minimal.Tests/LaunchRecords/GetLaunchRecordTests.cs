using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Minimal.Tests.Helpers;

using HttpRocketType = Sample.Minimal.Tests.Client.RocketType;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Minimal.Tests.LaunchRecords;

public class GetLaunchRecordTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Get_A_LaunchRecord()
    {
        var client = Client;
        LaunchRecord record;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var clock = scope.ServiceProvider.GetRequiredService<IClock>();
            var rocket = new ReadyRocket
            {
                Id = RocketId.Create(),
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

        var response = await client.GetLaunchRecord(record.Id.Value);

        await response.Partner.Should().BeEqualTo("partner");
        await response.Payload.Should().BeEqualTo("geo-fence-ftl");
        await response.RocketType.Should().BeEqualTo(HttpRocketType.Falcon9);
        await response.RocketSerialNumber.Should().BeEqualTo("12345678901234");
        await response.ScheduledLaunchDate.Should().BeEqualTo(record.ScheduledLaunchDate);
    }

    [Test]
    public async Task Should_Not_Get_A_Missing_Launch_Record()
    {
        var client = Client;

        var (status, problem) = await Failure<ProblemDetails>(() => client.GetLaunchRecord(Guid.NewGuid()));
        await status.Should().BeEqualTo(System.Net.HttpStatusCode.NotFound);
        await problem!.Status.Should().BeEqualTo(404);
        await problem.Title.Should().BeEqualTo("Not Found");
    }

    private static readonly Faker Faker = new();
}
