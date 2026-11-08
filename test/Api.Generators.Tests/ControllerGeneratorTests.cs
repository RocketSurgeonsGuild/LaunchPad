namespace Api.Generators.Tests;

public class ControllerGeneratorTests() : GeneratorTest()
{
    [Test]
    [Arguments(Mediator.MediatR)]
    [Arguments(Mediator.Mediator)]
    [Arguments(Mediator.ChannelMediator)]
    [Arguments(Mediator.ImmediateHandlers)]
    [Arguments(Mediator.Foundatio)]
    [Arguments(Mediator.ZeroAlloc)]
    public async Task Should_Generate_Actions(Mediator mediator)
    {
        var contracts = await CompileContracts(mediator, Contracts.For(mediator));
        var result = await Controllers(mediator, contracts).AddSources(Contracts.ControllerHost).Build().GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(mediator);
    }

    [Test]
    public async Task Should_Keep_What_The_Host_Declares()
    {
        var contracts = await CompileContracts(Mediator.MediatR, Contracts.For(Mediator.MediatR));
        var result = await Controllers(Mediator.MediatR, contracts)
                          .AddSources(
                               """
                               using Microsoft.AspNetCore.Mvc;
                               using Rocket.Surgery.LaunchPad.Api;
                               using Sample.Contracts.Rockets;

                               [ApiController]
                               [Route("v1/[controller]")]
                               [Operation<GetRocket.Request>(Policy = new[] { "rockets.read" }, Tags = new[] { "search" })]
                               [Operation<CreateRocket.Request>(Roles = new[] { "pilot" }, AuthenticationSchemes = new[] { "Bearer" })]
                               [Operation<DeleteRocket.Request>(AllowAnonymous = true, Summary = "Remove a rocket", Description = "Gone for good.")]
                               public partial class FleetController : ControllerBase
                               {
                                   public string Hello() => "hand-written actions stay";
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

}
