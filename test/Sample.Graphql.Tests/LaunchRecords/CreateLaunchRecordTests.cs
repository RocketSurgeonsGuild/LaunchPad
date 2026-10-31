using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.LaunchRecords;

public class CreateLaunchRecordTests : GraphQlWebAppFixtureTest
{
    private static readonly Faker Faker = new();

    [Test]
    public async Task Should_Create_A_LaunchRecord()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var clock = ServiceProvider.GetRequiredService<IClock>();
        var rocket = new ReadyRocket
        {
            Type = CoreRocketType.Falcon9,
            SerialNumber = "12345678901234"
        };
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            z.Add(rocket);

            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client.CreateLaunchRecord.ExecuteAsync(
            new CreateLaunchRecordRequest
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id.Value,
                ScheduledLaunchDate = clock.GetCurrentInstant(),
                PayloadWeightKg = 100,
            },
            cancellationToken: TestContext.CancellationToken
        );
        response.EnsureNoErrors();

        response.Data!.CreateLaunchRecord.Id.ShouldNotBe(Guid.Empty);
    }
}
