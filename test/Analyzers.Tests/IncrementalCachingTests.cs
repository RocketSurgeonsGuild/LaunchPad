using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Rocket.Surgery.LaunchPad.Analyzers;

namespace Analyzers.Tests;

/// <summary>
///     The generators that read symbols keep none in their pipeline: an edit that leaves a declaration's output as it was must not run the
///     output step again, and an edit to what a declaration reads must.
/// </summary>
public class IncrementalCachingTests
{
    public static IEnumerable<Func<(IIncrementalGenerator Generator, string Step)>> Generators()
    {
        yield return () => (new PropertyTrackingGenerator(), "PropertyTracking");
        yield return () => (new GraphqlOptionalPropertyTrackingGenerator(), "OptionalTracking");
        yield return () => (new InheritFromGenerator(), "InheritFrom");
        yield return () => (new MutableGenerator(), "Mutable");
    }

    [Test]
    [MethodDataSource(nameof(Generators))]
    public async Task Should_Cache_The_Output_When_An_Unrelated_File_Changes(IIncrementalGenerator generator, string step)
    {
        var compilation = GeneratorBenchmarkTests.Compilation(types: 3);
        var driver = Driver(generator).RunGenerators(compilation);
        var unrelated = compilation.SyntaxTrees.Single(t => t.FilePath == "Unrelated.cs");
        driver = driver.RunGenerators(
            compilation.ReplaceSyntaxTree(unrelated, GeneratorBenchmarkTests.Parse("Unrelated.cs", "namespace Perf { public static class Unrelated { public const int Value = 2; } }"))
        );

        var reasons = Reasons(driver, step);
        await reasons.Length.Should().BeEqualTo(3);
        await reasons.Should().All(reason => reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged);
        await Reasons(driver, WellKnownGeneratorOutputs.SourceOutput).Should().All(reason => reason == IncrementalStepRunReason.Cached);
    }

    [Test]
    [MethodDataSource(nameof(Generators))]
    public async Task Should_Rerun_When_What_A_Declaration_Reads_Changes(IIncrementalGenerator generator, string step)
    {
        var compilation = GeneratorBenchmarkTests.Compilation(types: 3);
        var driver = Driver(generator).RunGenerators(compilation);
        var model = compilation.SyntaxTrees.Single(t => t.FilePath == "Type0.cs");
        driver = driver.RunGenerators(compilation.ReplaceSyntaxTree(model, GeneratorBenchmarkTests.Parse("Type0.cs", GeneratorBenchmarkTests.Source(0, extraProperty: true))));

        await Reasons(driver, step).Should().Contain(IncrementalStepRunReason.Modified);
        await driver.GetRunResult().GeneratedTrees.Any(t => t.ToString().Contains("Extra", StringComparison.Ordinal)).Should().BeTrue();
    }

    private static GeneratorDriver Driver(IIncrementalGenerator generator) =>
        CSharpGeneratorDriver.Create(
            [generator.AsSourceGenerator()],
            parseOptions: new CSharpParseOptions(LanguageVersion.Preview),
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true)
        );

    private static IncrementalStepRunReason[] Reasons(GeneratorDriver driver, string step) =>
        [.. driver.GetRunResult()
              .Results.SelectMany(r => r.TrackedSteps.TryGetValue(step, out var runs) ? runs : r.TrackedOutputSteps.TryGetValue(step, out var outputs) ? outputs : [])
              .SelectMany(run => run.Outputs.Select(o => o.Reason))];
}
