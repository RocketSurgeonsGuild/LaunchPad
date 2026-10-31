using Grpc.Core;

using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;

using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords;

public class ListLaunchRecordsTests : WebAppFixtureTest
{
    private static readonly Faker Faker = new();

    [Test]
    public async Task Should_List_LaunchRecords()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rockets = faker.Generate(3);
            var records = new LaunchRecordFaker(rockets).Generate(10);
            z.AddRange(rockets);
            z.AddRange(records);
            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client
                            .ListLaunchRecords(new ListLaunchRecordsRequest(), cancellationToken: TestContext.CancellationToken)
                            .ResponseStream.ReadAllAsync(TestContext.CancellationToken)
                            .ToListAsync(TestContext.CancellationToken);

        response.Count.ShouldBe(10);
    }

    [Test]
    public async Task Should_List_Specific_Kinds_Of_LaunchRecords()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rockets = faker.UseSeed(100).Generate(3);
            var records = new LaunchRecordFaker(rockets).UseSeed(100).Generate(10);
            z.AddRange(rockets);
            z.AddRange(records);
            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

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
