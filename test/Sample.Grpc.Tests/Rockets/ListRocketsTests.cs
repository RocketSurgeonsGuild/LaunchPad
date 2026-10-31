using Grpc.Core;

using Microsoft.Extensions.DependencyInjection;

using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;

using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class ListRocketsTests : WebAppFixtureTest
{
    private static readonly Faker Faker = new();

    [Test]
    public async Task Should_List_Rockets()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            z.AddRange(faker.Generate(10));

            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client
                            .ListRockets(new ListRocketsRequest(), cancellationToken: TestContext.CancellationToken)
                            .ResponseStream.ReadAllAsync(TestContext.CancellationToken)
                            .ToListAsync(TestContext.CancellationToken);

        response.Count.ShouldBe(10);
    }

    [Test]
    public async Task Should_List_Specific_Kinds_Of_Rockets()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());
        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
            var faker = new RocketFaker();
            z.AddRange(faker.UseSeed(100).Generate(10));

            await z.SaveChangesAsync(TestContext.CancellationToken);
        }

        var response = await client
                            .ListRockets(
                                 new ListRocketsRequest
                                 {
                                     RocketType = new NullableRocketType
                                     {
                                         Data = RocketType.AtlasV
                                     }
                                 },
                                 cancellationToken: TestContext.CancellationToken
                             )
                            .ResponseStream.ReadAllAsync(TestContext.CancellationToken)
                            .ToListAsync(TestContext.CancellationToken);

        response.Count.ShouldBe(5);
    }
}
