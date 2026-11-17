using Rocket.Surgery.LaunchPad.Analyzers.Api.MinimalApi;

namespace Api.Generators.Tests;

/// <summary>A server with hosts of several transports runs several generators; their generated helpers must not collide.</summary>
public class CombinedGeneratorTests() : GeneratorTest()
{
    [Test]
    public async Task Should_Compile_Minimal_Api_And_Controller_Hosts_In_One_Project()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Controllers(Mediator.Mediator, contracts)
                          .WithGenerator<MinimalApiGenerator>()
                          .AddSources(Contracts.ServerHost, Contracts.ControllerHost)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }
}
