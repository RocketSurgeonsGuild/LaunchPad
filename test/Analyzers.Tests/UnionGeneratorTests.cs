using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Primitives;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers;

namespace Analyzers.Tests;

/// <summary>
///     The portable union form (docs/design/union-types.md): a <c>[Union] partial struct</c> with partial case constructors. Each case compiles on
///     C# 14, which has no unions, and on C# 15 (preview), where the compiler gives the generated type full union support.
/// </summary>
public class UnionGeneratorTests() : GeneratorTest()
{
    internal static bool RuntimeHasUnionTypes { get; } = typeof(object).Assembly.GetType("System.Runtime.CompilerServices.IUnion") is { };

    /// <summary>
    ///     On .NET 10 the union types come from the Polyfill package; .NET 11 has them in the BCL. The test hands .NET 10 compilations the types as a
    ///     referenced assembly (Polyfill's <c>PolyPublic</c> shape), so their sources are numbered the same on both target frameworks.
    /// </summary>
    internal static byte[]? PolyfillImage { get; } = RuntimeHasUnionTypes ? null : CompilePolyfill();

    internal static Microsoft.CodeAnalysis.MetadataReference? PolyfillReference { get; } =
        PolyfillImage is { } image ? Microsoft.CodeAnalysis.MetadataReference.CreateFromImage(image) : null;

    private static byte[] CompilePolyfill()
    {
        var compilation = CSharpCompilation.Create(
            "UnionPolyfill",
            [
                CSharpSyntaxTree.ParseText(
                    """
                    #nullable enable
                    namespace System.Runtime.CompilerServices
                    {
                        [global::System.AttributeUsage(global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
                        public sealed class UnionAttribute : global::System.Attribute;
                        public interface IUnion { object? Value { get; } }
                    }
                    """
                ),
            ],
            [Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(typeof(object).Assembly.Location),],
            new(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary)
        );
        using var stream = new MemoryStream();
        compilation.Emit(stream).Success.ShouldBeTrue();
        return stream.ToArray();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<UnionGenerator>()
                 .WithAnalyzer<UnionAnalyzer>()
                 .AddReferences(typeof(Problem));
        if (PolyfillReference is { } polyfill) Builder = Builder.AddReferences(polyfill);
    }

    // The compiled test assembly references the polyfill assembly; load it where the test assembly is loaded.
    private void LoadPolyfill()
    {
        if (PolyfillImage is { } image) AssemblyLoadContext.LoadFromStream(new MemoryStream(image));
    }

    public static IEnumerable<LanguageVersion> LanguageVersions() => [LanguageVersion.CSharp14, LanguageVersion.Preview];

    [Test]
    [MethodDataSource(nameof(LanguageVersions))]
    public async Task Should_Generate_Union(LanguageVersion languageVersion)
    {
        var result = await Builder
                          .WithLanguageVersion(languageVersion)
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;
                               using Rocket.Surgery.LaunchPad.Primitives;

                               namespace Sample.Core.Operations.Rockets;

                               public record RocketModel(string Id);

                               [Union]
                               public readonly partial struct GetRocketResult
                               {
                                   public partial GetRocketResult(RocketModel value);
                                   public partial GetRocketResult(NotFound value);
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(languageVersion);
    }

    [Test]
    [MethodDataSource(nameof(LanguageVersions))]
    public async Task Should_Generate_Generic_Nested_Union(LanguageVersion languageVersion)
    {
        var result = await Builder
                          .WithLanguageVersion(languageVersion)
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;
                               using Rocket.Surgery.LaunchPad.Primitives;

                               namespace Sample.Core;

                               public static partial class Results
                               {
                                   [Union]
                                   public partial struct Result<T>
                                       where T : notnull
                                   {
                                       public partial Result(T value);
                                       public partial Result(Problem problem);
                                   }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(languageVersion);
    }

    [Test]
    [MethodDataSource(nameof(LanguageVersions))]
    public async Task Should_Generate_Union_With_Nullable_Value_Interface_And_Internal_Cases(LanguageVersion languageVersion)
    {
        var result = await Builder
                          .WithLanguageVersion(languageVersion)
                          .AddSources(
                               """
                               using System;
                               using System.Runtime.CompilerServices;

                               [Union]
                               internal partial record struct Reading
                               {
                                   public partial Reading(double? measure);
                                   public partial Reading(string? text);
                                   public partial Reading(IFormattable formattable);
                                   internal partial Reading(bool @switch);
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(languageVersion);
    }

    [Test]
    [MethodDataSource(nameof(LanguageVersions))]
    public async Task Should_Leave_Hand_Written_Unions_Alone(LanguageVersion languageVersion)
    {
        var result = await Builder
                          .WithLanguageVersion(languageVersion)
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;

                               [Union]
                               public readonly struct Quantity : IUnion
                               {
                                   private readonly object? _value;
                                   public Quantity(int count) => _value = count;
                                   public Quantity(double measure) => _value = measure;
                                   public object? Value => _value;
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(languageVersion);
    }

    [Test]
    public async Task Should_Report_Union_That_Is_Not_A_Struct()
    {
        var result = await Builder
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;

                               [Union]
                               public partial class Shape
                               {
                                   public partial Shape(int sides);
                                   public partial Shape(string name);
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(GeneratorDiagnostics(result));
    }

    [Test]
    public async Task Should_Report_Case_Constructor_Without_One_Parameter()
    {
        var result = await Builder
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;

                               [Union]
                               public partial struct Shape
                               {
                                   public partial Shape(int sides);
                                   public partial Shape(string name, int sides);
                                   public partial Shape();
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(GeneratorDiagnostics(result));
    }

    [Test]
    public async Task Should_Report_Containing_Type_That_Is_Not_Partial()
    {
        var result = await Builder
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;

                               public static class Shapes
                               {
                                   [Union]
                                   public partial struct Shape
                                   {
                                       public partial Shape(int sides);
                                       public partial Shape(string name);
                                   }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(GeneratorDiagnostics(result));
    }

    [Test]
    [MethodDataSource(nameof(LanguageVersions))]
    public async Task Should_Behave_Like_A_Union_At_Runtime(LanguageVersion languageVersion)
    {
        var result = await Builder
                          .WithLanguageVersion(languageVersion)
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;
                               using Rocket.Surgery.LaunchPad.Primitives;

                               public record RocketModel(string Id);

                               [Union]
                               public readonly partial struct GetRocketResult
                               {
                                   public partial GetRocketResult(RocketModel value);
                                   public partial GetRocketResult(NotFound value);
                               }

                               public static class Exercise
                               {
                                   private static GetRocketResult Get(string id) => id == "1" ? new RocketModel(id) : new NotFound($"Rocket {id} not found");

                                   // The portable way to read a union: through Value, on any language version.
                                   private static string Describe(GetRocketResult result) => result.Value switch
                                   {
                                       RocketModel rocket => "rocket " + rocket.Id,
                                       NotFound notFound => "not found: " + notFound.Detail,
                                       _ => "empty",
                                   };

                                   public static string Run()
                                   {
                                       var found = Get("1");
                                       var missing = Get("2");
                                       var tried = found.TryGetValue(out RocketModel? rocket) && !found.TryGetValue(out NotFound? _);
                                       return string.Join(
                                           "|",
                                           Describe(found),
                                           Describe(missing),
                                           Describe(default),
                                           tried + ":" + rocket?.Id,
                                           found.HasValue + ":" + default(GetRocketResult).HasValue,
                                           ( ((IUnion)found).Value is RocketModel ).ToString()
                                       );
                                   }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        result.FinalDiagnostics.Where(diagnostic => diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ShouldBeEmpty();
        LoadPolyfill();
        var run = result.Assembly!.GetType("Exercise")!.GetMethod("Run", BindingFlags.Public | BindingFlags.Static)!;
        run.Invoke(null, null).ShouldBe("rocket 1|not found: Rocket 2 not found|empty|True:1|True:False|True");
    }

    [Test]
    public async Task Should_Get_Language_Support_On_CSharp_15()
    {
        var result = await Builder
                          .WithLanguageVersion(LanguageVersion.Preview)
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;
                               using Rocket.Surgery.LaunchPad.Primitives;

                               public record RocketModel(string Id);

                               [Union]
                               public readonly partial struct GetRocketResult
                               {
                                   public partial GetRocketResult(RocketModel value);
                                   public partial GetRocketResult(Problem value);
                               }

                               public static class Exercise
                               {
                                   // Exhaustive without a discard: the compiler knows the cases.
                                   private static string Describe(GetRocketResult result) => result switch
                                   {
                                       RocketModel rocket => "rocket " + rocket.Id,
                                       NotFound notFound => "not found",
                                       Problem problem => "problem " + problem.GetType().Name,
                                   };

                                   public static string Run() => Describe(new RocketModel("1")) + "|" + Describe(new NotFound()) + "|" + Describe(new Conflict());
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        result.FinalDiagnostics.Where(diagnostic => diagnostic.Severity >= Microsoft.CodeAnalysis.DiagnosticSeverity.Warning).ShouldBeEmpty();
        LoadPolyfill();
        var run = result.Assembly!.GetType("Exercise")!.GetMethod("Run", BindingFlags.Public | BindingFlags.Static)!;
        run.Invoke(null, null).ShouldBe("rocket 1|not found|problem Conflict");
    }

    [Test]
    public async Task Should_Report_Missing_Union_Attribute()
    {
        // Only reproducible where the runtime lacks the types: on .NET 11 the BCL always provides them.
        if (RuntimeHasUnionTypes) return;

        var result = await GeneratorTestContextBuilder
                          .Create()
                          .WithLogger(Logger)
                          .WithAssemblyLoadContext(AssemblyLoadContext)
                          .WithGenerator<UnionGenerator>()
                          .WithAnalyzer<UnionAnalyzer>()
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;

                               [Union]
                               public partial struct Shape
                               {
                                   public partial Shape(int sides);
                                   public partial Shape(string name);
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(AnalyzerDiagnostics(result));
    }

    [Test]
    public async Task Should_Rewrite_Union_Patterns_To_Value_On_CSharp_14()
    {
        var result = await Builder
                          .WithLanguageVersion(LanguageVersion.CSharp14)
                          .WithCodeFix<UnionPatternCodeFix>()
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;
                               using Rocket.Surgery.LaunchPad.Primitives;

                               public record RocketModel(string Id);

                               [Union]
                               public readonly partial struct GetRocketResult
                               {
                                   public partial GetRocketResult(RocketModel value);
                                   public partial GetRocketResult(NotFound value);
                               }

                               public static class Describer
                               {
                                   public static string Describe(GetRocketResult result) => result switch
                                   {
                                       RocketModel rocket => rocket.Id,
                                       NotFound notFound => "missing",
                                   };
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Report_And_Fix_Value_Match_On_CSharp_15()
    {
        var result = await Builder
                          .WithLanguageVersion(LanguageVersion.Preview)
                          .WithCodeFix<RedundantValueSwitchCodeFix>()
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;
                               using Rocket.Surgery.LaunchPad.Primitives;

                               public record RocketModel(string Id);

                               [Union]
                               public readonly partial struct GetRocketResult
                               {
                                   public partial GetRocketResult(RocketModel value);
                                   public partial GetRocketResult(Problem value);
                               }

                               public static class Describer
                               {
                                   public static string Describe(GetRocketResult result) => result.Value switch
                                   {
                                       RocketModel rocket => rocket.Id,
                                       NotFound => "missing",
                                       Problem problem => problem.Detail ?? "problem",
                                       _ => throw new System.Diagnostics.UnreachableException(),
                                   };

                                   // Not reported: the discard does more than throw, and string isn't a case.
                                   public static string Keep(GetRocketResult result) => result.Value switch
                                   {
                                       RocketModel rocket => rocket.Id,
                                       _ => "other",
                                   };

                                   public static bool KeepToo(GetRocketResult result) => result.Value is string;
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Report_And_Fix_Value_Type_Test_On_CSharp_15()
    {
        var result = await Builder
                          .WithLanguageVersion(LanguageVersion.Preview)
                          .WithCodeFix<RedundantValueSwitchCodeFix>()
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;
                               using Rocket.Surgery.LaunchPad.Primitives;

                               public record RocketModel(string Id);

                               [Union]
                               public readonly partial struct GetRocketResult
                               {
                                   public partial GetRocketResult(RocketModel value);
                                   public partial GetRocketResult(Problem value);
                               }

                               public static class Describer
                               {
                                   public static bool IsRocket(GetRocketResult result) => result.Value is RocketModel;
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    public static IEnumerable<string> NativeCandidates() =>
    [
        """
        /// <summary>What GetRocket answers.</summary>
        [Union]
        public readonly partial struct GetRocketResult
        {
            public partial GetRocketResult(RocketModel value);
            public partial GetRocketResult(NotFound value);
        }
        """,
        """
        [Union]
        internal partial struct Result<T>
            where T : notnull
        {
            public partial Result(T value);
            public partial Result(Problem problem);
        }
        """,
    ];

    [Test]
    [MethodDataSource(nameof(NativeCandidates))]
    public async Task Should_Report_And_Fix_Portable_Union_That_Can_Be_Native(string declaration)
    {
        var result = await Builder
                          .WithLanguageVersion(LanguageVersion.Preview)
                          .WithCodeFix<NativeUnionCodeFix>()
                          .AddSources(
                               $$"""
                               using System.Runtime.CompilerServices;
                               using Rocket.Surgery.LaunchPad.Primitives;

                               namespace Sample.Core;

                               public record RocketModel(string Id);

                               {{declaration}}

                               // Not reported: it has a member of its own.
                               [Union]
                               public partial struct WithMember
                               {
                                   public partial WithMember(int value);
                                   public partial WithMember(string value);
                                   public bool IsNumber => Value is int;
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(declaration.Contains("Result<T>", StringComparison.Ordinal) ? "generic" : "simple");
    }

    [Test]
    public async Task Should_Not_Report_CSharp_15_Rules_On_CSharp_14()
    {
        var result = await Builder
                          .WithLanguageVersion(LanguageVersion.CSharp14)
                          .AddSources(
                               """
                               using System.Runtime.CompilerServices;

                               [Union]
                               public partial struct Shape
                               {
                                   public partial Shape(int sides);
                                   public partial Shape(string name);
                               }

                               public static class Describer
                               {
                                   public static string Describe(Shape shape) => shape.Value switch
                                   {
                                       int sides => sides.ToString(),
                                       string name => name,
                                       _ => throw new System.Diagnostics.UnreachableException(),
                                   };
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(AnalyzerDiagnostics(result));
    }

    private static IEnumerable<string> AnalyzerDiagnostics(GeneratorTestResults result) =>
        result.TryGetAnalyzerResult<UnionAnalyzer>(out var output)
            ? output.Diagnostics.OrderBy(diagnostic => diagnostic.Location.SourceSpan.Start).Select(diagnostic => diagnostic.ToString())
            : [];

    private static IEnumerable<string> GeneratorDiagnostics(GeneratorTestResults result) =>
        result.TryGetResult<UnionGenerator>(out var output)
            ? output.Diagnostics.OrderBy(diagnostic => diagnostic.Location.SourceSpan.Start).Select(diagnostic => diagnostic.ToString())
            : [];
}
