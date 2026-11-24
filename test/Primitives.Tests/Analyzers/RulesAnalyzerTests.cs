using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>
///     LPAD2004 (regex), LPAD2005 (casing, with its fix), LPAD2006 (bounds), LPAD2020 (PastOnly and FutureOnly) and LPAD2022 (rule and kind).
/// </summary>
public class RulesAnalyzerTests
{
    private const string Usings = """
                                  using Rocket.Surgery.LaunchPad.Primitives;

                                  namespace Sample;


                                  """;

    [Test]
    public async Task Should_Report_An_Invalid_Regular_Expression()
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + """
            [StringPrimitive, Regex("[a-z")]
            public readonly partial record struct Code;
            """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Accept_Valid_Regular_Expressions()
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + """
            [StringPrimitive, Regex("^[A-Z]{2}[0-9]+$", ErrorCode = "CODE"), Regex(@"^\S+$", ErrorMessage = "No spaces.")]
            public readonly partial record struct Code;
            """
        );

        await VerifyAnalysis(result);
    }

    // LPAD2023: a [Regex] that reports the default error can be a [GeneratedRegex]; one with its own error or an invalid pattern can't, nor
    // one before a [Regex] with its own error, which a [GeneratedRegex], checked after every [Regex], would come after.
    [Test]
    public async Task Should_Suggest_A_GeneratedRegex()
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + """
            [StringPrimitive, Regex("^A", ErrorCode = "A"), Regex("^[A-Z]{3}$"), Regex("[a-z")]
            public readonly partial record struct Code;

            [StrongId<string>, Regex("^ID-[0-9]+$")]
            public sealed partial record Id;

            [StringPrimitive, Regex("^B"), Regex("^C", ErrorMessage = "C.")]
            public readonly partial record struct Before;
            """
        );

        await VerifyAnalysis(result);
    }

    // The braces go after the base list, the property isn't named like the type, and a comment after the [Regex] follows the pattern.
    [Test]
    public async Task Should_Write_The_GeneratedRegex_Around_The_Declaration()
    {
        var result = await Fix<RulesAnalyzer, UseGeneratedRegexCodeFix>(
            Usings
          + """
            public interface IMarker;

            [StringPrimitive]
            [Regex("^[a-z]+$")] // lower case letters
            public readonly partial record struct Format : IMarker;
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<UseGeneratedRegexCodeFix>(result, "Use a [GeneratedRegex]") ).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Keep_The_Line_Endings_Of_The_Document()
    {
        var result = await Fix<RulesAnalyzer, UseGeneratedRegexCodeFix>(
            ( Usings
            + """
              [StringPrimitive, Regex("^[A-Z]{3}$")]
              public readonly partial record struct Code;

              """ ).ReplaceLineEndings("\r\n")
        );

        await VerifyAnalysis(result);
        // Verify normalizes line endings, so the snapshot can't show them.
        var fixedSource = await Fixed<UseGeneratedRegexCodeFix>(result, "Use a [GeneratedRegex]");
        await fixedSource.Replace("\r\n", "", StringComparison.Ordinal).Should().NotContain("\n");
    }

    [Test]
    public async Task Should_Replace_A_Regex_With_A_GeneratedRegex_Property()
    {
        var result = await Fix<RulesAnalyzer, UseGeneratedRegexCodeFix>(
            Usings
          + """
            /// <summary>A code.</summary>
            [StringPrimitive, Trim, Regex(@"^[A-Z]{3}$")]
            public readonly partial record struct Code;
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<UseGeneratedRegexCodeFix>(result, "Use a [GeneratedRegex]") ).Should().BeEmpty();
    }

    // The attribute alone in its list goes with the list, and a name the type uses already is not taken.
    [Test]
    public async Task Should_Add_The_GeneratedRegex_To_An_Existing_Body()
    {
        var result = await Fix<RulesAnalyzer, UseGeneratedRegexCodeFix>(
            """
            using System.Text.RegularExpressions;
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive]
            [Regex("^[0-9]+$")]
            public sealed partial record Digits
            {
                public static string Format => "digits";
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<UseGeneratedRegexCodeFix>(result, "Use a [GeneratedRegex]") ).Should().BeEmpty();
    }

    // LPAD2009: a [GeneratedRegex] member is a rule of a string primitive.
    [Test]
    public async Task Should_Count_A_GeneratedRegex_Member_As_A_Rule()
    {
        var result = await Analyze<RulesAnalyzer>(
            """
            using System.Text.RegularExpressions;
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive]
            public readonly partial record struct Code
            {
                [GeneratedRegex("^[A-Z]{3}$")]
                private static partial Regex Format { get; }
            }

            [StrongId<string>]
            public readonly partial record struct CodeId
            {
                [GeneratedRegex("^[A-Z]{3}$")]
                private static partial Regex Format();
            }
            """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Report_Both_Lower_And_Upper_Case()
    {
        var result = await Fix<RulesAnalyzer, RemoveNormalizationCodeFix>(
            Usings
          + """
            [StringPrimitive, NotEmpty, LowerCase, UpperCase]
            public readonly partial record struct Code;

            [StringPrimitive, NotEmpty, LowerCase, Trim]
            public readonly partial record struct Lower;
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, "Remove [LowerCase]") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, "Remove [UpperCase]") ).Should().BeEmpty();
    }

    // An attribute alone in its list goes with its list; the first list leaves its comments to the next one.
    [Test]
    public async Task Should_Remove_A_Normalization_Alone_In_Its_List()
    {
        var result = await Fix<RulesAnalyzer, RemoveNormalizationCodeFix>(
            Usings
          + """
            /// <summary>A code.</summary>
            [UpperCase]
            [StringPrimitive, NotEmpty]
            [LowerCase]
            public readonly partial record struct Code;
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, "Remove [LowerCase]") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, "Remove [UpperCase]") ).Should().BeEmpty();
    }

    // The two can be on different declarations of the type: each is removed where it is written.
    [Test]
    public async Task Should_Remove_A_Normalization_From_Another_Declaration()
    {
        var result = await Fix<RulesAnalyzer, RemoveNormalizationCodeFix>(
            Usings
          + """
            [StringPrimitive, NotEmpty, LowerCase]
            public readonly partial record struct Code;
            """,
            builder => builder.AddSource(
                "Code.Casing.cs",
                """
                namespace Sample;

                [Rocket.Surgery.LaunchPad.Primitives.UpperCase]
                public readonly partial record struct Code;
                """
            )
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, "Remove [LowerCase]") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, "Remove [UpperCase]") ).Should().BeEmpty();
    }

    [Test]
    [Arguments("[StringPrimitive, MinLength(10), MaxLength(5)]")]
    [Arguments("[StringPrimitive, MaxLength(5), MinLength(10)]")]
    [Arguments("[StringPrimitive, Length(10, 5)]")]
    [Arguments("[NumericPrimitive<int>, PrimitiveRange(10, 5)]")]
    [Arguments("[NumericPrimitive<double>, PrimitiveRange(1.5, 0.5)]")]
    [Arguments("[DatePrimitive, PrimitiveRange(\"2030-01-01\", \"2020-01-01\")]")]
    [Arguments("[DatePrimitive(Kind = DatePrimitiveKind.DateTimeOffset), PrimitiveRange(\"2030-01-01T00:00:00Z\", \"2020-01-01T00:00:00Z\")]")]
    [Arguments("[DatePrimitive(Kind = DatePrimitiveKind.TimeOnly), PrimitiveRange(\"17:00\", \"09:00\")]")]
    [Arguments("[Percentage(Min = 50, Max = 10)]")]
    [Arguments("[Username(MinLength = 20, MaxLength = 3)]")]
    [Arguments("[FiscalYear(MinYear = 2100, MaxYear = 2000)]")]
    [Arguments("[Money(Min = 10, Max = 5)]")]
    // A bound of the kind against the other bound's default, or against a rule.
    [Arguments("[Age(Min = 200)]")]
    [Arguments("[Percentage(Max = -1)]")]
    [Arguments("[Temperature(Max = -300)]")]
    [Arguments("[FiscalYear(MinYear = 10000)]")]
    [Arguments("[Username(MinLength = 60)]")]
    [Arguments("[Username(MaxLength = 2)]")]
    [Arguments("[Username(MaxLength = 4), MinLength(5)]")]
    [Arguments("[Email(MaxLength = 3), MinLength(5)]")]
    [Arguments("[CountryCode, MinLength(3)]")]
    // A minimum length against the project's maximum length, 4096 by default, where neither a rule nor the kind sets one.
    [Arguments("[StringPrimitive, MinLength(5000)]")]
    [Arguments("[StrongId<string>, MinLength(5000)]")]
    public async Task Should_Report_A_Minimum_Greater_Than_Its_Maximum(string attributes)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              {{attributes}}
              public readonly partial record struct Bounded;
              """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    [Arguments("[StringPrimitive, MinLength(5), MaxLength(5)]")]
    [Arguments("[StringPrimitive, Length(5, 5)]")]
    [Arguments("[NumericPrimitive<int>, PrimitiveRange(5, 5)]")]
    [Arguments("[DatePrimitive, PrimitiveRange(\"2020-01-01\", \"2030-01-01\")]")]
    [Arguments("[Percentage(Min = 10, Max = 50)]")]
    [Arguments("[Percentage(Min = 10)]")]
    [Arguments("[Age(Max = 120)]")]
    [Arguments("[Week(IsoWeekNumbering = false)]")]
    [Arguments("[Distance(Min = 1000000)]")]
    // The generator ignores the kind's bounds where a rule sets them.
    [Arguments("[Age(Min = 200), PrimitiveRange(0, 300)]")]
    [Arguments("[Percentage(Min = 50, Max = 10), PrimitiveRange(0, 100)]")]
    [Arguments("[Username(MinLength = 60), MaxLength(100)]")]
    [Arguments("[Username(MinLength = 20, MaxLength = 3), Length(1, 10)]")]
    [Arguments("[CountryCode, ExactLength(3)]")]
    [Arguments("[CountryCode, Length(3, 3)]")]
    [Arguments("[StringPrimitive, MinLength(4096)]")]
    [Arguments("[StringPrimitive, MinLength(5000), MaxLength(6000)]")]
    [Arguments("[Email, MinLength(300)]")]
    public async Task Should_Accept_Bounds_In_Order(string attributes)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              {{attributes}}
              public readonly partial record struct Bounded;
              """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    [Arguments("[Age(Min = 200)]")]
    [Arguments("[Temperature(Max = -300)]")]
    [Arguments("[CountryCode, MinLength(3)]")]
    [Arguments("[StringPrimitive, MinLength(5000)]")]
    public async Task Should_Report_The_Default_A_Bound_Conflicts_With(string attributes)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              {{attributes}}
              public readonly partial record struct Bounded;
              """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    [Arguments("100", "[StringPrimitive, MinLength(200)]")]
    [Arguments("10000", "[StringPrimitive, MinLength(5000)]")]
    public async Task Should_Read_The_Maximum_Length_The_Project_Sets(string maxLength, string attributes)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              {{attributes}}
              public readonly partial record struct Bounded;
              """,
            builder => builder.AddGlobalOption("build_property.LaunchPadPrimitivesMaxLength", maxLength)
        );

        await VerifyAnalysis(result);
    }

    // The generator reads the range of [Money] from its Min and Max alone: a [PrimitiveRange] doesn't apply to it (LPAD2022) and doesn't hide them.
    [Test]
    public async Task Should_Report_The_Bounds_Of_Money_With_A_PrimitiveRange()
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + """
            [Money(Min = 10, Max = 5), PrimitiveRange(0, 100)]
            public sealed partial record Bounded;
            """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Report_PastOnly_And_FutureOnly_Together()
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + """
            [DatePrimitive(PastOnly = true, FutureOnly = true)]
            public readonly partial record struct Never;
            """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    [Arguments("Duration", "PastOnly")]
    [Arguments("Period", "FutureOnly")]
    [Arguments("AnnualDate", "PastOnly")]
    [Arguments("Offset", "FutureOnly")]
    public async Task Should_Report_A_Direction_On_An_Amount_Of_Time(string kind, string direction)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              [DatePrimitive(Kind = DatePrimitiveKind.{{kind}}, {{direction}} = true)]
              public readonly partial record struct Span;
              """,
            NodaTime
        );

        await VerifyAnalysis(result);
    }

    [Test]
    [Arguments("DateOnly")]
    [Arguments("DateTimeOffset")]
    public async Task Should_Accept_A_Direction_On_A_Point_In_Time(string kind)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              [DatePrimitive(Kind = DatePrimitiveKind.{{kind}}, PastOnly = true)]
              public readonly partial record struct When;
              """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    [Arguments("[MaxLength(10)]", "NumericPrimitive<int>")]
    [Arguments("[Trim]", "Percentage")]
    [Arguments("[Regex(\"^a$\")]", "DatePrimitive")]
    [Arguments("[PrimitiveRange(1, 2)]", "StringPrimitive")]
    [Arguments("[PrimitiveRange(1, 2)]", "Email")]
    [Arguments("[MaxLength(10)]", "StrongId<System.Guid>")]
    [Arguments("[PrimitiveRange(1, 2)]", "StrongId<string>")]
    [Arguments("[NotEmpty]", "SmartEnum<int>")]
    [Arguments("[MaxLength(3)]", "Money")]
    [Arguments("[CustomValidator<Sample.Check>]", "ValueObject")]
    // The generator reads a date's bounds as text and a number's as numbers: the other overload sets no bound or fails at run time.
    [Arguments("[PrimitiveRange(1, 2)]", "DatePrimitive")]
    [Arguments("[PrimitiveRange(0, 100)]", "BirthDate")]
    [Arguments("[PrimitiveRange(\"1\", \"2\")]", "NumericPrimitive<decimal>")]
    [Arguments("[PrimitiveRange(\"1\", \"2\")]", "Percentage")]
    [Arguments("[PrimitiveRange(\"1\", \"2\")]", "StrongId<int>")]
    public async Task Should_Report_A_Rule_That_Does_Not_Apply_To_The_Kind(string rule, string kind)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              public sealed class Check;

              [{{kind}}]
              {{rule}}
              {{DeclarationOf(kind)}} Target;
              """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    [Arguments("[MaxLength(10), Regex(\"^[a-z]+$\", ErrorCode = \"LOWER\"), Trim, LowerCase]", "StringPrimitive")]
    [Arguments("[MaxLength(10)]", "Email")]
    [Arguments("[PrimitiveRange(0, 10)]", "NumericPrimitive<int>")]
    [Arguments("[PrimitiveRange(\"2020-01-01\", \"2030-01-01\")]", "BirthDate")]
    [Arguments("[PrimitiveRange(\"09:00\", \"17:00\")]", "DatePrimitive(Kind = DatePrimitiveKind.TimeOnly)")]
    [Arguments("[PrimitiveRange(0.5, 99.5)]", "Percentage")]
    [Arguments("[MaxLength(10), Regex(\"^[A-Z]+$\", ErrorCode = \"CODE\")]", "StrongId<string>")]
    [Arguments("[PrimitiveRange(1, 1000)]", "StrongId<int>")]
    [Arguments("[CustomValidator<Sample.Check>]", "NumericPrimitive<decimal>")]
    [Arguments("[Normalize<Sample.Check>, NotEmpty]", "StringPrimitive")]
    [Arguments("[CustomValidator<Sample.Check>]", "StrongId<System.Guid>")]
    public async Task Should_Accept_A_Rule_That_Applies_To_The_Kind(string rule, string kind)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              public sealed class Check;

              [{{kind}}]
              {{rule}}
              public readonly partial record struct Target;
              """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Report_A_NodaTime_Kind_Without_LaunchPad_NodaTime()
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + """
            [DatePrimitive(Kind = DatePrimitiveKind.Instant, PastOnly = true)]
            public readonly partial record struct HappenedAt;

            [DateRange(Kind = DatePrimitiveKind.LocalDate)]
            public sealed partial record Stay;

            [DatePrimitive(Kind = DatePrimitiveKind.DateOnly, PastOnly = true)]
            public readonly partial record struct Day;
            """,
            NodaTime
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Accept_A_NodaTime_Kind_With_LaunchPad_NodaTime()
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + """
            [DatePrimitive(Kind = DatePrimitiveKind.Instant, PastOnly = true)]
            public readonly partial record struct HappenedAt;
            """,
            builder => NodaTime(builder).AddReferences(LaunchPadNodaTime())
        );

        await VerifyAnalysis(result);
    }

    /// <summary>An assembly named Rocket.Surgery.LaunchPad.NodaTime (the test harness names a compilation after the project without its last part).</summary>
    private static MetadataReference LaunchPadNodaTime()
    {
        var compilation = CSharpCompilation.Create(
            "Rocket.Surgery.LaunchPad.NodaTime",
            [CSharpSyntaxTree.ParseText("namespace Rocket.Surgery.LaunchPad.NodaTime { public static class Marker { } }")],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        using var image = new MemoryStream();
        var emitted = compilation.Emit(image);
        return !emitted.Success
            ? throw new InvalidOperationException(string.Join("\n", emitted.Diagnostics.Select(d => d.ToString())))
            : (MetadataReference)MetadataReference.CreateFromImage(image.ToArray());
    }

    // The NodaTime members of DatePrimitiveKind, which Primitives' targets turn on when the project references NodaTime.
    private static GeneratorTestContextBuilder NodaTime(GeneratorTestContextBuilder builder) => builder.AddPreprocessorSymbol("LAUNCHPAD_PRIMITIVES_NODATIME");
}

/// <summary>LPAD2009: a domain primitive with no rules, and its fix.</summary>
public class MissingValidationTests
{
    [Test]
    public async Task Should_Report_A_String_Primitive_Without_Rules()
    {
        var result = await Fix<RulesAnalyzer, AddNotEmptyCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive]
            public readonly partial record struct UserName;
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<AddNotEmptyCodeFix>(result, "Add [NotEmpty]") ).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Fix_With_The_Full_Name_When_The_Namespace_Is_Not_Imported()
    {
        var result = await Fix<RulesAnalyzer, AddNotEmptyCodeFix>(
            """
            namespace Sample
            {
                [Rocket.Surgery.LaunchPad.Primitives.StringPrimitive]
                public sealed partial record UserName;
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<AddNotEmptyCodeFix>(result, "Add [NotEmpty]") ).Should().BeEmpty();
    }

    [Test]
    [Arguments("[StringPrimitive, NotEmpty]")]
    [Arguments("[StringPrimitive, MaxLength(10)]")]
    [Arguments("[StringPrimitive, Regex(\"^a$\", ErrorCode = \"A\")]")]
    [Arguments("[StringPrimitive, CustomValidator<Sample.Check>]")]
    [Arguments("[Email]")]
    [Arguments("[CountryCode]")]
    [Arguments("[Percentage]")]
    [Arguments("[Month]")]
    [Arguments("[BirthDate]")]
    [Arguments("[NumericPrimitive<int>, PrimitiveRange(0, 10)]")]
    [Arguments("[DatePrimitive(PastOnly = true)]")]
    [Arguments("[DatePrimitive(FutureOnly = true)]")]
    [Arguments("[DatePrimitive, PrimitiveRange(\"2020-01-01\", \"2030-01-01\")]")]
    [Arguments("[StrongId<System.Guid>]")]
    [Arguments("[StrongId<int>]")]
    [Arguments("[StrongId<string>, MaxLength(32)]")]
    [Arguments("[StrongId<string>, Regex(\"^[A-Z]+$\", ErrorCode = \"CODE\")]")]
    [Arguments("[SmartEnum<int>]")]
    [Arguments("[ValueObject]")]
    public async Task Should_Accept_A_Primitive_With_Rules(string attributes)
    {
        var result = await Analyze<RulesAnalyzer>(
            $$"""
              using Rocket.Surgery.LaunchPad.Primitives;

              namespace Sample;

              public sealed class Check;

              {{attributes}}
              {{DeclarationOf(attributes)}} Target;
              """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Accept_A_String_Primitive_When_The_Project_Makes_Every_String_Not_Empty()
    {
        var result = await Analyze<RulesAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive]
            public readonly partial record struct UserName;
            """,
            builder => builder.AddGlobalOption("build_property.LaunchPadPrimitivesNotEmpty", "true")
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Report_A_Numeric_Or_Date_Primitive_Without_Rules_Without_A_Fix()
    {
        var result = await Fix<RulesAnalyzer, AddNotEmptyCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [NumericPrimitive<decimal>]
            public readonly partial record struct Amount;

            [DatePrimitive]
            public readonly partial record struct Day;
            """
        );

        await VerifyAnalysis(result);
    }

    // A string strong ID already rejects an empty string: without a length or a format it is any other string.
    [Test]
    public async Task Should_Report_A_String_Strong_Id_Without_A_Length_Or_Format()
    {
        var result = await Fix<RulesAnalyzer, AddNotEmptyCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StrongId<string>, NotEmpty]
            public readonly partial record struct Sku;
            """
        );

        await VerifyAnalysis(result);
    }
}
