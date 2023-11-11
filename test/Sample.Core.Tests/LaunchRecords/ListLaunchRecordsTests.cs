using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.LaunchRecords;

namespace Sample.Core.Tests.LaunchRecords;

public class ListLaunchRecordsTests(ITestOutputHelper outputHelper) : HandleTestHostBase(outputHelper, LogLevel.Trace)
{
    [Fact]
    public async Task Should_List_LaunchRecords()
    {
        await ServiceProvider.WithScoped<RocketDbContext>()
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

        var response = await ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.CreateStream(new ListLaunchRecords.Request(null)).ToListAsync()
        );

        response.Should().HaveCount(10);
    }

    private static readonly Faker Faker = new();
}
