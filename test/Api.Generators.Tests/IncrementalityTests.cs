using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Rocket.Surgery.LaunchPad.Api.Generators;

namespace Api.Generators.Tests;

/// <summary>
///     The emit step must be skipped when nothing an operation depends on changed, and must re-run when a listed request changes, even in another
///     file than the host.
/// </summary>
public class IncrementalityTests() : GeneratorTest()
{
    private const string Contracts = """
        #nullable enable
        using Mediator;
        using Rocket.Surgery.LaunchPad.Api;
        namespace Sample
        {
            public sealed record RocketModel(System.Guid Id);
            public static class GetRocket
            {
                [ApiOperation(Summary = "Load a rocket")]
                public sealed record Request(System.Guid Id) : IRequest<RocketModel>;
            }
        }
        """;

    private const string Host = """
        using Rocket.Surgery.LaunchPad.Api;
        [Operation<Sample.GetRocket.Request>]
        public static partial class RocketsEndpoints;
        """;

    [Test]
    public async Task Should_Skip_Emission_When_An_Unrelated_File_Changes()
    {
        var (driver, compilation) = await Run();
        var edited = compilation.AddSyntaxTrees(Parse("public static class Unrelated { public const int Value = 1; }"));
        driver = driver.RunGenerators(edited, TestContext.CancellationToken);

        await Reasons(driver, "Hosts").Should().All(reason => reason is IncrementalStepRunReason.Unchanged or IncrementalStepRunReason.Cached);
        await Task.CompletedTask;
    }

    [Test]
    public async Task Should_Rerun_When_A_Listed_Request_Changes_In_Another_File()
    {
        var (driver, compilation) = await Run();
        var contracts = compilation.SyntaxTrees.Single(t => t.ToString().Contains("Load a rocket", StringComparison.Ordinal));
        var edited = compilation.ReplaceSyntaxTree(contracts, Parse(Contracts.Replace("Load a rocket", "Fetch a rocket", StringComparison.Ordinal)));
        driver = driver.RunGenerators(edited, TestContext.CancellationToken);

        await Reasons(driver, "Hosts").Should().Contain(IncrementalStepRunReason.Modified);
        await driver.GetRunResult().GeneratedTrees.Any(t => t.ToString().Contains("Fetch a rocket", StringComparison.Ordinal)).Should().BeTrue();
        await Task.CompletedTask;
    }

    private static SyntaxTree Parse(string source) =>
        CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview, preprocessorSymbols: ["NET7_0_OR_GREATER"]));

    private async Task<(GeneratorDriver Driver, CSharpCompilation Compilation)> Run()
    {
        var compilation = CSharpCompilation.Create(
            "Server",
            [Parse(Contracts), Parse(Host)],
            FrameworkReferences.Concat(MediatorAssemblies(Mediator.Mediator).Select(a => MetadataReference.CreateFromFile(a.Location))),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new AttributesGenerator().AsSourceGenerator(), new MinimalApiGenerator().AsSourceGenerator()],
            parseOptions: (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true)
        );
        driver = driver.RunGenerators(compilation, TestContext.CancellationToken);
        await driver.GetRunResult().GeneratedTrees.Any(t => t.FilePath.EndsWith("RocketsEndpoints.MinimalApi.g.cs", StringComparison.Ordinal)).Should().BeTrue();
        return (driver, compilation);
    }

    private static IncrementalStepRunReason[] Reasons(GeneratorDriver driver, string step) =>
        [.. driver.GetRunResult()
              .Results.SelectMany(r => r.TrackedSteps.TryGetValue(step, out var runs) ? runs : [])
              .SelectMany(run => run.Outputs.Select(o => o.Reason))];
}
