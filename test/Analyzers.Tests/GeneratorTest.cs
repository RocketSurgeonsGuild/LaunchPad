using System.Reflection;
using System.Runtime.Loader;

using HotChocolate.Types;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Analyzers.Tests;

internal sealed class CollectibleTestAssemblyLoadContext() : AssemblyLoadContext(true), IDisposable
{
    public void Dispose() => Unload();

    protected override Assembly? Load(AssemblyName assemblyName) => null;
}

public abstract class GeneratorTest(ITestOutputHelper testOutputHelper) : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testOutputHelper)), IAsyncLifetime
{
    public virtual Task InitializeAsync()
    {
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

    public virtual Task DisposeAsync()
    {
        if (AssemblyLoadContext is IDisposable disposable) Disposables.Add(disposable);

        Disposables.Dispose();
        return Task.CompletedTask;
    }

    public AssemblyLoadContext AssemblyLoadContext { get; } = new CollectibleTestAssemblyLoadContext();
    protected GeneratorTestContextBuilder Builder { get; set; } = null!;
}
