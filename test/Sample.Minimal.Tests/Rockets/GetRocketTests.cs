﻿using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Restful.Client;
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

        response.Result.Type.ShouldBe(HttpRocketType.Falcon9);
        response.Result.Sn.ShouldBe("12345678901234");
    }

    [Fact]
    public async Task Should_Not_Get_A_Missing_Rocket()
    {
        var client = new RocketClient(Factory.CreateClient());

        Func<Task> action = () => client.GetRocketAsync(Guid.NewGuid());
        await action.ShouldThrowAsync<ApiException<ProblemDetails>>()
                    .Where(
                         z => z.StatusCode == 404 && z.Result.Status == 404 && z.Result.Title == "Not Found"
                     );
    }

    public GetRocketTests(ITestOutputHelper outputHelper, TestWebHost host) : base(outputHelper, host)
    {
    }

    private static readonly Faker Faker = new();
}
