namespace Api.Generators.Tests;

/// <summary>
///     The OpenAPI operation transformers the HTTP generators emit when the server references Microsoft.AspNetCore.OpenApi: each union error
///     case's schema, the 201 <c>Location</c> header, the 401 <c>WWW-Authenticate</c> header, and the request's ZeroAlloc.Validation rules, read
///     at compile time. Minimal API endpoints register theirs in <c>Map{Host}()</c>, controller actions through a convention attribute.
/// </summary>
public class OpenApiGeneratorTests() : GeneratorTest()
{
    public static IEnumerable<(HostCase Host, Mediator Mediator)> Cases() =>
        from host in new[] { HostCase.MinimalApi, HostCase.Controller }
        from mediator in new[] { Mediator.Mediator, Mediator.ChannelMediator, Mediator.ImmediateHandlers, Mediator.Foundatio, Mediator.ZeroAlloc }
        select (host, mediator);

    [Test]
    [MethodDataSource(nameof(Cases))]
    public async Task Should_Generate_OpenApi_Transformers(HostCase host, Mediator mediator)
    {
        var contracts = await CompileContracts(
            mediator,
            UnionContracts.For(mediator) + ValidatedContracts(mediator),
            typeof(System.Runtime.Serialization.DataContractAttribute).Assembly,
            typeof(ZeroAlloc.Validation.NotEmptyAttribute).Assembly
        );
        var builder = host == HostCase.Controller ? Controllers(mediator, contracts) : Server(mediator, contracts);
        var result = await builder.AddReferences(
                                       typeof(Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext).Assembly,
                                       typeof(Microsoft.OpenApi.OpenApiOperation).Assembly,
                                       typeof(ZeroAlloc.Validation.NotEmptyAttribute).Assembly
                                   )
                                  .AddSources(Host(host))
                                  .Build()
                                  .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(host, mediator);
    }

    // Descriptions from the contracts' XML documentation, which the base package's targets hand over as an additional file: the request,
    // its body and route members, the response and its members (a positional record's <param> when the property has no summary of its own),
    // and each union error case with its members.
    [Test]
    [Arguments(HostCase.MinimalApi)]
    [Arguments(HostCase.Controller)]
    public async Task Should_Describe_Types_And_Members_From_Xml_Documentation(HostCase host)
    {
        var contracts = await CompileContracts(
            Mediator.Mediator,
            $$"""
            #nullable enable
            using Mediator;

            {{UnionContracts.Polyfill}}

            namespace Sample.Contracts
            {
                public sealed record RocketModel(System.Guid Id, string Name)
                {
                    public int Stages { get; init; }
                }

                public sealed record RocketNotFound(System.Guid Id);
                public sealed record Forbidden;

                [System.Runtime.CompilerServices.Union]
                public readonly struct EditRocketResult : System.Runtime.CompilerServices.IUnion
                {
                    public EditRocketResult(RocketModel value) => Value = value;
                    public EditRocketResult(RocketNotFound value) => Value = value;
                    public EditRocketResult(Forbidden value) => Value = value;
                    public object? Value { get; }
                }

                public static class EditRocket
                {
                    public sealed record Request(System.Guid Id, string Name) : ICommand<EditRocketResult>
                    {
                        public string? Notes { get; init; }
                    }
                }
            }
            """
        );
        const string documentation = """
            <?xml version="1.0"?>
            <doc>
                <assembly><name>Contracts</name></assembly>
                <members>
                    <member name="T:Sample.Contracts.RocketModel">
                        <summary>A rocket in the fleet.</summary>
                        <param name="Id">The rocket's id.</param>
                        <param name="Name">The rocket's name.</param>
                    </member>
                    <member name="P:Sample.Contracts.RocketModel.Stages"><summary>How many stages it has.</summary></member>
                    <member name="T:Sample.Contracts.RocketNotFound">
                        <summary>No rocket has the id.</summary>
                        <param name="Id">The id that wasn't found.</param>
                    </member>
                    <member name="T:Sample.Contracts.Forbidden"><summary>The caller may not edit rockets.</summary></member>
                    <member name="T:Sample.Contracts.EditRocket.Request">
                        <summary>Renames a rocket.</summary>
                        <param name="Id">The rocket to rename.</param>
                        <param name="Name">Its new name.</param>
                    </member>
                    <member name="P:Sample.Contracts.EditRocket.Request.Notes"><summary>Why it was renamed.</summary></member>
                </members>
            </doc>
            """;
        var builder = host == HostCase.Controller ? Controllers(Mediator.Mediator, contracts) : Server(Mediator.Mediator, contracts);
        var result = await builder.AddReferences(typeof(Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext).Assembly, typeof(Microsoft.OpenApi.OpenApiOperation).Assembly)
                                  .AddAdditionalText("Contracts.xml", documentation)
                                  .AddOption("Contracts.xml", "build_metadata.AdditionalFiles.LaunchPadApiDocumentation", "true")
                                  .AddSources(
                                       $"using Rocket.Surgery.LaunchPad.Api;\nusing Sample.Contracts;\n[Operation<EditRocket.Request>]\n{( host == HostCase.Controller ? "public partial class RocketsController;" : "public static partial class RocketsEndpoints;" )}"
                                   )
                                  .Build()
                                  .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(host);
    }

    // A body with rules (ScheduleLaunch: route id, body window/crew/contact) and a query string with rules (GetLaunchPlan); a rule with When
    // holds only sometimes and is left out.
    private static string ValidatedContracts(Mediator mediator)
    {
        var shape = UnionContracts.ShapeOf(mediator);
        string Request(string container, string parameters, string response) =>
            $$"""
                public static class {{container}}
                {
                    {{( shape.Marker is null ? $"[ApiOperation(Response = typeof({response}))] " : "" )}}[DataContract] public {{shape.Kind}} Request({{parameters}}){{( shape.Marker is { } m ? " : " + m(response) : "" )}};
                }
            """;

        return $$"""

            namespace Sample.Contracts.Rockets
            {
                using ZeroAlloc.Validation;

                public enum LaunchSite { Canaveral, Vandenberg }

            {{Request(
                "ScheduleLaunch",
                """
                [property: DataMember(Order = 1)] System.Guid Id,
                [property: DataMember(Order = 2), NotEmpty, MaxLength(40), Matches("^[A-Za-z ]+$")] string Window,
                [property: DataMember(Order = 3), InclusiveBetween(1, 10)] int Crew,
                [property: DataMember(Order = 4), EmailAddress, NotEqual("nobody@example.com")] string? Contact
                """,
                "LaunchRocketResult"
            )}}
            {{Request(
                "GetLaunchPlan",
                """
                [property: DataMember(Order = 1), Length(2, 20)] string Name,
                [property: DataMember(Order = 2), GreaterThan(0), LessThanOrEqualTo(100)] int Take,
                [property: DataMember(Order = 3), IsEnumName(typeof(LaunchSite)), MinLength(3, When = "HasSite")] string? Site
                """,
                "RocketModel"
            )}}
            }
            """;
    }

    // Union cases on GetRocket (404, 403), CreateRocket (201 with Location; 422, 409) and LaunchRocket, which also requires authorization.
    private static string Host(HostCase host) => $$"""
        #nullable enable
        using Rocket.Surgery.LaunchPad.Api;
        using Sample.Contracts.Rockets;

        namespace Sample.Server
        {
            [Operation<GetRocket.Request>]
            [Operation<CreateRocket.Request>]
            [Operation<LaunchRocket.Request>(Route = "{id}/launch", Policy = new[] { "launch" })]
            [Operation<ScheduleLaunch.Request>(Route = "{id}/schedule")]
            [Operation<GetLaunchPlan.Request>(Route = "plan")]
            {{( host == HostCase.Controller ? "public partial class RocketsController;" : "public static partial class RocketsEndpoints;" )}}
        }
        """;
}
