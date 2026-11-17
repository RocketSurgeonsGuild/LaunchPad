using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers;
using Rocket.Surgery.LaunchPad.StrongTypes;

using SuperStrong.Types;

namespace Primitives.Tests;

/// <summary>
///     The strong type registry: <c>LaunchPadStrongTypes.All</c> describes every strong type the application declares or references, with the
///     representations it chose and, for the EF Core template, a visit per DbContext.
/// </summary>
public class StrongTypeRegistryGeneratorTests
{
    // What Primitives' templates declare: the registry class the generator completes.
    private const string Registry = """
        namespace Rocket.Surgery.LaunchPad.StrongTypes.Generated
        {
            internal static partial class LaunchPadStrongTypes;
        }
        """;

    private static GeneratorTestContextBuilder Builder(params System.Reflection.Assembly[] references) =>
        GeneratorTestContextBuilder
           .Create()
           .WithProjectName("Application")
           .WithGenerator<StrongTypeRegistryGenerator>()
           .AddReferences(typeof(IStrongType<,>).Assembly, typeof(StrongTypeDescription).Assembly)
           .AddReferences(references);

    [Test]
    public async Task Should_Describe_Every_Strong_Type_The_Application_References()
    {
        // Primitives' ready-made strong types, and this assembly's: a number, a format from the rules and an internal one left out.
        var result = await Builder(typeof(Score).Assembly)
                          .AddSources(Registry)
                          .Build()
                          .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Write_The_Representations_The_Application_Chose()
    {
        var result = await Builder()
                          .AddGlobalOption("build_property.LaunchPadStrongTypeOpenApiRepresentation", "Inline")
                          .AddGlobalOption("build_property.LaunchPadStrongTypeGraphQLRepresentation", "Named")
                          .AddSources(Registry)
                          .Build()
                          .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Visit_Every_DbContext_When_The_Template_Asks()
    {
        // EF Core's DbContext declared here: the generator knows it by metadata name, and finds the application's contexts in its source.
        var result = await Builder()
                          .AddSources(
                               """
                               namespace Microsoft.EntityFrameworkCore
                               {
                                   public class DbContext { }
                               }

                               namespace Rocket.Surgery.LaunchPad.StrongTypes.Generated
                               {
                                   internal static partial class LaunchPadStrongTypes
                                   {
                                       static partial void ForEachDbContext(Visitor visitor);
                                   }

                                   internal sealed class Visitor
                                   {
                                       public void Visit<TContext>() where TContext : Microsoft.EntityFrameworkCore.DbContext { }
                                   }
                               }

                               namespace Sample.Data
                               {
                                   public class RocketContext : Microsoft.EntityFrameworkCore.DbContext { }
                                   public sealed class LaunchContext : RocketContext { }

                                   // Not visited: abstract, or generic.
                                   public abstract class ContextBase : Microsoft.EntityFrameworkCore.DbContext { }
                                   public class GenericContext<T> : Microsoft.EntityFrameworkCore.DbContext { }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Write_Nothing_Without_The_Registry_Declaration()
    {
        // A library that references Primitives gets no registry: only an application with Primitives' templates declares it.
        var result = await Builder(typeof(Score).Assembly)
                          .AddSources("namespace Sample { public class Library; }")
                          .Build()
                          .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await Verify(result);
    }
}
