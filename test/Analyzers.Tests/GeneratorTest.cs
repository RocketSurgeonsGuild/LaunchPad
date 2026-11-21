using System.Reflection;
using System.Runtime.Loader;

using HotChocolate.Types;

using Microsoft.Extensions.DependencyInjection;

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
                      typeof(ErrorAttribute<>).Assembly
                  );

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Compiles <paramref name="source" /> on its own, without the test's generators, as the assembly of a referenced project
    ///     (a <c>Sample.Core</c>-style contracts library), to add to the test's builder with <c>AddCompilationReferences</c>.
    /// </summary>
    protected Task<GeneratorTestResults> CompileReferencedAssembly(
        string projectName,
        IEnumerable<Type> references,
        string source,
        CancellationToken cancellationToken
    ) =>
        GeneratorTestContextBuilder
           .Create()
           .WithLogger(Logger)
           .WithAssemblyLoadContext(AssemblyLoadContext)
           .WithProjectName(projectName)
           .AddReferences([.. references])
           // a contracts library is compiled with nullable reference types, so its metadata carries the annotations
           .AddSources("#nullable enable\n" + source)
           .Build()
           .GenerateAsync(cancellationToken);

    public AssemblyLoadContext AssemblyLoadContext { get; } = new CollectibleTestAssemblyLoadContext();
    protected GeneratorTestContextBuilder Builder { get; set; } = null!;
}
