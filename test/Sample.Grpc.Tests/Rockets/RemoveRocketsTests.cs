using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;

using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class RemoveRocketsTests : WebAppFixtureTest
{
    private static readonly Faker Faker = new();

    [Test]
    public async Task Should_Remove_Rocket()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());
        ReadyRocket rocket;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            rocket = faker.Generate();
            z.Add(rocket);

            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        await client.DeleteRocketAsync(new DeleteRocketRequest { Id = rocket.Id.ToString() },
            cancellationToken: TestContext.CancellationToken);

        await using var assertScope = ServiceProvider.CreateAsyncScope();
        assertScope.ServiceProvider.GetRequiredService<RocketDbContext>().Rockets.ShouldBeEmpty();
    }
}
