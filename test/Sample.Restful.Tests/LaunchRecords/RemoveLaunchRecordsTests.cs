using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Restful.Client;

namespace Sample.Restful.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Remove_LaunchRecord()
    {
        var client = new LaunchRecordClient(Factory.CreateClient());
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

        await client.DeleteLaunchRecordAsync(id.Value);

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.LaunchRecords.Should().BeEmpty());
    }

    [Fact]
    public async Task Should_Not_Be_Authorized_On_Given_Record()
    {
        var id = new LaunchRecordId(new Guid("bad361de-a6d5-425a-9cf6-f9b2dd236be6"));
        var client = new LaunchRecordClient(Factory.CreateClient());
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


        Func<Task> action = () => client.DeleteLaunchRecordAsync(id.Value);
        await action.Should().ThrowAsync<ApiException<ProblemDetails>>()
                    .Where(z => z.StatusCode == 403 && z.Result.Status == 403 && z.Result.Title == "Forbidden");
    }

    public RemoveLaunchRecordsTests(ITestOutputHelper outputHelper, TestWebHost host) : base(outputHelper, host)
    {
    }

    private static readonly Faker Faker = new();
}
