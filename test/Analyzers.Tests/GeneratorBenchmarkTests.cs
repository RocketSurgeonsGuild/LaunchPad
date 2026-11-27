using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.HotChocolate;
using Rocket.Surgery.LaunchPad.Tests;

namespace Analyzers.Tests;

/// <summary>
///     Times property tracking, optional tracking, InheritFrom and Mutable over a project of <see cref="Types" /> files, each declaring a model
///     and one type for each generator. Explicit: it measures, and asserts nothing beyond the generators running cleanly.
/// </summary>
[Explicit]
[NotInParallel]
public class GeneratorBenchmarkTests
{
    private const int Types = 40;

    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Preview);

    [Test]
    public async Task Benchmark_Generators()
    {
        var baseline = Compilation();
        var unrelatedTree = baseline.SyntaxTrees.Single(t => t.FilePath == "Unrelated.cs");
        var unrelated = baseline.ReplaceSyntaxTree(unrelatedTree, Parse("Unrelated.cs", "namespace Perf { public static class Unrelated { public const int Value = 2; } }"));
        var modelTree = baseline.SyntaxTrees.Single(t => t.FilePath == "Type0.cs");
        var input = baseline.ReplaceSyntaxTree(modelTree, Parse("Type0.cs", Source(0, extraProperty: true)));

        IIncrementalGenerator[] generators =
            [new PropertyTrackingGenerator(), new GraphqlOptionalPropertyTrackingGenerator(), new InheritFromGenerator(), new MutableGenerator()];
        // Before generation only the members the generators write are missing; any unresolved name is a broken setup.
        await baseline.GetDiagnostics().Where(d => d.Id is "CS0234" or "CS0246").Select(d => d.ToString()).ToArray().Should().BeEmpty();
        var measurements = new List<GeneratorBenchmark.Measurement>();
        foreach (var generator in generators)
        {
            var result = Driver(generator).RunGenerators(baseline).GetRunResult();
            await result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();
            await result.GeneratedTrees.Length.Should().BeGreaterThan(0);
            measurements.AddRange(GeneratorBenchmark.Measure(generator.GetType().Name, () => Driver(generator), baseline, unrelated, input));
        }

        GeneratorBenchmark.Report($"Analyzers ({Types} types)", measurements);
    }

    private static GeneratorDriver Driver(IIncrementalGenerator generator) =>
        CSharpGeneratorDriver.Create([generator.AsSourceGenerator()], parseOptions: ParseOptions);

    internal static SyntaxTree Parse(string path, string source) => CSharpSyntaxTree.ParseText(source, ParseOptions, path);

    internal static CSharpCompilation Compilation(int types = Types)
    {
        // What the sources use, plus everything the test process has loaded (the framework).
        var references = AppDomain
                        .CurrentDomain.GetAssemblies()
                        .Concat(
                             [
                                 typeof(IPropertyTracking<>).Assembly,
                                 typeof(IOptionalTracking<>).Assembly,
                                 typeof(ZeroAlloc.Mediator.IRequest<>).Assembly,
                                 typeof(ZeroAlloc.Validation.ValidatorFor<>).Assembly,
                                 typeof(ActivatorUtilities).Assembly,
                                 typeof(HotChocolate.Types.ErrorAttribute<>).Assembly,
                             ]
                         )
                        .Where(a => !a.IsDynamic && a.Location.Length > 0)
                        .Distinct()
                        .Select(a => (MetadataReference)MetadataReference.CreateFromFile(a.Location))
                        .ToArray();
        return CSharpCompilation.Create(
            "Perf",
            [
                .. Enumerable.Range(0, types).Select(i => Parse($"Type{i}.cs", Source(i, extraProperty: false))),
                Parse("Unrelated.cs", "namespace Perf { public static class Unrelated { public const int Value = 1; } }"),
            ],
            references,
            new(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable)
        );
    }

    internal static string Source(int i, bool extraProperty) => $$"""
        using System;
        using System.Collections.Generic;
        using Rocket.Surgery.LaunchPad.Foundation;
        using Rocket.Surgery.LaunchPad.HotChocolate;
        using ZeroAlloc.Mediator;

        namespace Perf.Rockets{{i}}
        {
            public partial record Model{{i}}
            {
                public Guid Id { get; init; }
                public string SerialNumber { get; set; } = "";
                public int Type { get; set; }
                public DateTimeOffset Planned { get; set; }
                public IReadOnlyList<string> Tags { get; set; } = [];
                {{( extraProperty ? "public long Extra { get; set; }" : "" )}}
            }

            [InheritFrom(typeof(Model{{i}}))]
            public partial record Request{{i}} : IRequest<Model{{i}}>
            {
                public Guid Other { get; init; }
            }

            public partial record Patch{{i}} : IPropertyTracking<Model{{i}}>, IRequest<Model{{i}}>
            {
                public Guid Id { get; init; }
                {{( extraProperty ? "public long PatchExtra { get; set; }" : "" )}}
            }

            public partial record Graph{{i}} : IOptionalTracking<Patch{{i}}>
            {
                public Guid Id { get; init; }
            }

            [Mutable(typeof(Model{{i}}))]
            public partial class Model{{i}}ViewModel;
        }
        """;
}
