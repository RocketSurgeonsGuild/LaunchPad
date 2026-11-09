namespace Api.Generators.Tests;

/// <summary>A server with hosts of several transports runs several generators; their generated helpers must not collide.</summary>
public class CombinedGeneratorTests() : GeneratorTest()
{
    [Test]
    public async Task Should_Compile_Minimal_Api_And_Controller_Hosts_In_One_Project()
    {
        var contracts = await CompileContracts(Mediator.MediatR, Contracts.For(Mediator.MediatR));
        var result = await Controllers(Mediator.MediatR, contracts)
                          .WithGenerator<Rocket.Surgery.LaunchPad.Api.Generators.MinimalApiGenerator>()
                          .AddSources(Contracts.ServerHost, Contracts.ControllerHost)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }
}
