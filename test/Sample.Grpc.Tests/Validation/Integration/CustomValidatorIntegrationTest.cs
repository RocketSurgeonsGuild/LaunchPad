using Grpc.Core;

using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;

namespace Sample.Grpc.Tests.Validation.Integration;

public class CustomValidatorIntegrationTest : WebAppFixtureTest
{
    [Test]
    public async Task Should_ResponseMessage_When_MessageIsValid()
    {
        await using (var scope = AlbaHost.Services.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rockets = faker.Generate(3);
            var records = new LaunchRecordFaker(rockets).Generate(10);
            z.AddRange(rockets);
            z.AddRange(records);
            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        // Given
        var client = new Grpc.Rockets.RocketsClient(AlbaHost.CreateGrpcChannel());

        // When
        var response = await client.ListRockets(new ListRocketsRequest()).ResponseStream.ReadAllAsync(TestContext.CancellationToken).ToListAsync(TestContext.CancellationToken);

        // Then nothing happen.
        response.Count.ShouldBeGreaterThan(0);
    }

    [Test]
    public async Task Should_ThrowInvalidArgument_When_NameOfMessageIsEmpty()
    {
        // Given
        var client = new Grpc.Rockets.RocketsClient(AlbaHost.CreateGrpcChannel());

        // When
        async Task Action()
        {
            await client.GetRocketsAsync(new GetRocketRequest { Id = "" });
        }

        // Then
        var rpcException = await Should.ThrowAsync<RpcException>(Action);
        rpcException.Status.StatusCode.ShouldBe(StatusCode.InvalidArgument);
    }
}
