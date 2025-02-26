using Grpc.Core;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;
using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords;

public class ListLaunchRecordsTests(ITestContextAccessor outputHelper, TestWebAppFixture webAppFixture)
    : WebAppFixtureTest<TestWebAppFixture>(outputHelper, webAppFixture)
{
    private static readonly Faker Faker = new();

    [Fact]
    public async Task Should_List_LaunchRecords()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
        await ServiceProvider
             .WithScoped<RocketDbContext>()
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

        var response = await client
                            .ListLaunchRecords(new ListLaunchRecordsRequest(), cancellationToken: TestContext.CancellationToken)
                            .ResponseStream.ReadAllAsync(TestContext.CancellationToken)
                            .ToListAsync(TestContext.CancellationToken);

        response.Count.ShouldBe(10);
    }

    [Fact]
    public async Task Should_List_Specific_Kinds_Of_LaunchRecords()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
        await ServiceProvider
             .WithScoped<RocketDbContext>()
             .Invoke(
                  async (z, ct) =>
                  {
                      var faker = new RocketFaker();
                      var rockets = faker.UseSeed(100).Generate(3);
                      var records = new LaunchRecordFaker(rockets).UseSeed(100).Generate(10);
                      z.AddRange(rockets);
                      z.AddRange(records);
                      await z.SaveChangesAsync(ct);
                  },
                  TestContext.CancellationToken
              );

        var response = await client
                            .ListLaunchRecords(
                                 new()
                                 {
                                     RocketType = new()
                                     {
                                         Data = RocketType.FalconHeavy
                                     }
                                 },
                                 cancellationToken: TestContext.CancellationToken
                             )
                            .ResponseStream.ReadAllAsync(TestContext.CancellationToken)
                            .ToListAsync(TestContext.CancellationToken);

        response.Count.ShouldBe(3);
    }
}
