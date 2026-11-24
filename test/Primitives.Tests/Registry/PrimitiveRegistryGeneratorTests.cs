using Microsoft.Extensions.DependencyInjection;

using Primitives.Tests.Generator;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Primitives.Tests.Registry;

/// <summary>
///     The registry generator: <c>LaunchPadPrimitives</c>, which describes every domain primitive an application declares or references for the
///     Joints Primitives writes into it as templates, and the Dapper type handlers and OpenTelemetry extensions it writes when the application
///     references those.
/// </summary>
public class PrimitiveRegistryGeneratorTests
{
    // What Primitives' registry template declares: the class the generator completes.
    private const string Registry = """
        namespace Rocket.Surgery.LaunchPad.Primitives.Generated
        {
            internal static partial class LaunchPadPrimitives;
        }
        """;

    // The application's own domain primitives: found by their attribute, since the domain primitive generator completes them in the same run.
    private const string Application = """
        using Rocket.Surgery.LaunchPad.Primitives;

        namespace Sample.Application
        {
            [Email]
            public readonly partial record struct EmailAddress;

            [StrongId<System.Guid>]
            public sealed partial record OrderId;

            [Money]
            public readonly partial record struct Price;

            [SmartEnum<string>]
            public closed partial class Channel
            {
                public static readonly Channel Web = new Web_("web");
                public static readonly Channel Store = new Store_("store");
            }

            // A smart enum can be a record struct too.
            [SmartEnum<int>]
            public readonly partial record struct Speed
            {
                public static readonly Speed Slow = new(1);
                public static readonly Speed Fast = new(2);
            }

            // Not public: the application's registry can name it, so it is listed too.
            [StringPrimitive, MaxLength(10)]
            internal readonly partial record struct Code;
        }
        """;

    // The hint names of what the domain primitive generator writes for Application, which the snapshots of the registry leave out.
    private static readonly string[] ApplicationFiles =
    [
        "Sample.Application.EmailAddress.g.cs", "Sample.Application.OrderId.g.cs", "Sample.Application.Price.g.cs", "Sample.Application.Code.g.cs",
        "Sample.Application.Channel.g.cs", "Sample.Application.Speed.g.cs",
    ];

    private static GeneratorTestContextBuilder Builder(bool nodaTime = false)
    {
        var builder = GeneratorTestBuilder.Create(nodaTime, withRegistry: true);
        foreach (var file in ApplicationFiles)
        {
            builder = builder.IgnoreOutputFile(file);
        }

        return builder;
    }

    /// <summary>
    ///     A contracts library that declares domain primitives of its own, compiled with the domain primitive generator: the registry finds them
    ///     in its metadata by the interfaces the generator implemented.
    /// </summary>
    private static Task<GeneratorTestResults> CompileReferencedAssembly() =>
        GeneratorTestBuilder
           .Create(nodaTime: true)
           .WithProjectName("Sample.Core")
           .AddSources(
                """
                #nullable enable
                using Rocket.Surgery.LaunchPad.Primitives;

                namespace Sample.Core
                {
                    [StrongId<int>]
                    public readonly partial record struct CustomerId;

                    [SmartEnum<int>]
                    public closed partial class Status
                    {
                        public static readonly Status Active = new Active_(1);
                        public static readonly Status Closed = new Closed_(2);
                    }

                    [SmartEnum<string>]
                    public readonly partial record struct Tier
                    {
                        public static readonly Tier Free = new("free");
                        public static readonly Tier Paid = new("paid");
                    }

                    [DateRange(Kind = DatePrimitiveKind.LocalDate)]
                    public readonly partial record struct Period;

                    [Rating]
                    public readonly partial record struct Stars;

                    public static partial class Nested
                    {
                        [Slug]
                        public readonly partial record struct Handle;
                    }

                    // Not visible to the application: left out.
                    [Slug]
                    internal readonly partial record struct Hidden;
                }
                """
            )
           .Build()
           .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

    [Test]
    public async Task Should_Write_Nothing_Without_The_Registry_Declaration()
    {
        // A library that references Primitives gets no registry: only an application with Primitives' templates declares it.
        var result = await Builder().AddSources(Application).Build().GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Describe_Every_Domain_Primitive_The_Application_Declares_Or_References()
    {
        var core = await CompileReferencedAssembly();
        await core.ShouldCompileCleanly();

        var result = await Builder(nodaTime: true)
                          .AddCompilationReferences(core)
                          .AddSources(Registry, Application)
                          .Build()
                          .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Write_The_Representations_The_Application_Chose()
    {
        var result = await Builder()
                          .AddGlobalOption("build_property.LaunchPadPrimitivesOpenApiRepresentation", "Inline")
                          .AddGlobalOption("build_property.LaunchPadPrimitivesGraphQLRepresentation", "Named")
                          .AddSources(Registry)
                          .Build()
                          .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Visit_Every_DbContext_And_Converter_When_The_Template_Asks()
    {
        // The registry and EF Core templates as Primitives ships them, which declare ForEachDbContext and ForEachConverter: the registry
        // generator writes them and a value converter per domain primitive, and the whole compiles against EF Core.
        var result = await Builder()
                          .AddReferences(
                               typeof(Microsoft.EntityFrameworkCore.DbContext).Assembly,
                               typeof(IServiceCollection).Assembly,
                               typeof(Joinery.Dovetail.IDovetailContext).Assembly
                           )
                          .AddSources(
                               Templates.Read("LaunchPadPrimitives"),
                               Templates.Read("LaunchPadPrimitivesEntityFrameworkJoint"),
                               Application,
                               """
                               using Microsoft.EntityFrameworkCore;

                               namespace Sample.Data
                               {
                                   public class RocketContext(DbContextOptions options) : DbContext(options);
                                   public sealed class LaunchContext(DbContextOptions options) : RocketContext(options);

                                   // Not visited: abstract, or generic.
                                   public abstract class ContextBase(DbContextOptions options) : DbContext(options);
                                   public class GenericContext<T>(DbContextOptions options) : DbContext(options);
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Register_A_Dapper_Type_Handler_For_Every_Domain_Primitive()
    {
        var core = await CompileReferencedAssembly();
        await core.ShouldCompileCleanly();

        // Without the registry declaration: an application that references Dapper gets the type handlers either way. A closed smart enum's
        // handler is registered for the class of each of its members too (Channel.Web_, and Sample.Core's Status.Active_ from its metadata), as
        // Dapper looks a handler up by a value's runtime type when a parameter's type isn't known.
        var result = await Builder(nodaTime: true)
                          .AddCompilationReferences(core)
                          .AddReferences(typeof(Dapper.SqlMapper).Assembly)
                          .AddSources(Application)
                          .Build()
                          .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Write_The_OpenTelemetry_Extensions()
    {
        var result = await Builder()
                          .AddReferences(typeof(OpenTelemetry.Metrics.MeterProviderBuilder).Assembly, typeof(OpenTelemetry.Sdk).Assembly)
                          .AddSources(Application)
                          .Build()
                          .GenerateAsync(TestContext.Current!.Execution.CancellationToken);

        await result.ShouldCompileCleanly();
        await Verify(result);
    }
}
