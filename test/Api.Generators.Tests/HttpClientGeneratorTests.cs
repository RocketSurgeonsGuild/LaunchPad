using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Api.Generators.Tests;

public class HttpClientGeneratorTests() : GeneratorTest()
{
    [Test]
    [Arguments(Mediator.MediatR)]
    [Arguments(Mediator.Mediator)]
    [Arguments(Mediator.ChannelMediator)]
    [Arguments(Mediator.ImmediateHandlers)]
    [Arguments(Mediator.Foundatio)]
    [Arguments(Mediator.ZeroAlloc)]
    public async Task Should_Generate_Client_And_Handlers(Mediator mediator)
    {
        var contracts = await CompileContracts(mediator, Contracts.For(mediator));
        var result = await Client(mediator, contracts).AddSources(Contracts.ClientHost).Build().GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(mediator);
    }

    [Test]
    public async Task Should_Skip_Handlers_When_Asked()
    {
        var contracts = await CompileContracts(Mediator.MediatR, Contracts.For(Mediator.MediatR));
        var result = await Client(Mediator.MediatR, contracts)
                          .AddSources(
                               """
                               using Rocket.Surgery.LaunchPad.Api;
                               using Sample.Contracts.Rockets;

                               [OperationHost(ClientName = "Fleet", Handlers = false, Version = "2.0", JsonOptions = JsonOptionsPreset.Default)]
                               [Operation<GetRocket.Request>]
                               [Operation<EditRocket.Request>(Handlers = true)]
                               public partial class FleetClient;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }


    [Test]
    [Arguments(Mediator.MediatR, "|Mediator.SourceGenerator|")]
    [Arguments(Mediator.Mediator, "|")]
    [Arguments(Mediator.Foundatio, "|ZeroAlloc.Mediator.Generator|")]
    [Arguments(Mediator.ZeroAlloc, "|ZeroAlloc.Mediator.Generator|")]
    public async Task Should_Accept_A_Client_Host_In_The_Right_Project(Mediator mediator, string mediatorGenerators)
    {
        var result = await ClientWithGenerators(mediator, mediatorGenerators);

        await Verify(result).UseParameters(mediator);
    }


    private async Task<GeneratorTestResults> ClientWithGenerators(Mediator mediator, string mediatorGenerators)
    {
        var contracts = await CompileContracts(mediator, Contracts.For(mediator));
        return await Client(mediator, contracts)
                    .AddGlobalOption("build_property.LaunchPadApiMediatorGenerators", mediatorGenerators)
                    .AddSources(Contracts.ClientHost)
                    .Build()
                    .GenerateAsync(TestContext.CancellationToken);
    }


}
