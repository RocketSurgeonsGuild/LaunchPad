namespace Api.Generators.Tests;

public class GrpcGeneratorTests() : GeneratorTest()
{
    [Test]
    [Arguments(Mediator.MediatR)]
    [Arguments(Mediator.Mediator)]
    [Arguments(Mediator.ChannelMediator)]
    [Arguments(Mediator.ImmediateHandlers)]
    [Arguments(Mediator.Foundatio)]
    [Arguments(Mediator.ZeroAlloc)]
    public async Task Should_Generate_A_Service(Mediator mediator)
    {
        var contracts = await CompileContracts(mediator, Contracts.For(mediator));
        var result = await GrpcServer(mediator, contracts).AddSources(Contracts.GrpcServiceHost).Build().GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(mediator);
    }

    [Test]
    [Arguments(Mediator.MediatR)]
    [Arguments(Mediator.Mediator)]
    [Arguments(Mediator.ChannelMediator)]
    [Arguments(Mediator.ImmediateHandlers)]
    [Arguments(Mediator.Foundatio)]
    [Arguments(Mediator.ZeroAlloc)]
    public async Task Should_Generate_A_Client(Mediator mediator)
    {
        var contracts = await CompileContracts(mediator, Contracts.For(mediator));
        var result = await GrpcClient(mediator, contracts).AddSources(Contracts.GrpcClientHost).Build().GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(mediator);
    }

}
