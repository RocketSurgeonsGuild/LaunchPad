using Bogus;
using FluentAssertions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Restful.Client;
using Xunit;
using Xunit.Abstractions;
using HttpRocketType = Sample.Restful.Client.RocketType;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Restful.Tests.Rockets;

public class GetRocketTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Get_A_Rocket()
    {
        var client = new RocketClient(Factory.CreateClient());
        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket.Id;
                                               }
                                           );

        var response = await client.GetRocketAsync(rocket.Value);

        response.Result.Type.Should().Be(HttpRocketType.Falcon9);
        response.Result.Sn.Should().Be("12345678901234");
    }

    [Fact]
    public async Task Should_Not_Get_A_Missing_Rocket()
    {
        var client = new RocketClient(Factory.CreateClient());

        Func<Task> action = () => client.GetRocketAsync(Guid.NewGuid());
        await action.Should().ThrowAsync<ApiException<ProblemDetails>>()
                    .Where(
                         z => z.StatusCode == 404 && z.Result.Status == 404 && z.Result.Title == "Not Found" && z.Result.Type == "https://httpstatuses.com/404"
                     );
    }

    public GetRocketTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new();
}
