using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Primitives.Tests.Generator;

using Rocket.Surgery.LaunchPad.Primitives;

namespace Primitives.Tests;

/// <summary>
///     The domain primitive and registry generators keep no symbol or compilation in their pipelines: an edit that changes no domain primitive
///     must not write anything again, and an edit to one must.
/// </summary>
public class IncrementalCachingTests
{
    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Preview);

    private const string Registry = """
        namespace Rocket.Surgery.LaunchPad.Primitives.Generated
        {
            internal static partial class LaunchPadPrimitives;
        }
        """;

    private static string Primitives(int maxLength) => $$"""
        using Rocket.Surgery.LaunchPad.Primitives;

        namespace Sample.Application
        {
            [Email]
            public readonly partial record struct EmailAddress;

            [StrongId<System.Guid>]
            public sealed partial record OrderId;

            [StringPrimitive, MaxLength({{maxLength}})]
            public sealed partial record Name;
        }
        """;

    [Test]
    public async Task Should_Cache_Everything_When_An_Unrelated_File_Changes()
    {
        var (driver, compilation) = Run();
        var unrelated = compilation.SyntaxTrees.Single(t => t.FilePath == "Unrelated.cs");
        driver = driver.RunGenerators(compilation.ReplaceSyntaxTree(unrelated, Parse("Unrelated.cs", "namespace Sample { public static class Unrelated { public const int Value = 2; } }")));

        // Each step ran (an empty list would pass the checks below), with three domain primitives and a registry.
        await Reasons(driver, "DomainPrimitiveModels").Length.Should().BeEqualTo(3);
        await Reasons(driver, "PrimitiveRegistry").Length.Should().BeEqualTo(1);
        await Reasons(driver, WellKnownGeneratorOutputs.SourceOutput).Length.Should().BeGreaterThan(3);
        await Reasons(driver, "DomainPrimitiveModels").Should().All(reason => reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged);
        await Reasons(driver, "DomainPrimitives").Should().All(reason => reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged);
        await Reasons(driver, "PrimitiveRegistryLocalTypes").Should().All(reason => reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged);
        await Reasons(driver, "PrimitiveRegistry").Should().All(reason => reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged);
        await Reasons(driver, WellKnownGeneratorOutputs.SourceOutput).Should().All(reason => reason == IncrementalStepRunReason.Cached);
    }

    [Test]
    public async Task Should_Rerun_When_A_Domain_Primitive_Changes()
    {
        var (driver, compilation) = Run();
        var primitives = compilation.SyntaxTrees.Single(t => t.FilePath == "Primitives.cs");
        driver = driver.RunGenerators(compilation.ReplaceSyntaxTree(primitives, Parse("Primitives.cs", Primitives(maxLength: 20))));

        await Reasons(driver, "DomainPrimitives").Should().Contain(IncrementalStepRunReason.Modified);
        await driver.GetRunResult().GeneratedTrees.Any(t => t.ToString().Contains("20", StringComparison.Ordinal)).Should().BeTrue();
    }

    private static SyntaxTree Parse(string path, string source) => CSharpSyntaxTree.ParseText(source, ParseOptions, path);

    private static (GeneratorDriver Driver, CSharpCompilation Compilation) Run()
    {
        var compilation = CSharpCompilation.Create(
            "Application",
            [Parse("Registry.cs", Registry), Parse("Primitives.cs", Primitives(maxLength: 10)), Parse("Unrelated.cs", "namespace Sample { public static class Unrelated { public const int Value = 1; } }")],
            [
                .. GeneratorTestBuilder.Framework.Select(path => MetadataReference.CreateFromFile(path)),
                MetadataReference.CreateFromFile(typeof(PrimitiveError).Assembly.Location),
            ],
            new(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable)
        );
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new PrimitiveGenerators().AsSourceGenerator()],
            parseOptions: ParseOptions,
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true)
        );
        return (driver.RunGenerators(compilation), compilation);
    }

    private static IncrementalStepRunReason[] Reasons(GeneratorDriver driver, string step) =>
        [.. driver.GetRunResult()
              .Results.SelectMany(r => r.TrackedSteps.TryGetValue(step, out var runs) ? runs : r.TrackedOutputSteps.TryGetValue(step, out var outputs) ? outputs : [])
              .SelectMany(run => run.Outputs.Select(o => o.Reason))];
}
