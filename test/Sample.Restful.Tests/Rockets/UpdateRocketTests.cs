using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;
using Sample.Restful.Client;
using Sample.Restful.Tests.Helpers;

using RocketType = Sample.Core.Domain.RocketType;
using ClientRocketType = Sample.Restful.Client.RocketType;

namespace Sample.Restful.Tests.Rockets;

public class UpdateRocketTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Update_A_Rocket()
    {
        var client = new RocketClient(AlbaHost.Server.CreateClient());

        var rocket = await SeedRocketAsync(RocketType.Falcon9);

        var u = await client.EditRocketAsync(
            rocket.Id.Value,
            new EditRocketRequest
            {
                Type = ClientRocketType.FalconHeavy,
                SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
            }
        );
        (u.StatusCode is >= 200 and < 300).ShouldBeTrue();

        var response = await client.GetRocketAsync(rocket.Id.Value);

        response.Result.Type.ShouldBe(ClientRocketType.FalconHeavy);
        response.Result.Sn.ShouldBe("43210987654321");
    }

    [Test]
    public async Task Should_Patch_A_Rocket_SerialNumber()
    {
        var client = new RocketClient(AlbaHost.Server.CreateClient());

        var rocket = await SeedRocketAsync(RocketType.AtlasV);

        var u = await client.PatchRocketAsync(
            rocket.Id.Value,
            new EditRocketPatchRequest
            {
                SerialNumber = new() { Value = "123456789012345" }
            }
        );
        (u.StatusCode is >= 200 and < 300).ShouldBeTrue();

        var response = await client.GetRocketAsync(rocket.Id.Value);

        response.Result.Type.ShouldBe(ClientRocketType.AtlasV);
        response.Result.Sn.ShouldBe("123456789012345");
    }

    [Test]
    public async Task Should_Fail_To_Patch_A_Null_Rocket_SerialNumber()
    {
        var client = new RocketClient(AlbaHost.Server.CreateClient());

        var rocket = await SeedRocketAsync(RocketType.AtlasV);

        Func<Task<Response<RocketModel>>> action = () => client.PatchRocketAsync(
            rocket.Id.Value,
            new EditRocketPatchRequest
            {
                SerialNumber = new() { Value = null }
            }
        );
        var result = (await action.ShouldThrowAsync<ApiException<FluentValidationProblemDetails>>()).And.Result;
        result.Errors.ShouldHaveCount(1);
        result.Errors["SerialNumber"][0].ErrorMessage.ShouldBe("'Serial Number' must not be empty.");
    }

    [Test]
    public async Task Should_Patch_A_Rocket_Type()
    {
        var client = new RocketClient(AlbaHost.Server.CreateClient());

        var rocket = await SeedRocketAsync(RocketType.AtlasV);

        var u = await client.PatchRocketAsync(
            rocket.Id.Value,
            new EditRocketPatchRequest
            {
                Type = new() { Value = ClientRocketType.FalconHeavy }
            }
        );
        (u.StatusCode is >= 200 and < 300).ShouldBeTrue();

        var response = await client.GetRocketAsync(rocket.Id.Value);
        response.Result.Type.ShouldBe(ClientRocketType.FalconHeavy);
        response.Result.Sn.ShouldBe("12345678901234");
    }

    [Test]
    [MethodDataSource(nameof(RequiredFieldData))]
    public async Task Should_Validate_Required_Fields(EditRocketRequest request, string propertyName)
    {
        var client = new RocketClient(AlbaHost.Server.CreateClient());
        Func<Task> a = () => client.EditRocketAsync(Guid.NewGuid(), request);
        (await a.ShouldThrowAsync<ApiException<FluentValidationProblemDetails>>())
           .And
           .Result.Errors.Values
           .SelectMany(x => x)
           .Select(z => z.PropertyName)
           .ShouldContain(propertyName);
    }

    public static IEnumerable<Func<(EditRocketRequest Request, string PropertyName)>> RequiredFieldData()
    {
        yield return () => (
            new EditRocketRequest(),
            nameof(EditRocketRequest.SerialNumber)
        );
        yield return () => (
            new EditRocketRequest { SerialNumber = Faker.Random.String2(0, 9) },
            nameof(EditRocketRequest.SerialNumber)
        );
        yield return () => (
            new EditRocketRequest { SerialNumber = Faker.Random.String2(600, 800) },
            nameof(EditRocketRequest.SerialNumber)
        );
    }

    private async Task<ReadyRocket> SeedRocketAsync(RocketType type)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var z = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
        var rocket = new ReadyRocket
        {
            Type = type,
            SerialNumber = "12345678901234"
        };
        z.Add(rocket);

        await z.SaveChangesAsync();
        return rocket;
    }

    private static readonly Faker Faker = new();
}
