using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;

using CoreRocketType = Sample.Core.Domain.RocketType;
using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class GetRocketTests : WebAppFixtureTest
{
    private static readonly Faker Faker = new();

    [Test]
    public async Task Should_Get_A_Rocket()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());
        ReadyRocket rocket;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            rocket = new ReadyRocket
            {
                Type = CoreRocketType.Falcon9,
                SerialNumber = "12345678901234"
            };
            z.Add(rocket);

            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client.GetRocketsAsync(new GetRocketRequest { Id = rocket.Id.ToString() },
            cancellationToken: TestContext.CancellationToken);

        response.Type.ShouldBe(RocketType.Falcon9);
        response.Sn.ShouldBe("12345678901234");
    }
}
