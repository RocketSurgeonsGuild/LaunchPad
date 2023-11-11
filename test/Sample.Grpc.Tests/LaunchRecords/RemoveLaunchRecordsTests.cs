using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;
using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests(ITestOutputHelper outputHelper, TestWebAppFixture webAppFixture)
    : WebAppFixtureTest<TestWebAppFixture>(outputHelper, webAppFixture)
{
    private static readonly Faker Faker = new();

    [Fact]
    public async Task Should_Remove_LaunchRecord()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
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

        await client.DeleteLaunchRecordAsync(new DeleteLaunchRecordRequest { Id = id.ToString() });

        ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.LaunchRecords.Should().BeEmpty());
    }
}
