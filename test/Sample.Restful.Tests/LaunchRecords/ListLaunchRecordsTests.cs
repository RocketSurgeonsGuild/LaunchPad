using System.Threading.Tasks;
using Bogus;
using Sample.Core;
using Sample.Core.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Restful.Tests.LaunchRecords;

public class ListLaunchRecordsTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_List_LaunchRecords()
    {
        var client = new LaunchRecordClient(Factory.CreateClient());
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

        var response = await client.ListLaunchRecordsAsync();

        response.Result.Should().HaveCount(10);
    }

    public ListLaunchRecordsTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static readonly Faker Faker = new Faker();
}
