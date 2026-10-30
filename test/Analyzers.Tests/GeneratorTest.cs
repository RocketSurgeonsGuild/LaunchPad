using System.Reflection;
using System.Runtime.Loader;

using HotChocolate.Types;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

using TUnit.Core.Interfaces;

namespace Analyzers.Tests;

internal sealed class CollectibleTestAssemblyLoadContext() : AssemblyLoadContext(true), IDisposable
{
    public void Dispose() => Unload();

    protected override Assembly? Load(AssemblyName assemblyName) => null;
}

public abstract class GeneratorTest() : LoggerTest<TestRecord>(TestRecord.Create()), IAsyncInitializer
{
    public virtual Task InitializeAsync()
    {
        // LoggerTest disposes these when the test ends, which unloads the collectible context.
        if (AssemblyLoadContext is IDisposable disposable) Disposables.Add(disposable);

        Builder = GeneratorTestContextBuilder
                 .Create()
                 .WithLogger(Logger)
                 .WithAssemblyLoadContext(AssemblyLoadContext)
                 .AddReferences(
                      typeof(ActivatorUtilities).Assembly,
                      typeof(ConventionContext).Assembly,
                      typeof(IConventionContext).Assembly,
                      typeof(ErrorAttribute<>).Assembly
                  );

        return Task.CompletedTask;
    }

    public AssemblyLoadContext AssemblyLoadContext { get; } = new CollectibleTestAssemblyLoadContext();
    protected GeneratorTestContextBuilder Builder { get; set; } = null!;
}
