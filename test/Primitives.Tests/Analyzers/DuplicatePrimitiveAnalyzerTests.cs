using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>LPAD2013: two domain primitives with the same kind and rules; strong IDs, smart enums and value objects are left out.</summary>
public class DuplicatePrimitiveAnalyzerTests
{
    private const string Usings = """
                                  using Rocket.Surgery.LaunchPad.Primitives;

                                  namespace Sample;


                                  """;

    [Test]
    public async Task Should_Report_The_Second_Of_Two_Identical_Primitives()
    {
        var result = await Analyze<DuplicatePrimitiveAnalyzer>(
            Usings
          + """
            [StringPrimitive, Regex("^[A-Z]+$")]
            public readonly partial record struct EmployeeCode;

            [StringPrimitive]
            [Regex("^[A-Z]+$")]
            public sealed partial record UserCode;
            """
        );

        await VerifyAnalysis(result);
    }

    // A [GeneratedRegex] member is a rule: its pattern and options count, its name doesn't.
    [Test]
    public async Task Should_Compare_The_GeneratedRegex_Members()
    {
        var result = await Analyze<DuplicatePrimitiveAnalyzer>(
            """
            using System.Text.RegularExpressions;
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, Trim]
            public readonly partial record struct Sku
            {
                [GeneratedRegex("^[A-Z]{3}$")]
                private static partial Regex Format { get; }
            }

            [StringPrimitive, Trim]
            public readonly partial record struct PostCode
            {
                [GeneratedRegex("^[0-9]{5}$")]
                private static partial Regex Format { get; }
            }

            [StringPrimitive, Trim]
            public readonly partial record struct Ticker
            {
                [GeneratedRegex("^[A-Z]{3}$")]
                private static partial Regex Symbol();
            }
            """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    [Arguments("[StringPrimitive, MinLength(5)]", "[MinLength(5), StringPrimitive]")]
    [Arguments("[DatePrimitive(PastOnly = true, Kind = DatePrimitiveKind.DateOnly)]", "[DatePrimitive(Kind = DatePrimitiveKind.DateOnly, PastOnly = true)]")]
    [Arguments("[Email]", "[Email]")]
    [Arguments("[Url(AllowedSchemes = new[] { \"https\" })]", "[Url(AllowedSchemes = new[] { \"https\" })]")]
    public async Task Should_Report_Primitives_Written_Differently_With_The_Same_Rules(string first, string second)
    {
        var result = await Analyze<DuplicatePrimitiveAnalyzer>(
            Usings
          + $$"""
              {{first}}
              public readonly partial record struct A;

              {{second}}
              public readonly partial record struct B;
              """
        );

        await VerifyAnalysis(result, settings => settings.UseTextForParameters(ParameterText(first, second)));
    }

    [Test]
    [Arguments("[StringPrimitive, MinLength(5)]", "[StringPrimitive, MinLength(10)]")]
    [Arguments("[StringPrimitive, NotEmpty]", "[StringPrimitive, NotEmpty, Trim]")]
    [Arguments("[NumericPrimitive<int>, PrimitiveRange(0, 10)]", "[NumericPrimitive<long>, PrimitiveRange(0, 10)]")]
    [Arguments("[StrongId<System.Guid>]", "[StrongId<System.Guid>(GuidVersion = 4)]")]
    [Arguments("[ValueObject]", "[ValueObject]")]
    [Arguments("[SmartEnum<int>]", "[SmartEnum<int>]")]
    // Strong IDs are distinct types of the same shape on purpose: an OrderId and a CustomerId are both a Guid.
    [Arguments("[StrongId<System.Guid>]", "[StrongId<System.Guid>]")]
    [Arguments("[StrongId<int>]", "[StrongId<int>]")]
    [Arguments("[StrongId<string>, MaxLength(32)]", "[StrongId<string>, MaxLength(32)]")]
    public async Task Should_Accept_Primitives_That_Differ_Are_Strong_Ids_Or_Are_Defined_By_Their_Members(string first, string second)
    {
        var result = await Analyze<DuplicatePrimitiveAnalyzer>(
            Usings
          + $$"""
              {{first}}
              public readonly partial record struct A;

              {{second}}
              public readonly partial record struct B;
              """
        );

        await VerifyAnalysis(result, settings => settings.UseTextForParameters(ParameterText(first, second)));
    }

    [Test]
    public async Task Should_Report_Every_Copy_As_A_Duplicate_Of_The_First()
    {
        var result = await Analyze<DuplicatePrimitiveAnalyzer>(
            Usings
          + """
            [StringPrimitive, NotEmpty]
            public readonly partial record struct C;

            [StringPrimitive, NotEmpty]
            public readonly partial record struct A;

            [StringPrimitive, NotEmpty, MaxLength(99)]
            public readonly partial record struct Unique;

            [StringPrimitive, NotEmpty]
            public readonly partial record struct B;
            """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Ignore_Attributes_That_Are_Not_Primitive_Attributes()
    {
        var result = await Analyze<DuplicatePrimitiveAnalyzer>(
            """
            namespace Custom
            {
                public class StringPrimitiveAttribute : System.Attribute;
            }

            namespace Sample
            {
                [Custom.StringPrimitive]
                public readonly partial record struct A;

                [Custom.StringPrimitive]
                public readonly partial record struct B;
            }
            """
        );

        await VerifyAnalysis(result);
    }

    // The pair names the snapshot without its spaces, which written out can be longer than a file name may be, and with '-' for the characters
    // Verify leaves in a parameter text it is given but Windows doesn't allow in a file name.
    private static string ParameterText(string first, string second) =>
        string.Concat($"{first}_{second}".Where(c => c != ' ').Select(c => "<>:\"|?*/\\".Contains(c, StringComparison.Ordinal) ? '-' : c));
}
