using Grpc.Core;

using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;

using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class UpdateRocketTests : WebAppFixtureTest
{
    private static readonly Faker Faker = new();

    [Test]
    public async Task Should_Update_A_Rocket()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());

        ReadyRocket rocket;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            rocket = new ReadyRocket
            {
                Type = Core.Domain.RocketType.Falcon9,
                SerialNumber = "12345678901234"
            };
            z.Add(rocket);

            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var u = await client.EditRocketAsync(
            new UpdateRocketRequest
            {
                Id = rocket.Id.ToString(),
                Type = RocketType.FalconHeavy,
                SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
            },
            cancellationToken: TestContext.CancellationToken
        );
        u.ShouldNotBeNull();

        // var response = await client.GetRocketAsync(rocket.Id);
        //
        // response.Type.ShouldBe(RocketType.FalconHeavy);
        // response.Sn.ShouldBe("43210987654321");
    }

    [Test]
    [MethodDataSource(nameof(RequiredFieldData))]
    public async Task Should_Validate_Required_Fields(UpdateRocketRequest request, string propertyName)
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());
        Func<Task> a = async () => await client.EditRocketAsync(request);
        var e = (await a.ShouldThrowAsync<RpcException>());
        e.Status.StatusCode.ShouldBe(StatusCode.InvalidArgument);
        e.Message.ShouldContain(propertyName);
    }

    public static IEnumerable<Func<(UpdateRocketRequest Request, string PropertyName)>> RequiredFieldData()
    {
        yield return () => (
            new UpdateRocketRequest(),
            nameof(UpdateRocketRequest.SerialNumber)
        );
        yield return () => (
            new UpdateRocketRequest { SerialNumber = Faker.Random.String2(0, 9) },
            nameof(UpdateRocketRequest.SerialNumber)
        );
        yield return () => (
            new UpdateRocketRequest { SerialNumber = Faker.Random.String2(600, 800) },
            nameof(UpdateRocketRequest.SerialNumber)
        );
    }
}
