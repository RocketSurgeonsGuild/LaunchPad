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

public abstract class GeneratorTest(ITestContextAccessor testContext) : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testContext)), IAsyncLifetime
{
    public virtual ValueTask InitializeAsync()
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

        return ValueTask.CompletedTask;
    }

    public virtual ValueTask DisposeAsync()
    {
        if (AssemblyLoadContext is IDisposable disposable) Disposables.Add(disposable);

        Disposables.Dispose();
        return ValueTask.CompletedTask;
    }

    public AssemblyLoadContext AssemblyLoadContext { get; } = new CollectibleTestAssemblyLoadContext();
    protected GeneratorTestContextBuilder Builder { get; set; } = null!;
}
