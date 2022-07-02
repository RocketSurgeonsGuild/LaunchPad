using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;

namespace Sample.Core.Tests.Rockets;

public class ListRocketsTests : HandleTestHostBase
{
    [Fact]
    public async Task Should_List_Rockets()
    {
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      var faker = new RocketFaker();
                                      z.AddRange(faker.Generate(10));

                                      await z.SaveChangesAsync();
                                  }
                              );

        var response = await ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.CreateStream(new ListRockets.Request(null)).ToListAsync()
        );

        response.Should().HaveCount(10);
    }

    public ListRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace)
    {
    }

    private static readonly Faker Faker = new();
}
