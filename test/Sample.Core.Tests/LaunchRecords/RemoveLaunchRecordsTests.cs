using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;

namespace Sample.Core.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests : HandleTestHostBase
{
    [Fact]
    public async Task Should_Remove_LaunchRecord()
    {
        var id = await ServiceProvider.WithScoped<RocketDbContext>()
                                      .Invoke(
                                           async z =>
                                           {
                                               var faker = new RocketFaker();
                                               var rocket = faker.Generate();
                                               var record = new LaunchRecordFaker(new[] { rocket }.ToList()).Generate();
                                               z.Add(rocket);
                                               z.Add(record);

                                               await z.SaveChangesAsync();
                                               return record.Id;
                                           }
                                       );

        await ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(new DeleteLaunchRecord.Request { Id = id })
        );

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.LaunchRecords.Should().BeEmpty());
    }

    [Fact]
    public async Task Should_Not_Be_Authorized_On_Given_Record()
    {
        var id = new LaunchRecordId(new Guid("bad361de-a6d5-425a-9cf6-f9b2dd236be6"));
        await ServiceProvider.WithScoped<RocketDbContext>()
                             .Invoke(
                                  async z =>
                                  {
                                      var faker = new RocketFaker();
                                      var rocket = faker.Generate();
                                      var record = new LaunchRecordFaker(new[] { rocket }.ToList()).Generate();
                                      z.Add(rocket);
                                      record.Id = id;
                                      z.Add(record);

                                      await z.SaveChangesAsync();
                                      return record.Id;
                                  }
                              );

        Func<Task> action = () => ServiceProvider.WithScoped<IMediator>().Invoke(
            mediator => mediator.Send(new DeleteLaunchRecord.Request { Id = id })
        );
        await action.Should().ThrowAsync<NotAuthorizedException>();
    }

    public RemoveLaunchRecordsTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace)
    {
    }

    private static readonly Faker Faker = new();
}
