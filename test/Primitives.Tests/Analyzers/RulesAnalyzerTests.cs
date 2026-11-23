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

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2004: Regex(\"[a-z\")"], CollectionOrdering.Matching);
        var message = await Messages(result, "LPAD2004").Should().HaveSingleItem();
        await message.Should().StartWith("The pattern '[a-z' of [Regex] is not a valid regular expression: ");
    }

    [Test]
    public async Task Should_Accept_Valid_Regular_Expressions()
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + """
            [StringPrimitive, Regex("^[A-Z]{2}[0-9]+$"), Regex(@"^\S+$")]
            public readonly partial record struct Code;
            """
        );

        await Diagnostics(result).Should().BeEmpty();
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

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2005: Code"], CollectionOrdering.Matching);
        await FixTitles<RemoveNormalizationCodeFix>(result).Should().BeEquivalentTo(["Remove [LowerCase]", "Remove [UpperCase]"], CollectionOrdering.Matching);
        await ( await Fixed<RemoveNormalizationCodeFix>(result, "Remove [LowerCase]") ).Should().Contain("[StringPrimitive, NotEmpty, UpperCase]\npublic readonly partial record struct Code;");
        await ( await Fixed<RemoveNormalizationCodeFix>(result, "Remove [UpperCase]") ).Should().Contain("[StringPrimitive, NotEmpty, LowerCase]\npublic readonly partial record struct Code;");
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, "Remove [LowerCase]") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, "Remove [UpperCase]") ).Should().BeEmpty();
    }

    // An attribute alone in its list goes with its list; the first list leaves its comments to the next one.
    [Test]
    [Arguments("Remove [LowerCase]", "/// <summary>A code.</summary>\n[UpperCase]\n[StringPrimitive, NotEmpty]\npublic readonly partial record struct Code;")]
    [Arguments("Remove [UpperCase]", "/// <summary>A code.</summary>\n[StringPrimitive, NotEmpty]\n[LowerCase]\npublic readonly partial record struct Code;")]
    public async Task Should_Remove_A_Normalization_Alone_In_Its_List(string title, string fixedSource)
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

        await ( await Fixed<RemoveNormalizationCodeFix>(result, title) ).Should().Contain(fixedSource);
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, title) ).Should().BeEmpty();
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

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2005: Code"], CollectionOrdering.Matching);
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, "Remove [LowerCase]") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<RemoveNormalizationCodeFix>(result, "Remove [UpperCase]") ).Should().BeEmpty();
    }

    [Test]
    [Arguments("[StringPrimitive, MinLength(10), MaxLength(5)]", "MinLength(10)")]
    [Arguments("[StringPrimitive, MaxLength(5), MinLength(10)]", "MaxLength(5)")]
    [Arguments("[StringPrimitive, Length(10, 5)]", "Length(10, 5)")]
    [Arguments("[NumericPrimitive<int>, PrimitiveRange(10, 5)]", "PrimitiveRange(10, 5)")]
    [Arguments("[NumericPrimitive<double>, PrimitiveRange(1.5, 0.5)]", "PrimitiveRange(1.5, 0.5)")]
    [Arguments("[DatePrimitive, PrimitiveRange(\"2030-01-01\", \"2020-01-01\")]", "PrimitiveRange(\"2030-01-01\", \"2020-01-01\")")]
    [Arguments(
        "[DatePrimitive(Kind = DatePrimitiveKind.DateTimeOffset), PrimitiveRange(\"2030-01-01T00:00:00Z\", \"2020-01-01T00:00:00Z\")]",
        "PrimitiveRange(\"2030-01-01T00:00:00Z\", \"2020-01-01T00:00:00Z\")"
    )]
    [Arguments("[DatePrimitive(Kind = DatePrimitiveKind.TimeOnly), PrimitiveRange(\"17:00\", \"09:00\")]", "PrimitiveRange(\"17:00\", \"09:00\")")]
    [Arguments("[Percentage(Min = 50, Max = 10)]", "Percentage(Min = 50, Max = 10)")]
    [Arguments("[Username(MinLength = 20, MaxLength = 3)]", "Username(MinLength = 20, MaxLength = 3)")]
    [Arguments("[FiscalYear(MinYear = 2100, MaxYear = 2000)]", "FiscalYear(MinYear = 2100, MaxYear = 2000)")]
    [Arguments("[Money(Min = 10, Max = 5)]", "Money(Min = 10, Max = 5)")]
    // A bound of the kind against the other bound's default, or against a rule.
    [Arguments("[Age(Min = 200)]", "Age(Min = 200)")]
    [Arguments("[Percentage(Max = -1)]", "Percentage(Max = -1)")]
    [Arguments("[Temperature(Max = -300)]", "Temperature(Max = -300)")]
    [Arguments("[FiscalYear(MinYear = 10000)]", "FiscalYear(MinYear = 10000)")]
    [Arguments("[Username(MinLength = 60)]", "Username(MinLength = 60)")]
    [Arguments("[Username(MaxLength = 2)]", "Username(MaxLength = 2)")]
    [Arguments("[Username(MaxLength = 4), MinLength(5)]", "Username(MaxLength = 4)")]
    [Arguments("[Email(MaxLength = 3), MinLength(5)]", "Email(MaxLength = 3)")]
    [Arguments("[CountryCode, MinLength(3)]", "CountryCode")]
    // A minimum length against the project's maximum length, 4096 by default, where neither a rule nor the kind sets one.
    [Arguments("[StringPrimitive, MinLength(5000)]", "MinLength(5000)")]
    [Arguments("[StrongId<string>, MinLength(5000)]", "MinLength(5000)")]
    public async Task Should_Report_A_Minimum_Greater_Than_Its_Maximum(string attributes, string reported)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              {{attributes}}
              public readonly partial record struct Bounded;
              """
        );

        await Diagnostics(result).Should().BeEquivalentTo([$"LPAD2006: {reported}"], CollectionOrdering.Matching);
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

        await Diagnostics(result).Should().BeEmpty();
    }

    [Test]
    [Arguments("[Age(Min = 200)]", "The minimum (200) is greater than the maximum (150), so no value is valid")]
    [Arguments("[Temperature(Max = -300)]", "The minimum (-273.15) is greater than the maximum (-300), so no value is valid")]
    [Arguments("[CountryCode, MinLength(3)]", "The minimum (3) is greater than the maximum (2), so no value is valid")]
    [Arguments(
        "[StringPrimitive, MinLength(5000)]",
        "The minimum (5000) is greater than the maximum (4096, the project's LaunchPadPrimitivesMaxLength), so no value is valid"
    )]
    public async Task Should_Report_The_Default_A_Bound_Conflicts_With(string attributes, string message)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              {{attributes}}
              public readonly partial record struct Bounded;
              """
        );

        await Messages(result, "LPAD2006").Should().BeEquivalentTo([message], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments("100", "[StringPrimitive, MinLength(200)]", true)]
    [Arguments("10000", "[StringPrimitive, MinLength(5000)]", false)]
    public async Task Should_Read_The_Maximum_Length_The_Project_Sets(string maxLength, string attributes, bool reported)
    {
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + $$"""
              {{attributes}}
              public readonly partial record struct Bounded;
              """,
            builder => builder.AddGlobalOption("build_property.LaunchPadPrimitivesMaxLength", maxLength)
        );

        string[] expected = reported ? ["LPAD2006: MinLength(200)"] : [];
        await Diagnostics(result).Should().BeEquivalentTo(expected, CollectionOrdering.Matching);
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

        await Diagnostics(result)
             .Should()
             .BeEquivalentTo(["LPAD2006: Money(Min = 10, Max = 5)", "LPAD2022: PrimitiveRange(0, 100)"], CollectionOrdering.Matching);
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

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2020: DatePrimitive(PastOnly = true, FutureOnly = true)"], CollectionOrdering.Matching);
        await Messages(result, "LPAD2020").Should().BeEquivalentTo(["Domain primitive 'Never' sets both PastOnly and FutureOnly, so no value is valid"], CollectionOrdering.Matching);
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

        await Messages(result, "LPAD2020")
             .Should()
             .BeEquivalentTo([$"Domain primitive 'Span' sets {direction} on the {kind} kind, which is not a point in time"], CollectionOrdering.Matching);
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

        await Diagnostics(result).Should().BeEmpty();
    }

    [Test]
    [Arguments("[MaxLength(10)]", "NumericPrimitive<int>", "MaxLength", "a numeric primitive, and the rule applies to a string")]
    [Arguments("[Trim]", "Percentage", "Trim", "a numeric primitive, and the rule applies to a string")]
    [Arguments("[Regex(\"^a$\")]", "DatePrimitive", "Regex", "a date primitive, and the rule applies to a string")]
    [Arguments("[PrimitiveRange(1, 2)]", "StringPrimitive", "PrimitiveRange", "a string primitive, and the rule applies to a number, a date or a time")]
    [Arguments("[PrimitiveRange(1, 2)]", "Email", "PrimitiveRange", "a string primitive, and the rule applies to a number, a date or a time")]
    [Arguments("[MaxLength(10)]", "StrongId<System.Guid>", "MaxLength", "a strong ID of Guid, and the rule applies to a string")]
    [Arguments("[PrimitiveRange(1, 2)]", "StrongId<string>", "PrimitiveRange", "a strong ID of string, and the rule applies to a number, a date or a time")]
    [Arguments("[NotEmpty]", "SmartEnum<int>", "NotEmpty", "a smart enum, which takes no rules: its values are its members")]
    [Arguments("[MaxLength(3)]", "Money", "MaxLength", "a value object, which takes no rules: check its members in their own domain primitives")]
    [Arguments("[CustomValidator<Sample.Check>]", "ValueObject", "CustomValidator", "a value object, which takes no rules: check its members in their own domain primitives")]
    // The generator reads a date's bounds as text and a number's as numbers: the other overload sets no bound or fails at run time.
    [Arguments("[PrimitiveRange(1, 2)]", "DatePrimitive", "PrimitiveRange", "a date primitive, whose bounds are ISO 8601 text: use [PrimitiveRange(\"min\", \"max\")]")]
    [Arguments("[PrimitiveRange(0, 100)]", "BirthDate", "PrimitiveRange", "a date primitive, whose bounds are ISO 8601 text: use [PrimitiveRange(\"min\", \"max\")]")]
    [Arguments("[PrimitiveRange(\"1\", \"2\")]", "NumericPrimitive<decimal>", "PrimitiveRange", "a numeric primitive, whose bounds are numbers: use [PrimitiveRange(min, max)]")]
    [Arguments("[PrimitiveRange(\"1\", \"2\")]", "Percentage", "PrimitiveRange", "a numeric primitive, whose bounds are numbers: use [PrimitiveRange(min, max)]")]
    [Arguments("[PrimitiveRange(\"1\", \"2\")]", "StrongId<int>", "PrimitiveRange", "a strong ID of int, whose bounds are numbers: use [PrimitiveRange(min, max)]")]
    public async Task Should_Report_A_Rule_That_Does_Not_Apply_To_The_Kind(string rule, string kind, string name, string reason)
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

        await Messages(result, "LPAD2022").Should().BeEquivalentTo([$"[{name}] does not apply to 'Target', {reason}"], CollectionOrdering.Matching);
        await Diagnostics(result).Where(d => d.StartsWith("LPAD2022", StringComparison.Ordinal)).Should().BeEquivalentTo([$"LPAD2022: {rule[1..^1]}"], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments("[MaxLength(10), Regex(\"^[a-z]+$\"), Trim, LowerCase]", "StringPrimitive")]
    [Arguments("[MaxLength(10)]", "Email")]
    [Arguments("[PrimitiveRange(0, 10)]", "NumericPrimitive<int>")]
    [Arguments("[PrimitiveRange(\"2020-01-01\", \"2030-01-01\")]", "BirthDate")]
    [Arguments("[PrimitiveRange(\"09:00\", \"17:00\")]", "DatePrimitive(Kind = DatePrimitiveKind.TimeOnly)")]
    [Arguments("[PrimitiveRange(0.5, 99.5)]", "Percentage")]
    [Arguments("[MaxLength(10), Regex(\"^[A-Z]+$\")]", "StrongId<string>")]
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

        await Diagnostics(result).Should().BeEmpty();
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

        await Diagnostics(result)
             .Should()
             .BeEquivalentTo(
                  ["LPAD2021: DatePrimitive(Kind = DatePrimitiveKind.Instant, PastOnly = true)", "LPAD2021: DateRange(Kind = DatePrimitiveKind.LocalDate)"],
                  CollectionOrdering.Matching
              );
        await Messages(result, "LPAD2021")
             .Should()
             .Contain(
                  "Domain primitive 'HappenedAt' wraps NodaTime's Instant, but the project does not reference Rocket.Surgery.LaunchPad.NodaTime, whose JSON converters its JSON converter uses"
              );
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

        await Diagnostics(result).Should().BeEmpty();
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

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2009: UserName"], CollectionOrdering.Matching);
        await ( await Fixed<AddNotEmptyCodeFix>(result, "Add [NotEmpty]") ).Should().BeEqualTo(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive]
            [NotEmpty]
            public readonly partial record struct UserName;
            """
        );
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

        await ( await Fixed<AddNotEmptyCodeFix>(result, "Add [NotEmpty]") ).Should().BeEqualTo(
            """
            namespace Sample
            {
                [Rocket.Surgery.LaunchPad.Primitives.StringPrimitive]
                [global::Rocket.Surgery.LaunchPad.Primitives.NotEmpty]
                public sealed partial record UserName;
            }
            """
        );
        await ( await ErrorsAfterFix<AddNotEmptyCodeFix>(result, "Add [NotEmpty]") ).Should().BeEmpty();
    }

    [Test]
    [Arguments("[StringPrimitive, NotEmpty]")]
    [Arguments("[StringPrimitive, MaxLength(10)]")]
    [Arguments("[StringPrimitive, Regex(\"^a$\")]")]
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
    [Arguments("[StrongId<string>, Regex(\"^[A-Z]+$\")]")]
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
              public readonly partial record struct Target;
              """
        );

        await Diagnostics(result).Should().BeEmpty();
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

        await Diagnostics(result).Should().BeEmpty();
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

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2009: Amount", "LPAD2009: Day"], CollectionOrdering.Matching);
        await FixTitles<AddNotEmptyCodeFix>(result).Should().BeEmpty();
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

        await Messages(result, "LPAD2009")
             .Should()
             .BeEquivalentTo(
                  ["Domain primitive 'Sku' declares no rules to check its value: a string strong ID without a length or a [Regex] accepts any string"],
                  CollectionOrdering.Matching
              );
        await FixTitles<AddNotEmptyCodeFix>(result).Should().BeEmpty();
    }
}
