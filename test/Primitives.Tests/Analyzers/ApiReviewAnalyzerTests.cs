using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>LPAD2014 (API budget), LPAD2015 (XML documentation) and LPAD2016 (factory name), with their fixes.</summary>
public class ApiReviewAnalyzerTests
{
    [Test]
    public async Task Should_Report_A_Public_Member_Without_Documentation()
    {
        var result = await Fix<ApiReviewAnalyzer, AddDocumentationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public readonly partial record struct UserName
            {
                public int Length => 0;
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<AddDocumentationCodeFix>(result, "Add XML documentation") ).Should().BeEmpty();
    }

    [Test]
    [Arguments("public bool IsShort() => true;")]
    [Arguments("public static readonly int Limit = 3;")]
    public async Task Should_Document_After_The_Other_Comments(string member)
    {
        var result = await Fix<ApiReviewAnalyzer, AddDocumentationCodeFix>(
            $$"""
              using Rocket.Surgery.LaunchPad.Primitives;

              namespace Sample;

              [StringPrimitive, NotEmpty]
              public readonly partial record struct UserName
              {
                  // A comment
                  {{member}}
              }
              """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<AddDocumentationCodeFix>(result, "Add XML documentation") ).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Accept_Documented_Overriding_And_Generated_Members()
    {
        var result = await Analyze<ApiReviewAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            /// <param name="Value">The value.</param>
            [StringPrimitive, NotEmpty]
            public readonly partial record struct UserName(string Value)
            {
                /// <summary>The length.</summary>
                public int Length => 0;

                /// <inheritdoc />
                public bool IsEmpty => false;

                public override string ToString() => Value;

                private int Hidden() => 0;
            }
            """,
            builder => builder.AddSource(
                "UserName.g.cs",
                """
                namespace Sample;

                public readonly partial record struct UserName
                {
                    public static UserName Build(string value) => default;
                    public int Undocumented => 0;
                }
                """
            )
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Report_A_Factory_Method_With_Another_Name()
    {
        var result = await Fix<ApiReviewAnalyzer, RenameFactoryCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public sealed partial record UserName
            {
                /// <summary>Makes one.</summary>
                public static UserName Build(int length) => Create(new string('a', length));

                /// <summary>Makes one.</summary>
                public static UserName Parse(string value, int length) => Create(value[..length]);

                /// <summary>Not a factory.</summary>
                public static int Count(UserName name) => 0;

                /// <summary>Not static.</summary>
                public UserName With(int length) => this;
            }

            public static class Uses
            {
                public static UserName Run() => UserName.Build(3);
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<RenameFactoryCodeFix>(result, "Rename to 'Create'") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<RenameFactoryCodeFix>(result, "Rename to 'Parse'") ).Should().BeEmpty();
    }

    // The generated Create(TValue) takes the name for the same parameters, so the fix offers only what doesn't collide.
    [Test]
    public async Task Should_Offer_Only_Names_That_Do_Not_Collide()
    {
        var result = await Fix<ApiReviewAnalyzer, RenameFactoryCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public sealed partial record UserName
            {
                /// <summary>Makes one.</summary>
                public static UserName From(string value) => new();
            }
            """,
            builder => builder.AddSource(
                "UserName.g.cs",
                """
                namespace Sample;

                public sealed partial record UserName
                {
                    public static UserName Create(string value) => new();
                }
                """
            )
        );

        await VerifyAnalysis(result);
    }

    [Test]
    [Arguments("StringPrimitive, NotEmpty", 25)]
    [Arguments("Email", 25)]
    [Arguments("NumericPrimitive<int>, PrimitiveRange(0, 1)", 27)]
    [Arguments("Percentage", 27)]
    [Arguments("DatePrimitive(PastOnly = true)", 23)]
    [Arguments("StrongId<System.Guid>", 15)]
    public async Task Should_Report_More_Members_Than_The_Budget_Of_The_Kind(string attribute, int budget)
    {
        var within = await Analyze<ApiReviewAnalyzer>(Members(attribute, budget));
        var over = await Analyze<ApiReviewAnalyzer>(Members(attribute, budget + 1));

        await VerifySnapshot(new { Within = await Describe(within), Over = await Describe(over) });
    }

    // A value object gets one more for each property or field, a smart enum for each static value.
    [Test]
    public async Task Should_Count_The_Members_That_Make_A_Value_Object_Or_A_Smart_Enum()
    {
        var valueObject = Members("ValueObject", 20, "/// <summary>A.</summary>\n    public string? A { get; init; }\n    /// <summary>B.</summary>\n    public string? B;");
        var bigValueObject = Members("ValueObject", 21, "/// <summary>A.</summary>\n    public string? A { get; init; }\n    /// <summary>B.</summary>\n    public string? B;");
        var smartEnum = Members("SmartEnum<int>", 12, "/// <summary>A.</summary>\n    public static readonly Target A = default;\n    /// <summary>B.</summary>\n    public static Target B => default;");
        var bigSmartEnum = Members("SmartEnum<int>", 13, "/// <summary>A.</summary>\n    public static readonly Target A = default;\n    /// <summary>B.</summary>\n    public static Target B => default;");

        await VerifySnapshot(
            new
            {
                ValueObjectWithinBudget = await Describe(await Analyze<ApiReviewAnalyzer>(valueObject)),
                ValueObjectOverBudget = await Describe(await Analyze<ApiReviewAnalyzer>(bigValueObject)),
                SmartEnumWithinBudget = await Describe(await Analyze<ApiReviewAnalyzer>(smartEnum)),
                SmartEnumOverBudget = await Describe(await Analyze<ApiReviewAnalyzer>(bigSmartEnum)),
            }
        );
    }

    private static string Members(string attribute, int count, string extra = "") =>
        $$"""
          using Rocket.Surgery.LaunchPad.Primitives;

          namespace Sample;

          [{{attribute}}]
          public readonly partial record struct Target
          {
              {{extra}}
          {{string.Join("\n", Enumerable.Range(1, count).Select(i => $"    /// <summary>M{i}.</summary>\n    public void M{i}() {{ }}"))}}
          }
          """;
}
