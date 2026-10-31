using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Restful.Client;
using Sample.Minimal.Tests.Helpers;

namespace Sample.Minimal.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Remove_LaunchRecord()
    {
        var client = new LaunchRecordClient(AlbaHost.Server.CreateClient());
        var faker = new RocketFaker();
        var rocket = faker.Generate();
        var record = new LaunchRecordFaker(new[] { rocket }.ToList()).Generate();
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            z.Add(rocket);
            z.Add(record);

            await z.SaveChangesAsync();
        }

        await client.DeleteLaunchRecordAsync(record.Id.Value);

        await using var assertScope = ServiceProvider.CreateAsyncScope();
        assertScope.ServiceProvider.GetRequiredService<RocketDbContext>().LaunchRecords.ShouldBeEmpty();
    }

    [Test]
    public async Task Should_Not_Be_Authorized_On_Given_Record()
    {
        var id = new LaunchRecordId(new Guid("bad361de-a6d5-425a-9cf6-f9b2dd236be6"));
        var client = new LaunchRecordClient(AlbaHost.Server.CreateClient());
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            var rocket = faker.Generate();
            var record = new LaunchRecordFaker(new[] { rocket }.ToList()).Generate();
            z.Add(rocket);
            record.Id = id;
            z.Add(record);

            await z.SaveChangesAsync();
        }

        Func<Task> action = () => client.DeleteLaunchRecordAsync(id.Value);
        await action.ShouldThrowAsync<ApiException<ProblemDetails>>()
                    .Where(z => z.StatusCode == 403 && z.Result.Status == 403 && z.Result.Title == "Forbidden");
    }

    private static readonly Faker Faker = new();
}
