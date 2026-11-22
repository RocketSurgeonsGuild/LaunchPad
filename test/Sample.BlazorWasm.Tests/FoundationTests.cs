using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;

using Sample.BlazorWasm.Client;
using Sample.BlazorWasm.Forms;

using ZeroAlloc.Validation;

// as in Sdk.Tests: the public test classes expose nothing for other languages to consume. Qualified, because Codacy compiles the file on
// its own, without the implicit usings.
#pragma warning disable IDE0001
[assembly: System.CLSCompliant(false)]
#pragma warning restore IDE0001

namespace Sample.BlazorWasm.Tests;

public class FoundationTests : HandleTestHostBase
{
    [Test]
    public async Task Should_Register_The_Validators_Of_The_Sample()
    {
        var result = await ServiceProvider
                          .GetRequiredService<ValidatorFor<RocketRequest>>()
                          .ValidateAsync(new RocketRequest { SerialNumber = "short" }, TestContext.CancellationToken);

        await result.Failures.ToArray().Select(z => z.PropertyName).Distinct().Should().BeEquivalentTo([nameof(RocketRequest.SerialNumber)], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Register_The_Validator_Of_The_View_Model()
    {
        var result = await ServiceProvider
                          .GetRequiredService<ValidatorFor<RocketForm>>()
                          .ValidateAsync(new RocketForm { SerialNumber = "", Type = RocketType.AtlasV }, TestContext.CancellationToken);

        await result.Failures.ToArray().Select(z => z.PropertyName).Distinct().Should().BeEquivalentTo([nameof(RocketForm.SerialNumber)], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Accept_A_Valid_Rocket()
    {
        var result = await ServiceProvider
                          .GetRequiredService<ValidatorFor<RocketForm>>()
                          .ValidateAsync(new RocketForm { SerialNumber = "1234567890", Type = RocketType.FalconHeavy }, TestContext.CancellationToken);

        await result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task Should_Read_The_Rockets_Of_Sample_Minimal_Without_Reflection()
    {
        var rocket = JsonSerializer.Deserialize(
            """{"id":"0058f30b-982a-9428-aa4e-aa704a6eb989","sn":"1234567890","type":"falconHeavy"}""",
            RocketsClientJson.Default.RocketModel
        );

        await rocket!.Sn.Should().BeEqualTo("1234567890");
        await rocket.Type.Should().BeEqualTo(RocketType.FalconHeavy);
    }

    [Test]
    public async Task Should_Read_The_Problem_Details_Of_Sample_Minimal()
    {
        var problem = JsonSerializer.Deserialize(
            """{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"Not Found","status":404,"detail":"Rocket 1 was not found"}""",
            RocketsClientJson.Default.ProblemDetails
        );

        await problem!.Message().Should().BeEqualTo("Rocket 1 was not found");
    }
}
