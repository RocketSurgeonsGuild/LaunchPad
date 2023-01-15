using DryIoc;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;
using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class GetRocketTests : WebAppFixtureTest<TestWebAppFixture>
{
    private static readonly Faker Faker = new();

    public GetRocketTests(ITestOutputHelper outputHelper, TestWebAppFixture webAppFixture) : base(outputHelper, webAppFixture)
    {
    }

    [Fact]
    public async Task Should_Get_A_Rocket()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());
        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = Core.Domain.RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket.Id;
                                               }
                                           );

        var response = await client.GetRocketsAsync(new GetRocketRequest { Id = rocket.ToString() });

        response.Type.Should().Be(RocketType.Falcon9);
        response.Sn.Should().Be("12345678901234");
    }
}
