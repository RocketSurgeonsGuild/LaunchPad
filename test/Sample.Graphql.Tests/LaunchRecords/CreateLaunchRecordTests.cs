using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.LaunchRecords;

public class CreateLaunchRecordTests(ITestOutputHelper outputHelper, GraphQlAppFixture rocketSurgeryWebWebAppFixture)
    : GraphQlWebAppFixtureTest<GraphQlAppFixture>(outputHelper, rocketSurgeryWebWebAppFixture)
{
    private static readonly Faker Faker = new();

    [Fact]
    public async Task Should_Create_A_LaunchRecord()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var clock = ServiceProvider.GetRequiredService<IClock>();
        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = CoreRocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket;
                                               }
                                           );


        var response = await client.CreateLaunchRecord.ExecuteAsync(
            new CreateLaunchRecordRequest
            {
                Partner = "partner",
                Payload = "geo-fence-ftl",
                RocketId = rocket.Id.Value,
                ScheduledLaunchDate = clock.GetCurrentInstant(),
                PayloadWeightKg = 100,
            }
        );
        response.EnsureNoErrors();

        response.Data!.CreateLaunchRecord.Id.Should().NotBeEmpty();
    }
}
