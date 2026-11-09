namespace Api.Generators.Tests;

/// <summary>
///     LPAD1030: in a trimmed or native AOT project, every body and response type of a Minimal API host needs source generated JSON metadata.
/// </summary>
public class JsonMetadataTests() : GeneratorTest()
{
    // A stand-in for a source generated context: the check reads the [JsonSerializable] attributes, so the System.Text.Json source generator
    // doesn't need to run, and an abstract class compiles without the members it would write.
    private static string Context(string ns, params string[] types) => $$"""
        namespace {{ns}}
        {
        {{string.Join("\n", types.Select(t => $"    [System.Text.Json.Serialization.JsonSerializable(typeof({t}))]"))}}
            public abstract partial class RocketsJsonContext : System.Text.Json.Serialization.JsonSerializerContext
            {
                protected RocketsJsonContext() : base(null) { }
            }
        }
        """;

    private static readonly string[] EveryType =
        ["Sample.Contracts.Rockets.RocketModel", "Sample.Contracts.Rockets.CreateRocket.Request", "Sample.Contracts.Rockets.CreateRocket.Response", "Sample.Contracts.Rockets.EditRocket.Request"];

    [Test]
    public async Task Should_Report_Undeclared_Types_When_Publishing_Aot()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddGlobalOption("build_property.PublishAot", "true")
                          .AddSources(Contracts.ServerHost)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result, settings => settings.UseFileName($"{nameof(JsonMetadataTests)}.Should_Report_Undeclared_Types_When_Publishing_Aot"));
    }

    [Test]
    [Arguments("build_property.IsAotCompatible")]
    [Arguments("build_property.PublishTrimmed")]
    [Arguments("build_property.IsTrimmable")]
    [Arguments("build_property.LaunchPadApiRequireJsonContext")]
    public async Task Should_Report_Undeclared_Types_When_Trimmed(string property)
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddGlobalOption(property, "true")
                          .AddSources(Contracts.ServerHost)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        result.Results.Values.SelectMany(r => r.Diagnostics).Count(d => d.Id == "LPAD1030").ShouldBe(4);
    }

    [Test]
    public async Task Should_Not_Report_Without_Trimming()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts).AddSources(Contracts.ServerHost).Build().GenerateAsync(TestContext.CancellationToken);

        result.Results.Values.SelectMany(r => r.Diagnostics).ShouldNotContain(d => d.Id == "LPAD1030");
    }

    [Test]
    public async Task Should_Not_Report_When_Turned_Off()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddGlobalOption("build_property.PublishAot", "true")
                          .AddGlobalOption("build_property.LaunchPadApiRequireJsonContext", "false")
                          .AddSources(Contracts.ServerHost)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        result.Results.Values.SelectMany(r => r.Diagnostics).ShouldNotContain(d => d.Id == "LPAD1030");
    }

    [Test]
    public async Task Should_Not_Report_Types_A_Context_In_The_Project_Declares()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddGlobalOption("build_property.PublishAot", "true")
                          .AddSources(Contracts.ServerHost, Context("Sample.Server", EveryType))
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result, settings => settings.UseFileName($"{nameof(JsonMetadataTests)}.Should_Not_Report_Types_A_Context_In_The_Project_Declares"));
    }

    [Test]
    public async Task Should_Not_Report_Types_A_Context_In_A_Referenced_Assembly_Declares()
    {
        var contracts = await CompileContracts(
            Mediator.Mediator,
            Contracts.For(Mediator.Mediator) + "\n" + Context("Sample.Contracts", EveryType),
            typeof(System.Text.Json.Serialization.JsonSerializerContext).Assembly
        );
        var result = await Server(Mediator.Mediator, contracts)
                          .AddGlobalOption("build_property.PublishAot", "true")
                          .AddSources(Contracts.ServerHost)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        result.Results.Values.SelectMany(r => r.Diagnostics).ShouldNotContain(d => d.Id == "LPAD1030");
    }

    // The source generator writes metadata for everything a declared type reaches: properties, collections and their elements.
    [Test]
    public async Task Should_Not_Report_Types_A_Declared_Type_Reaches()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddGlobalOption("build_property.PublishAot", "true")
                          .AddSources(
                               Contracts.ServerHost,
                               """
                               namespace Sample.Server
                               {
                                   public sealed record Catalog(
                                       Sample.Contracts.Rockets.RocketModel[] Rockets,
                                       System.Collections.Generic.List<Sample.Contracts.Rockets.CreateRocket.Request> Creates,
                                       System.Collections.Generic.Dictionary<string, Sample.Contracts.Rockets.EditRocket.Request> Edits
                                   )
                                   {
                                       public Sample.Contracts.Rockets.CreateRocket.Response? Created { get; init; }
                                   }
                               }
                               """,
                               Context("Sample.Server", "Sample.Server.Catalog")
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        result.Results.Values.SelectMany(r => r.Diagnostics).ShouldNotContain(d => d.Id == "LPAD1030");
    }

    [Test]
    public async Task Should_Report_Only_The_Types_No_Context_Declares()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddGlobalOption("build_property.PublishAot", "true")
                          .AddSources(Contracts.ServerHost, Context("Sample.Server", "Sample.Contracts.Rockets.RocketModel"))
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result, settings => settings.UseFileName($"{nameof(JsonMetadataTests)}.Should_Report_Only_The_Types_No_Context_Declares"));
    }
}
