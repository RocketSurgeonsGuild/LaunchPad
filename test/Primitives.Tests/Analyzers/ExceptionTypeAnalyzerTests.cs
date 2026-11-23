using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>LPAD2017: the exception LaunchPadPrimitivesExceptionType names.</summary>
public class ExceptionTypeAnalyzerTests
{
    private const string Source = """
                                  using Rocket.Surgery.LaunchPad.Primitives;

                                  namespace Sample;

                                  [StringPrimitive, NotEmpty]
                                  public readonly partial record struct UserName;

                                  public class GoodException(string message) : System.Exception(message);

                                  public class DerivedException(string message, int code) : GoodException(message)
                                  {
                                      public DerivedException(string message) : this(message, 0) { }
                                      public int Code { get; } = code;
                                  }

                                  public class InternalException : System.Exception
                                  {
                                      internal InternalException(string message) : base(message) { }
                                  }

                                  public class NotAnException(string message);

                                  public class NoMessageException : System.Exception
                                  {
                                      public NoMessageException(int code) { }
                                  }

                                  public abstract class AbstractException(string message) : System.Exception(message);

                                  public static class Outer
                                  {
                                      public class NestedException(string message) : System.Exception(message);
                                  }
                                  """;

    [Test]
    [Arguments("Sample.GoodException")]
    [Arguments("global::Sample.GoodException")]
    [Arguments("Sample.DerivedException")]
    [Arguments("Sample.InternalException")]
    [Arguments("System.ArgumentException")]
    // The generator writes new global::<name>(message): a nested type is named as C# names it.
    [Arguments("Sample.Outer.NestedException")]
    [Arguments("")]
    public async Task Should_Accept_An_Exception_That_Takes_The_Message(string type)
    {
        var result = await Analyze<ExceptionTypeAnalyzer>(Source, builder => builder.AddGlobalOption("build_property.LaunchPadPrimitivesExceptionType", type));

        await Diagnostics(result).Should().BeEmpty();
    }

    [Test]
    [Arguments("Sample.Missing", "which the project cannot find")]
    [Arguments("Sample.NotAnException", "which does not derive from System.Exception")]
    [Arguments("Sample.NoMessageException", "which has no accessible constructor that takes the message (a single string)")]
    [Arguments("Sample.AbstractException", "which is abstract, so it cannot be thrown")]
    [Arguments(
        "Sample.Outer+NestedException",
        "which is not a C# name of a type: write a nested type as Outer.Inner (the generator writes new global::<name>(message)), and a generic type can't be used"
    )]
    [Arguments("Sample.Outer.Missing", "which the project cannot find")]
    [Arguments("Sample..GoodException", "which the project cannot find")]
    public async Task Should_Report_A_Type_Create_Cannot_Throw(string type, string problem)
    {
        var result = await Analyze<ExceptionTypeAnalyzer>(Source, builder => builder.AddGlobalOption("build_property.LaunchPadPrimitivesExceptionType", type));

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2017"], CollectionOrdering.Matching);
        await Messages(result, "LPAD2017").Should().BeEquivalentTo([$"LaunchPadPrimitivesExceptionType is '{type}', {problem}"], CollectionOrdering.Matching);
    }

    // Two references that both define the name make it ambiguous (CS0433), which is not the same as missing.
    [Test]
    public async Task Should_Report_A_Name_Two_References_Define()
    {
        var result = await Analyze<ExceptionTypeAnalyzer>(
            Source,
            builder => builder.AddReferences(Exceptions("First"), Exceptions("Second"))
                              .AddGlobalOption("build_property.LaunchPadPrimitivesExceptionType", "Shared.DuplicateException")
        );

        await Messages(result, "LPAD2017")
             .Should()
             .BeEquivalentTo(
                  ["LaunchPadPrimitivesExceptionType is 'Shared.DuplicateException', which is defined in more than one referenced assembly (First, Second), so the name is ambiguous"],
                  CollectionOrdering.Matching
              );
    }

    // The project's own type wins over a reference's (CS0436 is a warning); one reference alone is no ambiguity.
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Should_Accept_A_Name_That_Binds_To_One_Type(bool declaredByTheProject)
    {
        var source = declaredByTheProject
            ? Source + "\nnamespace Shared { public class DuplicateException(string message) : System.Exception(message); }"
            : Source;
        var result = await Analyze<ExceptionTypeAnalyzer>(
            source,
            builder => builder.AddReferences(Exceptions("First")).AddGlobalOption("build_property.LaunchPadPrimitivesExceptionType", "Shared.DuplicateException")
        );

        await Diagnostics(result).Should().BeEmpty();
    }

    /// <summary>An assembly named <paramref name="assemblyName" /> that declares <c>Shared.DuplicateException</c>.</summary>
    private static MetadataReference Exceptions(string assemblyName)
    {
        var compilation = CSharpCompilation.Create(
            assemblyName,
            [CSharpSyntaxTree.ParseText("namespace Shared { public class DuplicateException : System.Exception { public DuplicateException(string message) : base(message) { } } }")],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        using var image = new MemoryStream();
        var emitted = compilation.Emit(image);
        return !emitted.Success
            ? throw new InvalidOperationException(string.Join("\n", emitted.Diagnostics.Select(d => d.ToString())))
            : MetadataReference.CreateFromImage(image.ToArray());
    }

    // The setting can come from a Directory.Build.props shared by projects that declare no domain primitive.
    [Test]
    public async Task Should_Ignore_A_Project_Without_Domain_Primitives()
    {
        var result = await Analyze<ExceptionTypeAnalyzer>(
            "namespace Sample; public class Plain;",
            builder => builder.AddGlobalOption("build_property.LaunchPadPrimitivesExceptionType", "Sample.Missing")
        );

        await Diagnostics(result).Should().BeEmpty();
    }
}
