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
                Id = RocketId.Create(),
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

        var response = await client.GetLaunchRecord.ExecuteAsync(record.Id.ToString(),
            cancellationToken: TestContext.CancellationToken);
        response.EnsureNoErrors();

        await response.Data!.LaunchRecords!.Nodes![0].Partner.Should().BeEqualTo("partner");
        await response.Data.LaunchRecords!.Nodes[0].Payload.Should().BeEqualTo("geo-fence-ftl");
        await response.Data.LaunchRecords!.Nodes[0].Rocket.Type.Should().BeEqualTo(GraphQlRocketType.Falcon9);
        await response.Data.LaunchRecords!.Nodes[0].Rocket.SerialNumber.Should().BeEqualTo("12345678901234");
        await response.Data.LaunchRecords!.Nodes[0].ScheduledLaunchDate.ToDateTimeOffset().Should().BeEqualTo(record.ScheduledLaunchDate);
    }

    private static readonly Faker Faker = new();
}
