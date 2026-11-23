using Grpc.Core;

using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;

using CoreRocketType = Sample.Core.Domain.RocketType;
using GrpcRocketType = Sample.Grpc.RocketType;
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

        await response.Type.Should().BeEqualTo(GrpcRocketType.Falcon9);
        await response.Sn.Should().BeEqualTo("12345678901234");
    }

    // A missing or malformed ID maps to the default ID, and the empty Guid is read like any other (RejectEmpty = false): the IsAssigned rule of
    // the operation rejects both, before the handler looks the ID up.
    [Test]
    [Arguments("")]
    [Arguments("not-a-guid")]
    [Arguments("00000000-0000-0000-0000-000000000000")]
    public async Task Should_Reject_An_Unassigned_Id(string id)
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());

        Func<Task> a = async () => await client.GetRocketsAsync(new GetRocketRequest { Id = id }, cancellationToken: TestContext.CancellationToken);

        var e = await a.Should().Throw<RpcException>();
        await e.Status.StatusCode.Should().BeEqualTo(StatusCode.InvalidArgument);
        await e.Message.Should().Contain(nameof(GetRocketRequest.Id));
    }
}
