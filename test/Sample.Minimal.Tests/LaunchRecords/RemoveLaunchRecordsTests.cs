using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Minimal.Tests.Helpers;

namespace Sample.Minimal.Tests.LaunchRecords;

public class RemoveLaunchRecordsTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Remove_LaunchRecord()
    {
        var client = Client;
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

        await client.DeleteLaunchRecord(record.Id.Value);

        await using var assertScope = ServiceProvider.CreateAsyncScope();
        await assertScope.ServiceProvider.GetRequiredService<RocketDbContext>().LaunchRecords.Should().BeEmpty();
    }

    [Test]
    public async Task Should_Not_Be_Authorized_On_Given_Record()
    {
        var id = LaunchRecordId.Create(new Guid("bad361de-a6d5-425a-9cf6-f9b2dd236be6"));
        var client = Client;
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

        var (status, problem) = await Failure<ProblemDetails>(() => client.DeleteLaunchRecord(id.Value));
        await status.Should().BeEqualTo(System.Net.HttpStatusCode.Forbidden);
        await problem!.Status.Should().BeEqualTo(403);
        await problem.Title.Should().BeEqualTo("Forbidden");
    }

    private static readonly Faker Faker = new();
}
