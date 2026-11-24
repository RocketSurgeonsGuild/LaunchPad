using Microsoft.CodeAnalysis;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Primitives;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Registry;

namespace Primitives.Tests.Generator;

/// <summary>
///     The compilations the generator tests run the domain primitive generator on: an application that references Primitives, and with
///     <c>nodaTime</c> NodaTime too, with the preprocessor symbol Primitives' targets define for it (the NodaTime members of DatePrimitiveKind).
/// </summary>
internal static class GeneratorTestBuilder
{
    /// <summary>
    ///     The attributes the generator writes into every project: <see cref="DomainPrimitiveGeneratorTests" /> has the one snapshot of them,
    ///     every other snapshot leaves them out.
    /// </summary>
    public const string AttributesFile = "PrimitiveAttributes.g.cs";

    public const string EmbeddedAttributeFile = "Microsoft.CodeAnalysis.EmbeddedAttribute.cs";

    public static GeneratorTestContextBuilder Create(bool nodaTime = false, bool withAttributes = false, bool withRegistry = false)
    {
        var builder = GeneratorTestContextBuilder
                     .Create()
                     .WithProjectName("Application");
        builder = withRegistry ? builder.WithGenerator<PrimitiveGenerators>() : builder.WithGenerator<DomainPrimitiveGenerator>();
        builder = builder
                     .AddReferences(typeof(PrimitiveError).Assembly)
                     .AddReferences(Framework);
        if (!withAttributes) builder = builder.IgnoreOutputFile(AttributesFile).IgnoreOutputFile(EmbeddedAttributeFile);
        if (nodaTime) builder = builder.AddReferences(typeof(NodaTime.Instant).Assembly).AddPreprocessorSymbol("LAUNCHPAD_PRIMITIVES_NODATIME");
        return builder;
    }

    /// <summary>
    ///     The assemblies of the shared framework, as an application compiles against them: the generated code uses System.Text.Json,
    ///     System.ComponentModel, regular expressions, frozen collections and so on.
    /// </summary>
    public static IReadOnlyCollection<string> Framework { get; } =
    [
        .. ( (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")! )
          .Split(Path.PathSeparator)
          .Where(static z => Path.GetFileName(z) is { } name && ( name.StartsWith("System.", StringComparison.Ordinal) || name is "netstandard.dll" or "mscorlib.dll" ))
          .Where(static z => Path.GetDirectoryName(z) == Path.GetDirectoryName(typeof(object).Assembly.Location)),
    ];

    /// <summary>
    ///     Runs the generator on <paramref name="source" />; with <paramref name="regex" /> the regular expression generator too, which implements
    ///     the <c>[GeneratedRegex]</c> members of the source (its output is left out of the snapshot).
    /// </summary>
    public static Task<GeneratorTestResults> Generate(string source, bool nodaTime = false, bool regex = false)
    {
        var builder = Create(nodaTime);
        if (regex) builder = builder.WithGenerator(RegexGenerator.Type).IgnoreOutputFile("RegexGenerator.g.cs");
        return builder.AddSources("#nullable enable\n" + source).Build().GenerateAsync(TestContext.Current!.Execution.CancellationToken);
    }

    /// <summary>
    ///     Checks that what the generator wrote compiles without an error or a warning: the snapshot shows the errors only.
    /// </summary>
    public static async Task ShouldCompileCleanly(this GeneratorTestResults result)
    {
        var problems = result.FinalCompilation
                             .GetDiagnostics()
                             .Where(static z => z.Severity >= DiagnosticSeverity.Warning)
                             // assembly version unification, which the .NET SDK leaves out of every build too (NoWarn)
                             .Where(static z => z.Id is not ( "CS1701" or "CS1702" ))
                             .Select(static z => z.ToString())
                             .ToArray();
        await Assert.That(problems).IsEmpty();
    }
}

/// <summary>
///     The domain primitive generator and the registry generator in one driver, as a project runs them: the test harness runs each generator it
///     is given on its own, and the registry finds the project's own domain primitives by the attributes the other generator writes.
/// </summary>
internal sealed class PrimitiveGenerators : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        new DomainPrimitiveGenerator().Initialize(context);
        new PrimitiveRegistryGenerator().Initialize(context);
    }
}
