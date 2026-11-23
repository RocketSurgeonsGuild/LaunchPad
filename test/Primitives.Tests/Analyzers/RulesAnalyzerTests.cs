using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>LPAD2004 (regex), LPAD2005 (casing), LPAD2006 (bounds), LPAD2020 (PastOnly and FutureOnly) and LPAD2022 (rule and kind).</summary>
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
        var result = await Analyze<RulesAnalyzer>(
            Usings
          + """
            [StringPrimitive, NotEmpty, LowerCase, UpperCase]
            public readonly partial record struct Code;

            [StringPrimitive, NotEmpty, LowerCase, Trim]
            public readonly partial record struct Lower;
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2005: Code"], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments("[StringPrimitive, MinLength(10), MaxLength(5)]", "MinLength(10)")]
    [Arguments("[StringPrimitive, MaxLength(5), MinLength(10)]", "MaxLength(5)")]
    [Arguments("[StringPrimitive, Length(10, 5)]", "Length(10, 5)")]
    [Arguments("[NumericPrimitive<int>, PrimitiveRange(10, 5)]", "PrimitiveRange(10, 5)")]
    [Arguments("[NumericPrimitive<double>, PrimitiveRange(1.5, 0.5)]", "PrimitiveRange(1.5, 0.5)")]
    [Arguments("[NumericPrimitive<decimal>, PrimitiveRange(\"100.5\", \"10\")]", "PrimitiveRange(\"100.5\", \"10\")")]
    [Arguments("[DatePrimitive, PrimitiveRange(\"2030-01-01\", \"2020-01-01\")]", "PrimitiveRange(\"2030-01-01\", \"2020-01-01\")")]
    [Arguments("[DatePrimitive(Kind = DatePrimitiveKind.TimeOnly), PrimitiveRange(\"17:00\", \"09:00\")]", "PrimitiveRange(\"17:00\", \"09:00\")")]
    [Arguments("[Percentage(Min = 50, Max = 10)]", "Percentage(Min = 50, Max = 10)")]
    [Arguments("[Username(MinLength = 20, MaxLength = 3)]", "Username(MinLength = 20, MaxLength = 3)")]
    [Arguments("[FiscalYear(MinYear = 2100, MaxYear = 2000)]", "FiscalYear(MinYear = 2100, MaxYear = 2000)")]
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
    [Arguments("[NumericPrimitive<decimal>, PrimitiveRange(\"-10\", \"10\")]")]
    [Arguments("[DatePrimitive, PrimitiveRange(\"2020-01-01\", \"2030-01-01\")]")]
    [Arguments("[Percentage(Min = 10, Max = 50)]")]
    [Arguments("[Percentage(Min = 10)]")]
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
