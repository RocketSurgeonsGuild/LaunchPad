using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Graphql.Tests.Helpers;

using CoreRocketType = Sample.Core.Domain.RocketType;
using GraphQlRocketType = Sample.Graphql.Tests.RocketType;

namespace Sample.Graphql.Tests.LaunchRecords;

public class GetLaunchRecordTests : GraphQlWebAppFixtureTest
{
    [Test]
    public async Task Should_Get_A_LaunchRecord()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        LaunchRecord record;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var clock = scope.ServiceProvider.GetRequiredService<IClock>();
            var rocket = new ReadyRocket
            {
                Id = RocketId.New(),
                Type = CoreRocketType.Falcon9,
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

            await context.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client.GetLaunchRecord.ExecuteAsync(record.Id.Value,
            cancellationToken: TestContext.CancellationToken);
        response.EnsureNoErrors();

        response.Data!.LaunchRecords!.Nodes![0].Partner.ShouldBe("partner");
        response.Data.LaunchRecords!.Nodes[0].Payload.ShouldBe("geo-fence-ftl");
        response.Data.LaunchRecords!.Nodes[0].Rocket.Type.ShouldBe(GraphQlRocketType.Falcon9);
        response.Data.LaunchRecords!.Nodes[0].Rocket.SerialNumber.ShouldBe("12345678901234");
        response.Data.LaunchRecords!.Nodes[0].ScheduledLaunchDate.ShouldBe(record.ScheduledLaunchDate);
    }

    private static readonly Faker Faker = new();
}
