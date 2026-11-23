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
                                  """;

    [Test]
    [Arguments("Sample.GoodException")]
    [Arguments("global::Sample.GoodException")]
    [Arguments("Sample.DerivedException")]
    [Arguments("Sample.InternalException")]
    [Arguments("System.ArgumentException")]
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
    public async Task Should_Report_A_Type_Create_Cannot_Throw(string type, string problem)
    {
        var result = await Analyze<ExceptionTypeAnalyzer>(Source, builder => builder.AddGlobalOption("build_property.LaunchPadPrimitivesExceptionType", type));

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2017"], CollectionOrdering.Matching);
        await Messages(result, "LPAD2017").Should().BeEquivalentTo([$"LaunchPadPrimitivesExceptionType is '{type}', {problem}"], CollectionOrdering.Matching);
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
