using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Analyzers.Tests.Helpers;

internal sealed class CollectibleTestAssemblyLoadContext() : AssemblyLoadContext(true), IDisposable
{
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        return null;
    }

    public void Dispose()
    {
        Unload();
    }
}

public abstract class GeneratorTest(ITestOutputHelper testOutputHelper) : LoggerTest(testOutputHelper, LogLevel.Trace), IAsyncLifetime
{
    private readonly HashSet<MetadataReference> _metadataReferences = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<Type> _generators = new();
    private readonly List<string> _sources = new();

    public AssemblyLoadContext AssemblyLoadContext { get; } = new CollectibleTestAssemblyLoadContext();
    protected GeneratorTestContextBuilder Builder { get; set; } = null!;

    public virtual Task InitializeAsync()
    {
        Builder = GeneratorTestContextBuilder
                 .Create()
                 .WithLogger(Logger)
                 .WithAssemblyLoadContext(AssemblyLoadContext)
                 .AddReferences(
                      typeof(ActivatorUtilities).Assembly,
                      typeof(ConventionContext).Assembly,
                      typeof(IConventionContext).Assembly
                  );

        return Task.CompletedTask;
    }

    public virtual Task DisposeAsync()
    {
        if (AssemblyLoadContext is IDisposable disposable)
        {
            Disposables.Add(disposable);
        }

        Disposables.Dispose();
        return Task.CompletedTask;
    }
}
