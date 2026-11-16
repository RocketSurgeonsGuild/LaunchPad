using Microsoft.Extensions.DependencyInjection;
using Sample.Core.Domain;
using Sample.Restful.Tests.Helpers;
using ClientRocketType = Sample.Restful.Tests.Client.RocketType;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Restful.Tests.Rockets;

public class UpdateRocketTests : WebAppFixtureTest
{
    [Test]
    public async Task Should_Update_A_Rocket()
    {
        var client = Client;

        var rocket = await SeedRocketAsync(RocketType.Falcon9);

        var u = await client.EditRocket(
            rocket.Id.Value,
            new EditRocketRequest
            {
                Type = ClientRocketType.FalconHeavy,
                SerialNumber = new string([.. rocket.SerialNumber.Reverse()])
            }
        );
        await u.Should().NotBeNull();

        var response = await client.GetRocket(rocket.Id.Value);

        await response.Type.Should().BeEqualTo(ClientRocketType.FalconHeavy);
        await response.Sn.Should().BeEqualTo("43210987654321");
    }

    [Test]
    public async Task Should_Patch_A_Rocket_SerialNumber()
    {
        var client = Client;

        var rocket = await SeedRocketAsync(RocketType.AtlasV);

        var u = await client.PatchRocket(
            rocket.Id.Value,
            new EditRocketPatchRequest
            {
                SerialNumber = "123456789012345",
            }
        );
        await u.Should().NotBeNull();

        var response = await client.GetRocket(rocket.Id.Value);

        await response.Type.Should().BeEqualTo(ClientRocketType.AtlasV);
        await response.Sn.Should().BeEqualTo("123456789012345");
    }

    [Test]
    public async Task Should_Fail_To_Patch_A_Null_Rocket_SerialNumber()
    {
        var client = Client;

        var rocket = await SeedRocketAsync(RocketType.AtlasV);

        // an explicit null assigns null, which the rocket's serial number can't be: the patched request fails validation
        var (status, problem) = await PatchJson($"/Rocket/{rocket.Id.Value}", """{"serialNumber":null}""");
        await status.Should().BeEqualTo(System.Net.HttpStatusCode.UnprocessableEntity);
        await Errors(problem).Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task Should_Leave_The_Rocket_Type_Alone_When_Patched_With_Null()
    {
        var rocket = await SeedRocketAsync(RocketType.AtlasV);

        // a rocket's type has no null value: an explicit null leaves it unassigned, as if it weren't sent
        var (status, _) = await PatchJson($"/Rocket/{rocket.Id.Value}", """{"type":null,"serialNumber":"123456789012345"}""");
        await status.Should().BeEqualTo(System.Net.HttpStatusCode.OK);
        var response = await Client.GetRocket(rocket.Id.Value);
        await response.Type.Should().BeEqualTo(ClientRocketType.AtlasV);
        await response.Sn.Should().BeEqualTo("123456789012345");
    }

    [Test]
    public async Task Should_Patch_A_Rocket_Type()
    {
        var client = Client;

        var rocket = await SeedRocketAsync(RocketType.AtlasV);

        var u = await client.PatchRocket(
            rocket.Id.Value,
            new EditRocketPatchRequest
            {
                Type = ClientRocketType.FalconHeavy,
            }
        );
        await u.Should().NotBeNull();

        var response = await client.GetRocket(rocket.Id.Value);
        await response.Type.Should().BeEqualTo(ClientRocketType.FalconHeavy);
        await response.Sn.Should().BeEqualTo("12345678901234");
    }

    [Test]
    [MethodDataSource(nameof(RequiredFieldData))]
    public async Task Should_Validate_Required_Fields(EditRocketRequest request, string propertyName)
    {
        var client = Client;
        var (_, problem) = await Failure<System.Text.Json.Nodes.JsonObject>(() => client.EditRocket(Guid.NewGuid(), request));
        await Errors(problem).Should().Contain(propertyName);
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

    // the properties a validation problem has errors for
    private static IEnumerable<string> Errors(System.Text.Json.Nodes.JsonObject? problem) =>
        ( problem?["errors"] ?? throw new InvalidOperationException($"No errors in {problem?.ToJsonString()}") ).AsObject().Select(z => z.Key);

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
    private static readonly string[] expected = ["SerialNumber"];
}
