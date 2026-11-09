namespace Api.Generators.Tests;

public class GraphQLGeneratorTests() : GeneratorTest()
{
    [Test]
    [Arguments(Mediator.MediatR)]
    [Arguments(Mediator.Mediator)]
    [Arguments(Mediator.ChannelMediator)]
    [Arguments(Mediator.ImmediateHandlers)]
    [Arguments(Mediator.Foundatio)]
    [Arguments(Mediator.ZeroAlloc)]
    public async Task Should_Generate_Mutations(Mediator mediator)
    {
        var contracts = await CompileContracts(mediator, Contracts.For(mediator));
        var result = await GraphQLServer(mediator, contracts).AddSources(Contracts.MutationHost).Build().GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(mediator);
    }


    // Queries aren't generated: they usually return IQueryable for paging and filtering (LPAD1017).
    [Test]
    public async Task Should_Report_A_Queries_Host()
    {
        var contracts = await CompileContracts(Mediator.MediatR, Contracts.For(Mediator.MediatR));
        var result = await GraphQLServer(Mediator.MediatR, contracts)
                          .AddSources(
                               """
                               using Rocket.Surgery.LaunchPad.Api;
                               using Sample.Contracts.Rockets;

                               [Operation<GetRocket.Request>] public partial class RocketsQueries;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }
}
