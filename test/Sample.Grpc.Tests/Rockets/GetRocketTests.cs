using Bogus;
using FluentAssertions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Xunit;
using Xunit.Abstractions;
using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class GetRocketTests : HandleGrpcHostBase
{
    [Fact]
    public async Task Should_Get_A_Rocket()
    {
        var client = new R.RocketsClient(Factory.CreateGrpcChannel());
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

    public GetRocketTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new Faker();
}
