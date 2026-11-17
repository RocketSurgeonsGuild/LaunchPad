using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Analyzers.Api.CodeFixes;

namespace Api.Generators.Tests;

/// <summary>
///     LPAD1030: every body and response type of a Minimal API host needs source generated JSON metadata, trimmed or not.
/// </summary>
public class JsonMetadataTests() : GeneratorTest()
{
    protected override bool ReportsJsonMetadata => true;

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
    public async Task Should_Report_Undeclared_Types()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts).AddSources(Contracts.ServerHost).Build().GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result, settings => settings.UseFileName($"{nameof(JsonMetadataTests)}.Should_Report_Undeclared_Types"));
    }

    // The check doesn't depend on trimming or native AOT: a JIT application serializes through the same context.
    [Test]
    [Arguments("build_property.PublishAot", "true")]
    [Arguments("build_property.PublishAot", "false")]
    [Arguments("build_property.IsTrimmable", "false")]
    [Arguments("build_property.LaunchPadApiRequireJsonContext", "true")]
    public async Task Should_Report_Undeclared_Types_Whatever_The_Publish_Settings(string property, string value)
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddGlobalOption(property, value)
                          .AddSources(Contracts.ServerHost)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await result.Results.Values.SelectMany(r => r.Diagnostics).Count(d => d.Id == "LPAD1030").Should().BeEqualTo(4);
    }

    [Test]
    public async Task Should_Not_Report_When_Turned_Off()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddGlobalOption("build_property.LaunchPadApiRequireJsonContext", "false")
                          .AddSources(Contracts.ServerHost)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await result.Results.Values.SelectMany(r => r.Diagnostics).Should().All(d => d.Id != "LPAD1030");
    }

    [Test]
    public async Task Should_Not_Report_Types_A_Context_In_The_Project_Declares()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
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
                          .AddSources(Contracts.ServerHost)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await result.Results.Values.SelectMany(r => r.Diagnostics).Should().All(d => d.Id != "LPAD1030");
    }

    // The source generator writes metadata for everything a declared type reaches: properties, collections and their elements.
    [Test]
    public async Task Should_Not_Report_Types_A_Declared_Type_Reaches()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
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

        await result.Results.Values.SelectMany(r => r.Diagnostics).Should().All(d => d.Id != "LPAD1030");
    }

    [Test]
    public async Task Should_Report_Only_The_Types_No_Context_Declares()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddSources(Contracts.ServerHost, Context("Sample.Server", "Sample.Contracts.Rockets.RocketModel"))
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result, settings => settings.UseFileName($"{nameof(JsonMetadataTests)}.Should_Report_Only_The_Types_No_Context_Declares"));
    }

    // The types every test of the code fix leaves out of the referenced context, so only CreateRocket.Request is reported: the harness runs one
    // fix per diagnostic against the same workspace.
    private static readonly string[] AllButCreateRequest =
        ["Sample.Contracts.Rockets.RocketModel", "Sample.Contracts.Rockets.CreateRocket.Response", "Sample.Contracts.Rockets.EditRocket.Request"];

    private Task<GeneratorTestResults> ContractsDeclaringAllButCreateRequest() =>
        CompileContracts(
            Mediator.Mediator,
            Contracts.For(Mediator.Mediator) + "\n" + Context("Sample.Contracts", AllButCreateRequest),
            typeof(System.Text.Json.Serialization.JsonSerializerContext).Assembly
        );

    // The contexts are abstract: the System.Text.Json source generator, which would write their members, doesn't run here.
    // The fix writes the missing entry onto the project's own context, after its other entries, with a TypeInfoPropertyName for the nested type.
    [Test]
    public async Task Should_Fix_By_Adding_The_Type_To_The_Context_Of_The_Project()
    {
        var contracts = await ContractsDeclaringAllButCreateRequest();
        var result = await Server(Mediator.Mediator, contracts)
                          .AddSources(
                               Contracts.ServerHost,
                               """
                               using System.Text.Json.Serialization;
                               using Sample.Contracts.Rockets;

                               namespace Sample.Server
                               {
                                   /// <summary>The application's JSON metadata.</summary>
                                   [JsonSerializable(typeof(RocketModel))]
                                   [JsonSerializable(typeof(EditRocket.Request), TypeInfoPropertyName = "EditRocketRequest")]
                                   [System.Obsolete("not the last attribute")]
                                   internal abstract partial class AppJsonContext : JsonSerializerContext
                                   {
                                       protected AppJsonContext() : base(null) { }
                                   }
                               }
                               """
                           )
                          .WithCodeFix<AddJsonSerializableCodeFix>()
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result, settings => settings.UseFileName($"{nameof(JsonMetadataTests)}.Should_Fix_By_Adding_The_Type_To_The_Context_Of_The_Project"));
    }

    // A context with no entries yet and a doc comment: the entry goes between the comment and the declaration.
    [Test]
    public async Task Should_Fix_A_Context_Without_Entries()
    {
        var contracts = await ContractsDeclaringAllButCreateRequest();
        var result = await Server(Mediator.Mediator, contracts)
                          .AddSources(
                               Contracts.ServerHost,
                               """
                               namespace Sample.Server;

                               /// <summary>The application's JSON metadata.</summary>
                               public abstract partial class AppJsonContext : System.Text.Json.Serialization.JsonSerializerContext
                               {
                                   protected AppJsonContext() : base(null) { }
                               }
                               """
                           )
                          .WithCodeFix<AddJsonSerializableCodeFix>()
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result, settings => settings.UseFileName($"{nameof(JsonMetadataTests)}.Should_Fix_A_Context_Without_Entries"));
    }

    // Entries are found by symbol, not by the attribute's spelling: of a context's two partial declarations, the entry goes beside the one whose
    // entries are written through an alias.
    [Test]
    public async Task Should_Fix_Beside_Entries_Written_Through_An_Alias()
    {
        var contracts = await ContractsDeclaringAllButCreateRequest();
        var result = await Server(Mediator.Mediator, contracts)
                          .AddSources(
                               Contracts.ServerHost,
                               """
                               using Entry = System.Text.Json.Serialization.JsonSerializableAttribute;

                               namespace Sample.Server
                               {
                                   /// <summary>The application's JSON metadata.</summary>
                                   public abstract partial class AppJsonContext : System.Text.Json.Serialization.JsonSerializerContext
                                   {
                                       protected AppJsonContext() : base(null) { }
                                   }

                                   [Entry(typeof(Sample.Contracts.Rockets.RocketModel))]
                                   public abstract partial class AppJsonContext;
                               }
                               """
                           )
                          .WithCodeFix<AddJsonSerializableCodeFix>()
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result);
    }

    // Fix all: every missing type of the project lands on one new context, each with its own TypeInfoPropertyName.
    [Test]
    public async Task Should_Fix_All_By_Creating_One_Context_With_Every_Missing_Type()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddSources(Contracts.ServerHost)
                          .WithCodeFix<AddJsonSerializableCodeFix>()
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyFixAll<AddJsonSerializableCodeFix>(result, Microsoft.CodeAnalysis.CodeFixes.FixAllScope.Project);
    }

    // Fix all onto the project's context: the missing types go after its entries, a declared one isn't repeated.
    [Test]
    public async Task Should_Fix_All_Onto_The_Context_Of_The_Project()
    {
        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        var result = await Server(Mediator.Mediator, contracts)
                          .AddSources(
                               Contracts.ServerHost,
                               """
                               using System.Text.Json.Serialization;
                               using Sample.Contracts.Rockets;

                               namespace Sample.Server;

                               [JsonSerializable(typeof(RocketModel))]
                               internal abstract partial class AppJsonContext : JsonSerializerContext
                               {
                                   protected AppJsonContext() : base(null) { }
                               }
                               """
                           )
                          .WithCodeFix<AddJsonSerializableCodeFix>()
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyFixAll<AddJsonSerializableCodeFix>(result, Microsoft.CodeAnalysis.CodeFixes.FixAllScope.Solution);
    }

    // The only context is in a referenced assembly, which the fix can't edit: it creates one in the project, in the host's namespace.
    [Test]
    public async Task Should_Fix_By_Creating_A_Context_When_The_Project_Has_None()
    {
        var contracts = await ContractsDeclaringAllButCreateRequest();
        var result = await Server(Mediator.Mediator, contracts)
                          .AddSources(Contracts.ServerHost)
                          .WithCodeFix<AddJsonSerializableCodeFix>()
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result, settings => settings.UseFileName($"{nameof(JsonMetadataTests)}.Should_Fix_By_Creating_A_Context_When_The_Project_Has_None"));
    }
}
