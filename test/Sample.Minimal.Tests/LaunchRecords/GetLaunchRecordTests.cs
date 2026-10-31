using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Restful.Client;
using Sample.Minimal.Tests.Helpers;

using HttpRocketType = Sample.Restful.Client.RocketType;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Minimal.Tests.LaunchRecords;

public class GetLaunchRecordTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Get_A_LaunchRecord()
    {
        var client = new LaunchRecordClient(AlbaHost.Server.CreateClient());
        LaunchRecord record;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var clock = scope.ServiceProvider.GetRequiredService<IClock>();
            var rocket = new ReadyRocket
            {
                Id = RocketId.New(),
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

        var response = (await client.GetLaunchRecordAsync(record.Id.Value)).Result;

        response.Partner.ShouldBe("partner");
        response.Payload.ShouldBe("geo-fence-ftl");
        response.RocketType.ShouldBe(HttpRocketType.Falcon9);
        response.RocketSerialNumber.ShouldBe("12345678901234");
        response.ScheduledLaunchDate.ShouldBe(record.ScheduledLaunchDate);
    }

    [Test]
    public async Task Should_Not_Get_A_Missing_Launch_Record()
    {
        var client = new LaunchRecordClient(AlbaHost.Server.CreateClient());

        Func<Task> action = () => client.GetLaunchRecordAsync(Guid.NewGuid());
        await action.ShouldThrowAsync<ApiException<ProblemDetails>>()
                    .Where(
                         z => z.StatusCode == 404 && z.Result.Status == 404 && z.Result.Title == "Not Found"
                     );
    }

    private static readonly Faker Faker = new();
}
