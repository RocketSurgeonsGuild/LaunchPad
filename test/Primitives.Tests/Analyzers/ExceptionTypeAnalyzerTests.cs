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

        await VerifyAnalysis(result);
    }

    [Test]
    [Arguments("Sample.Missing")]
    [Arguments("Sample.NotAnException")]
    [Arguments("Sample.NoMessageException")]
    [Arguments("Sample.AbstractException")]
    [Arguments("Sample.Outer+NestedException")]
    [Arguments("Sample.Outer.Missing")]
    [Arguments("Sample..GoodException")]
    public async Task Should_Report_A_Type_Create_Cannot_Throw(string type)
    {
        var result = await Analyze<ExceptionTypeAnalyzer>(Source, builder => builder.AddGlobalOption("build_property.LaunchPadPrimitivesExceptionType", type));

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
    }
}
