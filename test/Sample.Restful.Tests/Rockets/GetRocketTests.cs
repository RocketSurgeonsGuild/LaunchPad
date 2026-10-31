using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;
using Sample.Restful.Client;
using Sample.Restful.Tests.Helpers;

using HttpRocketType = Sample.Restful.Client.RocketType;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Restful.Tests.Rockets;

public class GetRocketTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Get_A_Rocket()
    {
        var client = new RocketClient(AlbaHost.Server.CreateClient());
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

        var response = await client.GetRocketAsync(rocket.Id.Value);

        response.Result.Type.ShouldBe(HttpRocketType.Falcon9);
        response.Result.Sn.ShouldBe("12345678901234");
    }

    [Test]
    public async Task Should_Not_Get_A_Missing_Rocket()
    {
        var client = new RocketClient(AlbaHost.Server.CreateClient());

        Func<Task> action = () => client.GetRocketAsync(Guid.NewGuid());
        await action.ShouldThrowAsync<ApiException<ProblemDetails>>()
                    .Where(
                         z => z.StatusCode == 404 && z.Result.Status == 404 && z.Result.Title == "Not Found"
                     );
    }

    private static readonly Faker Faker = new();
}
