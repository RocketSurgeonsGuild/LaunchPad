using Grpc.Core;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;

namespace Sample.Grpc.Tests.Validation.Integration;

public class CustomValidatorIntegrationTest : WebAppFixtureTest<TestWebAppFixture>
{
    public CustomValidatorIntegrationTest(ITestOutputHelper testOutputHelper, TestWebAppFixture factory) : base(testOutputHelper, factory) { }
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
        response.Should().HaveCountGreaterThan(0);
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
