using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.Rockets;

public class GetRocketTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Get_A_Rocket()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = CoreRocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket.Id;
                                               }
                                           );

        var response = await client.GetRocket.ExecuteAsync(rocket.Value);
        response.EnsureNoErrors();

        response.Data.Rockets.Items[0].Type.Should().Be(RocketType.Falcon9);
        response.Data.Rockets.Items[0].SerialNumber.Should().Be("12345678901234");
    }

    public GetRocketTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new();
}
