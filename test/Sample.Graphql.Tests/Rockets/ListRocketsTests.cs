using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests.Rockets;

public class ListRocketsTests : GraphQlWebAppFixtureTest
{
    [Test]
    public async Task Should_List_Rockets()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            z.AddRange(faker.Generate(10));

            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client.GetRockets.ExecuteAsync(cancellationToken: TestContext.CancellationToken);
        response.EnsureNoErrors();

        response.Data!.Rockets!.Nodes!.Count.ShouldBe(10);
    }

    [Test]
    public async Task Should_List_Specific_Kinds_Of_Rockets()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            z.AddRange(faker.UseSeed(100).Generate(10));

            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client.GetFilteredRockets.ExecuteAsync(RocketType.AtlasV, TestContext.CancellationToken);
        response.EnsureNoErrors();

        response.Data!.Rockets!.Nodes!.Count.ShouldBe(5);
    }

    private static readonly Faker Faker = new();
}
