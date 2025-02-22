using Sample.Restful.Client;

namespace Sample.Restful.Tests.Rockets;

public class CreateRocketTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Create_A_Rocket()
    {
        var client = new RocketClient(Factory.CreateClient());

        var response = await client.CreateRocketAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );

        response.Result.Id.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Should_Throw_If_Rocket_Exists()
    {
        var client = new RocketClient(Factory.CreateClient());
        await client.CreateRocketAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );

        Func<Task> action = () => client.CreateRocketAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );
        var r = (await action.ShouldThrowAsync<ApiException<ProblemDetails>>())
               .And.Result;
        r.Title.ShouldBe("Rocket Creation Failed");
    }

    public CreateRocketTests(ITestOutputHelper testOutputHelper, TestWebHost host) : base(testOutputHelper, host)
    {
    }
}
