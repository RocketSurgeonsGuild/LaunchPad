using Grpc.Core;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;

namespace Sample.Grpc.Tests.Validation.Integration;

public class CustomValidatorIntegrationTest(ITestContextAccessor testContext, TestWebAppFixture factory)
    : WebAppFixtureTest<TestWebAppFixture>(testContext, factory)
{
    [Fact]
    public async Task Should_ResponseMessage_When_MessageIsValid()
    {
        await AlbaHost.Services.WithScoped<RocketDbContext>()
                      .Invoke(
                           async z =>
                           {
                               var faker = new RocketFaker();
                               var rockets = faker.Generate(3);
                               var records = new LaunchRecordFaker(rockets).Generate(10);
                               z.AddRange(rockets);
                               z.AddRange(records);
                               await z.SaveChangesAsync();
                           }
                       );

        // Given
        var client = new Grpc.Rockets.RocketsClient(AlbaHost.CreateGrpcChannel());

        // When
        var response = await client.ListRockets(new ListRocketsRequest()).ResponseStream.ReadAllAsync().ToListAsync();

        // Then nothing happen.
        response.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
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
        var rpcException = await Assert.ThrowsAsync<RpcException>(Action);
        Assert.Equal(StatusCode.InvalidArgument, rpcException.Status.StatusCode);
    }
}
