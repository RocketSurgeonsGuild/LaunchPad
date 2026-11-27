using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Rocket.Surgery.LaunchPad.Analyzers.Api;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Controllers;
using Rocket.Surgery.LaunchPad.Analyzers.Api.GraphQL;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Grpc;
using Rocket.Surgery.LaunchPad.Analyzers.Api.MinimalApi;
using Rocket.Surgery.LaunchPad.Tests;

namespace Api.Generators.Tests;

/// <summary>
///     Times the transport generators over two operations for each of <see cref="Operations" /> indexes (a query and a command) in <see cref="Files" /> contract files, each listed on a
///     minimal API host, a controller, a gRPC service and a GraphQL mutation type. Explicit: it measures, and asserts nothing beyond the
///     generators running cleanly.
/// </summary>
[Explicit]
[NotInParallel]
public class GeneratorBenchmarkTests() : GeneratorTest()
{
    private const int Files = 10;
    private const int OperationsPerFile = 4;
    private const int Operations = Files * OperationsPerFile;

    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Preview, preprocessorSymbols: ["NET7_0_OR_GREATER"]);

    [Test]
    public async Task Benchmark_Generators()
    {
        var baseline = Compilation();
        var unrelatedTree = baseline.SyntaxTrees.Single(t => t.FilePath == "Unrelated.cs");
        var unrelated = baseline.ReplaceSyntaxTree(unrelatedTree, Parse("Unrelated.cs", "namespace Perf { public static class Unrelated { public const int Value = 2; } }"));
        var contractsTree = baseline.SyntaxTrees.Single(t => t.FilePath == "Contracts0.cs");
        var input = baseline.ReplaceSyntaxTree(contractsTree, Parse("Contracts0.cs", ContractsSource(0, "Fetch")));

        IIncrementalGenerator[] generators = [new MinimalApiGenerator(), new ControllerGenerator(), new GrpcServiceGenerator(), new GraphQLMutationGenerator()];
        var measurements = new List<GeneratorBenchmark.Measurement>();
        var errors = new List<Diagnostic>();
        foreach (var generator in generators)
        {
            var result = Driver(generator).RunGenerators(baseline).GetRunResult();
            errors.AddRange(result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            await result.GeneratedTrees.Length.Should().BeGreaterThan(1);
            measurements.AddRange(GeneratorBenchmark.Measure(generator.GetType().Name, () => Driver(generator), baseline, unrelated, input));
        }

        await errors.ConvertAll(d => d.ToString()).Should().BeEmpty();
        GeneratorBenchmark.Report($"Api ({Operations * 2} operations)", measurements);
    }

    private static GeneratorDriver Driver(IIncrementalGenerator generator) =>
        CSharpGeneratorDriver.Create([new AttributesGenerator().AsSourceGenerator(), generator.AsSourceGenerator()], parseOptions: ParseOptions);

    private static SyntaxTree Parse(string path, string source) => CSharpSyntaxTree.ParseText(source, ParseOptions, path);

    private static CSharpCompilation Compilation() =>
        CSharpCompilation.Create(
            "Server",
            [
                .. Enumerable.Range(0, Files).Select(i => Parse($"Contracts{i}.cs", ContractsSource(i, "Load"))),
                Parse("Hosts.cs", HostsSource()),
                Parse("Unrelated.cs", "namespace Perf { public static class Unrelated { public const int Value = 1; } }"),
            ],
            FrameworkReferences.Concat(
                [
                    .. MediatorAssemblies(Mediator.Mediator).Select(a => MetadataReference.CreateFromFile(a.Location)),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.Grpc.Configuration.ServiceAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.Grpc.Server.ServicesExtensions).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.ProtoContractAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(HotChocolate.Types.ObjectTypeAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(HotChocolate.Execution.Configuration.IRequestExecutorBuilder).Assembly.Location),
                ]
            ),
            new(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable)
        );

    private static string ContractsSource(int file, string verb) =>
        $$"""
        #nullable enable
        using Mediator;
        using Rocket.Surgery.LaunchPad.Api;

        namespace Perf.Contracts{{file}}
        {
            public sealed record RocketModel(System.Guid Id, string Name, int Stage);
        {{string.Concat(Enumerable.Range(0, OperationsPerFile).Select(op => $$"""

            public static class GetRocket{{file * OperationsPerFile + op}}
            {
                [ApiOperation(Summary = "{{verb}} a rocket")]
                public sealed record Request(System.Guid Id) : IQuery<RocketModel?>;
            }

            public static class EditRocket{{file * OperationsPerFile + op}}
            {
                public sealed record Request(System.Guid Id, string Name, int Stage) : ICommand<RocketModel>;
            }
        """))}}
        }
        """;

    private static string HostsSource()
    {
        var operations = string.Concat(
            Enumerable
               .Range(0, Files)
               .SelectMany(file => Enumerable.Range(0, OperationsPerFile).Select(op => $"[Operation<Perf.Contracts{file}.GetRocket{file * OperationsPerFile + op}.Request>][Operation<Perf.Contracts{file}.EditRocket{file * OperationsPerFile + op}.Request>]\n"))
        );
        var mutations = string.Concat(
            Enumerable.Range(0, Files).SelectMany(file => Enumerable.Range(0, OperationsPerFile).Select(op => $"[Operation<Perf.Contracts{file}.EditRocket{file * OperationsPerFile + op}.Request>]\n"))
        );
        return $$"""
            #nullable enable
            using Rocket.Surgery.LaunchPad.Api;

            namespace Perf.Server
            {
                {{operations}}
                public static partial class RocketsEndpoints;

                {{operations}}
                public partial class RocketsController;

                {{operations}}
                public partial class RocketsService;

                {{mutations}}
                public partial class RocketsMutations;
            }
            """;
    }
}
