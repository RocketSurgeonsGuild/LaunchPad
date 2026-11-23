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

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2015: Length"], CollectionOrdering.Matching);
        await ( await Fixed<AddDocumentationCodeFix>(result, "Add XML documentation") ).Should().BeEqualTo(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public readonly partial record struct UserName
            {
                /// <summary>
                /// Gets the Length.
                /// </summary>
                public int Length => 0;
            }
            """
        );
    }

    [Test]
    [Arguments("public bool IsShort() => true;", "    // A comment\n    /// <summary>\n    /// IsShort.\n    /// </summary>\n    public bool IsShort() => true;")]
    [Arguments("public static readonly int Limit = 3;", "    // A comment\n    /// <summary>\n    /// The Limit.\n    /// </summary>\n    public static readonly int Limit = 3;")]
    public async Task Should_Document_After_The_Other_Comments(string member, string documented)
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

        await ( await Fixed<AddDocumentationCodeFix>(result, "Add XML documentation") ).Should().Contain(documented);
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

        await Diagnostics(result).Should().BeEmpty();
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
                public static UserName Build(int length) => new();

                /// <summary>Makes one.</summary>
                public static UserName Parse(string value, int length) => new();

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

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2016: Build"], CollectionOrdering.Matching);
        await FixTitles<RenameFactoryCodeFix>(result).Should().BeEquivalentTo(["Rename to 'Create'", "Rename to 'Parse'"], CollectionOrdering.Any);
        var renamed = await Fixed<RenameFactoryCodeFix>(result, "Rename to 'Create'");
        await renamed.Should().Contain("public static UserName Create(int length) => new();");
        await renamed.Should().Contain("public static UserName Run() => UserName.Create(3);");
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

        await FixTitles<RenameFactoryCodeFix>(result).Should().BeEquivalentTo(["Rename to 'Parse'"], CollectionOrdering.Matching);
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
        await Diagnostics(within).Should().BeEmpty();

        var over = await Analyze<ApiReviewAnalyzer>(Members(attribute, budget + 1));
        await Diagnostics(over).Should().BeEquivalentTo(["LPAD2014: Target"], CollectionOrdering.Matching);
        await Messages(over, "LPAD2014")
             .Should()
             .BeEquivalentTo(
                  [$"Domain primitive 'Target' declares {budget + 1} public members, more than the {budget} a domain primitive of its kind should need"],
                  CollectionOrdering.Matching
              );
    }

    // A value object gets one more for each property or field, a smart enum for each static value.
    [Test]
    public async Task Should_Count_The_Members_That_Make_A_Value_Object_Or_A_Smart_Enum()
    {
        var valueObject = Members("ValueObject", 20, "/// <summary>A.</summary>\n    public string? A { get; init; }\n    /// <summary>B.</summary>\n    public string? B;");
        await Diagnostics(await Analyze<ApiReviewAnalyzer>(valueObject)).Should().BeEmpty();
        var bigValueObject = Members("ValueObject", 21, "/// <summary>A.</summary>\n    public string? A { get; init; }\n    /// <summary>B.</summary>\n    public string? B;");
        await Diagnostics(await Analyze<ApiReviewAnalyzer>(bigValueObject)).Should().BeEquivalentTo(["LPAD2014: Target"], CollectionOrdering.Matching);

        var smartEnum = Members("SmartEnum<int>", 12, "/// <summary>A.</summary>\n    public static readonly Target A = default;\n    /// <summary>B.</summary>\n    public static Target B => default;");
        await Diagnostics(await Analyze<ApiReviewAnalyzer>(smartEnum)).Should().BeEmpty();
        var bigSmartEnum = Members("SmartEnum<int>", 13, "/// <summary>A.</summary>\n    public static readonly Target A = default;\n    /// <summary>B.</summary>\n    public static Target B => default;");
        await Diagnostics(await Analyze<ApiReviewAnalyzer>(bigSmartEnum)).Should().BeEquivalentTo(["LPAD2014: Target"], CollectionOrdering.Matching);
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
