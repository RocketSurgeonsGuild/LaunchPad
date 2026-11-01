using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.Rockets;

public class UpdateRocketTests : GraphQlWebAppFixtureTest
{
    [Test]
    public async Task Should_Update_A_Rocket()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await SeedRocketAsync(CoreRocketType.Falcon9);

        var u = await client.UpdateRocket.ExecuteAsync(
            new()
            {
                Id = rocket.Id.Value,
                Type = GraphQlRocketType.FalconHeavy,
                SerialNumber = string.Join("", rocket.SerialNumber.Reverse()),
            },
            TestContext.CancellationToken
        );
        await Verify(u);
    }

    [Test]
    public async Task Should_Patch_A_Rocket_SerialNumber()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await SeedRocketAsync(CoreRocketType.AtlasV);

        var u = await client.PatchRocket.ExecuteAsync(
            new()
            {
                Id = rocket.Id.Value,
                SerialNumber = "123456789012345",
            },
            cancellationToken: TestContext.CancellationToken
        );
        u.EnsureNoErrors();

        await Verify(u);
    }

    [Test]
    public async Task Should_Fail_To_Patch_A_Null_Rocket_SerialNumber()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await SeedRocketAsync(CoreRocketType.AtlasV);

        var u = await client.PatchRocket.ExecuteAsync(
            new()
            {
                Id = rocket.Id.Value,
                SerialNumber = null,
            },
            cancellationToken: TestContext.CancellationToken
        );
        await Verify(u);
    }

    [Test]
    public async Task Should_Patch_A_Rocket_Type()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await SeedRocketAsync(CoreRocketType.AtlasV);

        var u = await client.PatchRocket.ExecuteAsync(
            new()
            {
                Id = rocket.Id.Value,
                Type = GraphQlRocketType.FalconHeavy,
            },
            TestContext.CancellationToken
        );

        await Verify(u);
    }

    [Test]
    [MethodDataSource(nameof(RequiredFieldData))]
    public async Task Should_Validate_Required_Fields(EditRocketRequest request, string propertyName)
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var response = await client.UpdateRocket.ExecuteAsync(request, TestContext.CancellationToken);
        response.IsErrorResult().ShouldBeTrue();

        await Verify(response.Errors).UseParameters(request.Id);
    }

    public static IEnumerable<Func<(EditRocketRequest Request, string PropertyName)>> RequiredFieldData()
    {
        // The ids come from one seeded faker, so the rows are built in order up front: the snapshot names contain them.
        var faker = new Faker
        {
            Random = new(1234567),
        };
        var rows = new (EditRocketRequest Request, string PropertyName)[]
        {
            (
                new()
                {
                    Id = faker.Random.Guid(),
                    Type = GraphQlRocketType.Falcon9,
                },
                nameof(EditRocketRequest.SerialNumber)
            ),
            (
                new()
                {
                    Id = faker.Random.Guid(),
                    SerialNumber = faker.Random.String2(0, 9),
                    Type = GraphQlRocketType.FalconHeavy,
                },
                nameof(EditRocketRequest.SerialNumber)
            ),
            (
                new()
                {
                    Id = faker.Random.Guid(),
                    SerialNumber = faker.Random.String2(600, 800),
                    Type = GraphQlRocketType.AtlasV,
                },
                nameof(EditRocketRequest.SerialNumber)
            ),
            (
                new()
                {
                    Id = faker.Random.Guid(),
                    SerialNumber = faker.Random.String2(11),
                },
                nameof(EditRocketRequest.Type)
            ),
        };

        foreach (var row in rows)
        {
            yield return () => row;
        }
    }

    private async Task<ReadyRocket> SeedRocketAsync(CoreRocketType type)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
        var rocket = new ReadyRocket
        {
            Type = type,
            SerialNumber = "12345678901234",
        };
        z.Add(rocket);

        await z.SaveChangesAsync(TestContext.CancellationToken);
        return rocket;
    }
}
