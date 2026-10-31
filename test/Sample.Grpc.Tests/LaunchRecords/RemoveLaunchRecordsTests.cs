using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;

using LR = Sample.Grpc.LaunchRecords;

namespace Sample.Grpc.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests : WebAppFixtureTest
{
    private static readonly Faker Faker = new();

    [Test]
    public async Task Should_Remove_LaunchRecord()
    {
        var client = new LR.LaunchRecordsClient(AlbaHost.CreateGrpcChannel());
        LaunchRecord record;
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rocket = faker.Generate();
            record = new LaunchRecordFaker(new[] { rocket }.ToList()).Generate();
            z.Add(rocket);
            z.Add(record);

            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        await client.DeleteLaunchRecordAsync(new DeleteLaunchRecordRequest { Id = record.Id.ToString() }, cancellationToken: TestContext.CancellationToken);

        await using var assertScope = ServiceProvider.CreateAsyncScope();
        assertScope.ServiceProvider.GetRequiredService<RocketDbContext>().LaunchRecords.ShouldBeEmpty();
    }
}
