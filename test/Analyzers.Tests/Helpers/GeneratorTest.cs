using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Analyzers.Tests.Helpers;

public abstract class GeneratorTest : LoggerTest
{
    private readonly HashSet<MetadataReference> _metadataReferences = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<Type> _generators = new();
    private readonly List<string> _sources = new();

    protected GeneratorTest(ITestOutputHelper testOutputHelper, LogLevel minLevel) : base(testOutputHelper, minLevel)
    {
        AddReferences(
            "mscorlib.dll",
            "netstandard.dll",
            "System.dll",
            "System.Core.dll",
#if NETCOREAPP
            "System.Private.CoreLib.dll",
#endif
            "System.Runtime.dll"
        );

        AddReferences(
            typeof(ActivatorUtilities).Assembly,
            typeof(ConventionAttribute).Assembly,
            typeof(ConventionContext).Assembly,
            typeof(IConventionContext).Assembly
        );
    }

    protected GeneratorTest WithGenerator(Type type)
    {
        _generators.Add(type);
        return this;
    }

    protected GeneratorTest WithGenerator<T>()
        where T : new()
    {
        _generators.Add(typeof(T));
        return this;
    }

    protected GeneratorTest AddReferences(params string[] coreAssemblyNames)
    {
        // this "core assemblies hack" is from https://stackoverflow.com/a/47196516/4418060
        var coreAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        foreach (var name in coreAssemblyNames)
        {
            _metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(coreAssemblyPath, name)));
        }

        return this;
    }

    protected GeneratorTest AddReferences(params MetadataReference[] references)
    {
        foreach (var metadataReference in references)
        {
            _metadataReferences.Add(metadataReference);
        }

        return this;
    }

    protected GeneratorTest AddReferences(params Type[] references)
    {
        foreach (var type in references)
        {
            _metadataReferences.Add(MetadataReference.CreateFromFile(type.Assembly.Location));
        }

        return this;
    }

    protected GeneratorTest AddReferences(params Assembly[] references)
    {
        foreach (var type in references)
        {
            _metadataReferences.Add(MetadataReference.CreateFromFile(type.Location));
        }

        return this;
    }

    protected GeneratorTest AddSources(params string[] sources)
    {
        _sources.AddRange(sources);
        return this;
    }

    public async Task<GenerationTestResults> GenerateAsync(params string[] sources)
    {
        Logger.LogInformation("Starting Generation for {SourceCount}", sources.Length);
        if (Logger.IsEnabled(LogLevel.Trace))
        {
            Logger.LogTrace("--- References {Count} ---", _metadataReferences.Count);
            foreach (var reference in _metadataReferences)
                Logger.LogTrace("    Reference: {Name}", reference.Display);
        }

        var project = GenerationHelpers.CreateProject(_metadataReferences, _sources.Concat(sources).ToArray());

        var compilation = (CSharpCompilation?)await project.GetCompilationAsync().ConfigureAwait(false);
        if (compilation is null)
        {
            throw new InvalidOperationException("Could not compile the sources");
        }

        var diagnostics = compilation.GetDiagnostics();
        if (Logger.IsEnabled(LogLevel.Trace) && diagnostics is { Length: > 0 })
        {
            Logger.LogTrace("--- Input Diagnostics {Count} ---", diagnostics.Length);
            foreach (var d in diagnostics)
                Logger.LogTrace("    Reference: {Name}", d.ToString());
        }

        var results = new GenerationTestResults(
            compilation,
            diagnostics,
            compilation.SyntaxTrees,
            ImmutableDictionary<Type, GenerationTestResult>.Empty,
            ImmutableArray<Diagnostic>.Empty
        );

        var builder = ImmutableDictionary<Type, GenerationTestResult>.Empty.ToBuilder();

        Assert.NotEmpty(_generators);
        foreach (var generatorType in _generators)
        {
            Logger.LogInformation("--- {Generator} ---", generatorType.FullName);
            var generator = ( Activator.CreateInstance(generatorType) as IIncrementalGenerator )!;
            var _ = CSharpGeneratorDriver.Create(generator).RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out diagnostics);

            if (Logger.IsEnabled(LogLevel.Trace) && diagnostics is { Length: > 0 })
            {
                results = results with { ResultDiagnostics = results.ResultDiagnostics.AddRange(diagnostics) };
                Logger.LogTrace("--- Diagnostics {Count} ---", sources.Length);
                foreach (var d in diagnostics)
                    Logger.LogTrace("    Reference: {Name}", d.ToString());
            }

            var trees = outputCompilation.SyntaxTrees.Except(compilation.SyntaxTrees).ToImmutableArray();
            if (Logger.IsEnabled(LogLevel.Trace) && trees is { Length: > 0 })
            {
                Logger.LogTrace("--- Syntax Trees {Count} ---", sources.Length);
                foreach (var t in trees)
                {
                    Logger.LogTrace("    FilePath: {Name}", t.FilePath);
                    Logger.LogTrace("    Source:\n{Name}", ( await t.GetTextAsync().ConfigureAwait(false) ).ToString());
                }
            }

            builder.Add(generatorType, new GenerationTestResult(( outputCompilation as CSharpCompilation )!, diagnostics, trees, Logger));
        }

        return results with { Results = builder.ToImmutable() };
    }
}
