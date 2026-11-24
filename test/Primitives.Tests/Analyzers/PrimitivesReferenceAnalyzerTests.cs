using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Analyzers.Primitives;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>
///     LPAD2024, which LaunchPad's analyzer reports in an application that uses the domain primitives of its references with OpenAPI,
///     HotChocolate, EF Core or Dapper but doesn't reference Primitives itself, so it gets none of the Primitives Joints.
/// </summary>
public class PrimitivesReferenceAnalyzerTests
{
    // What the application compiles: it only uses the contracts.
    private const string Shop = """
                                       namespace Sample;

                                       public static class Shop
                                       {
                                           public static string Describe(Contracts.Customer customer) => customer.Email.Value;
                                       }
                                       """;

    [Test]
    public async Task Should_Report_An_Application_That_Uses_Domain_Primitives_Without_Primitives()
    {
        var contracts = await Contracts();

        var result = await Analyze<PrimitivesReferenceAnalyzer>(Shop, builder => AsApplication(builder, contracts).AddReferences(Libraries));

        await VerifyAnalysis(result);
    }

    // Each library a Primitives Joint describes domain primitives to is enough, and only those the application references are named.
    [Test]
    public async Task Should_Name_The_Libraries_The_Application_References()
    {
        var contracts = await Contracts();

        var result = await Analyze<PrimitivesReferenceAnalyzer>(
            Shop,
            builder => AsApplication(builder, contracts).AddReferences(typeof(Dapper.SqlMapper).Assembly)
        );

        await VerifyAnalysis(result);
    }

    // The registry template is written, with the Joints, into an application that references Primitives.
    [Test]
    public async Task Should_Accept_An_Application_With_The_Primitives_Templates()
    {
        var contracts = await Contracts();

        var result = await Analyze<PrimitivesReferenceAnalyzer>(
            Shop,
            builder => AsApplication(builder, contracts)
                      .AddReferences(Libraries)
                      .AddSource(
                           "LaunchPadPrimitives.cs",
                           """
                           namespace Rocket.Surgery.LaunchPad.Primitives.Generated;

                           internal static partial class LaunchPadPrimitives;
                           """
                       )
        );

        await VerifyAnalysis(result);
    }

    // A library gets no templates: the application that references it is checked instead. A test project isn't an application to describe.
    [Test]
    [Arguments("build_property.OutputType", "Library")]
    [Arguments("build_property.IsTestProject", "true")]
    public async Task Should_Accept_A_Library_Or_A_Test_Project(string property, string value)
    {
        var contracts = await Contracts();

        var result = await Analyze<PrimitivesReferenceAnalyzer>(
            Shop,
            builder => AsApplication(builder, contracts).AddReferences(Libraries).AddGlobalOption(property, value)
        );

        await VerifyAnalysis(result);
    }

    // Nothing to describe: none of the libraries, or no domain primitive the application can use.
    [Test]
    public async Task Should_Accept_An_Application_Without_A_Library_To_Describe_Them_To()
    {
        var contracts = await Contracts();

        var result = await Analyze<PrimitivesReferenceAnalyzer>(Shop, builder => AsApplication(builder, contracts));

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Accept_References_Without_A_Public_Domain_Primitive()
    {
        var contracts = await CompileWithGenerator(
            "Contracts",
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Contracts;

            [StringPrimitive, NotEmpty]
            internal readonly partial record struct Sku;

            public sealed class Customer
            {
                public required string Email { get; init; }
            }
            """
        );

        var result = await Analyze<PrimitivesReferenceAnalyzer>(
            """
            namespace Sample;

            public static class Shop
            {
                public static string Describe(Contracts.Customer customer) => customer.Email;
            }
            """,
            builder => AsApplication(builder, contracts).AddReferences(Libraries)
        );

        await VerifyAnalysis(result);
    }

    // The libraries the Primitives Joints (and the registry generator, for Dapper) describe domain primitives to.
    private static readonly System.Reflection.Assembly[] Libraries =
    [
        typeof(Dapper.SqlMapper).Assembly,
        typeof(Microsoft.EntityFrameworkCore.DbContext).Assembly,
    ];

    // An application, as the build tells the compiler (Foundation's targets make OutputType visible), named Shop, that references the contracts.
    private static GeneratorTestContextBuilder AsApplication(GeneratorTestContextBuilder builder, GeneratorTestResults contracts) =>
        builder.WithProjectName("Shop").AddGlobalOption("build_property.OutputType", "Exe").AddCompilationReferences(contracts);

    // A library of domain primitives, which references Primitives.
    private static Task<GeneratorTestResults> Contracts() =>
        CompileWithGenerator(
            "Contracts",
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Contracts;

            [Email]
            public readonly partial record struct CustomerEmail;

            public sealed class Customer
            {
                public required CustomerEmail Email { get; init; }
            }
            """
        );
}
