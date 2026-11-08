namespace Api.Generators.Tests;

public class MinimalApiGeneratorTests() : GeneratorTest()
{
    [Test]
    [Arguments(Mediator.MediatR)]
    [Arguments(Mediator.Mediator)]
    [Arguments(Mediator.ChannelMediator)]
    [Arguments(Mediator.ImmediateHandlers)]
    [Arguments(Mediator.Foundatio)]
    [Arguments(Mediator.ZeroAlloc)]
    public async Task Should_Generate_Rest_Endpoints(Mediator mediator)
    {
        var contracts = await CompileContracts(mediator, Contracts.For(mediator));
        var result = await Server(mediator, contracts).AddSources(Contracts.ServerHost).Build().GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(mediator);
    }

    [Test]
    public async Task Should_Apply_Operation_And_Host_Settings()
    {
        var contracts = await CompileContracts(
            Mediator.MediatR,
            """
            #nullable enable
            using MediatR;
            using Rocket.Surgery.LaunchPad.Api;

            namespace Sample.Contracts
            {
                public enum RocketType { Falcon, Starship }
                public sealed record RocketModel(System.Guid Id, string Name);

                public static class SearchRockets
                {
                    [ApiOperation(Path = "search", Tags = new[] { "search" }, Description = "Find rockets")]
                    public sealed record Request : IRequest<RocketModel[]>
                    {
                        [ApiQuery(Name = "q")] public string? Text { get; init; }
                        public RocketType[]? Types { get; init; }
                        [ApiHeader(Name = "X-Tenant")] public string? Tenant { get; init; }
                        [ApiIgnore] public object? Internal { get; init; }
                    }
                }

                public static class LaunchRocket
                {
                    [ApiOperation(Verb = "POST", SuccessStatusCode = 202)]
                    public sealed record Request(System.Guid Id) : IRequest<RocketModel>;
                }
            }
            """
        );
        var result = await Server(Mediator.MediatR, contracts)
                          .AddSources(
                               """
                               using Rocket.Surgery.LaunchPad.Api;
                               using Sample.Contracts;

                               [OperationHost(Group = "fleet", RoutePrefix = "api/v2")]
                               [Operation<SearchRockets.Request>(Policy = new[] { "rockets.read" }, Summary = "Search")]
                               [Operation<LaunchRocket.Request>(Route = "{id}/launch", Roles = new[] { "pilot" }, AuthenticationSchemes = new[] { "Bearer" })]
                               public static partial class RocketsEndpoints
                               {
                                   static partial void ConfigureLaunchRocket(Microsoft.AspNetCore.Builder.IEndpointConventionBuilder endpoint) { }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Bind_A_Transport_Input()
    {
        var contracts = await CompileContracts(
            Mediator.MediatR,
            """
            #nullable enable
            using MediatR;

            namespace Sample.Contracts
            {
                public sealed record RocketModel(System.Guid Id, string Name);
                public static class PatchRocket { public sealed record Request(System.Guid Id, string? Name) : IRequest<RocketModel>; }
                public sealed record PatchRocketInput(System.Guid Id, string? Name)
                {
                    public PatchRocket.Request Create() => new(Id, Name);
                }
            }
            """
        );
        var result = await Server(Mediator.MediatR, contracts)
                          .AddSources(
                               """
                               using Rocket.Surgery.LaunchPad.Api;
                               using Sample.Contracts;

                               [Operation<PatchRocket.Request>(Input = typeof(PatchRocketInput))]
                               public static partial class RocketsEndpoints;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    // A host whose name, base type and attributes name no transport (LPAD1017).
    [Test]
    public async Task Should_Report_A_Host_Without_A_Transport()
    {
        var contracts = await CompileContracts(Mediator.MediatR, Contracts.For(Mediator.MediatR));
        var result = await Server(Mediator.MediatR, contracts)
                          .AddSources(
                               """
                               using Rocket.Surgery.LaunchPad.Api;
                               using Sample.Contracts.Rockets;

                               [Operation<GetRocket.Request>]
                               public static partial class Rockets;
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Read_Summaries_From_Xml_Documentation()
    {
        var contracts = await CompileContracts(
            Mediator.MediatR,
            """
            #nullable enable
            using MediatR;

            namespace Sample.Contracts
            {
                public sealed record RocketModel(System.Guid Id, string Name);

                /// <summary>Rocket lookups.</summary>
                /// <remarks>Answers 404 when the <paramref name="id" /> is unknown; see <see cref="T:Sample.Contracts.RocketModel" />.</remarks>
                public static class GetRocket
                {
                    /// <summary>
                    ///     Load a <see cref="RocketModel" />
                    ///     by its id.
                    /// </summary>
                    public sealed record Request(System.Guid Id) : IRequest<RocketModel?>;
                }

                /// <summary>Not used: the request's own summary wins.</summary>
                public static class ListRockets
                {
                    /// <summary>Search for rockets.</summary>
                    public sealed record Request(string? Name) : IStreamRequest<RocketModel>;
                }
            }
            """
        );
        // What the compiler writes for the contracts project; the base package's targets hand it over as an additional file.
        const string documentation = """
            <?xml version="1.0"?>
            <doc>
                <assembly><name>Contracts</name></assembly>
                <members>
                    <member name="T:Sample.Contracts.GetRocket">
                        <summary>Rocket lookups.</summary>
                        <remarks>Answers 404 when the <paramref name="id" /> is unknown; see <see cref="T:Sample.Contracts.RocketModel" />.</remarks>
                    </member>
                    <member name="T:Sample.Contracts.GetRocket.Request">
                        <summary>
                            Load a <see cref="T:Sample.Contracts.RocketModel" />
                            by its id.
                        </summary>
                    </member>
                    <member name="T:Sample.Contracts.ListRockets.Request"><summary>Search for rockets.</summary></member>
                </members>
            </doc>
            """;
        var result = await Server(Mediator.MediatR, contracts)
                          .AddAdditionalText("Contracts.xml", documentation)
                          .AddOption("Contracts.xml", "build_metadata.AdditionalFiles.LaunchPadApiDocumentation", "true")
                          .AddSources("using Rocket.Surgery.LaunchPad.Api;\nusing Sample.Contracts;\n[Operation<GetRocket.Request>]\n[Operation<ListRockets.Request>(Summary = \"Explicit wins\")]\npublic static partial class RocketsEndpoints;")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }
}
