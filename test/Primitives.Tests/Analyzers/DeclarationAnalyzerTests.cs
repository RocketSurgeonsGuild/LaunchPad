using Microsoft.CodeAnalysis;

using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>LPAD2001 (partial), LPAD2002 (readonly), LPAD2003 (record), LPAD2012 (public constructor) and LPAD2019 (sealed), with their fixes.</summary>
public class DeclarationAnalyzerTests
{
    [Test]
    public async Task Should_Compile_The_Attributes_The_Tests_Add()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty, MaxLength(10), Trim]
            public readonly partial record struct UserName;

            [NumericPrimitive<decimal>(Operations = NumericOperations.Additive), PrimitiveRange(0, 100)]
            public sealed partial record Amount;

            [StrongId<System.Guid>(GuidVersion = 4)]
            public readonly partial record struct OrderId;
            """
        );

        await result.FinalDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.ToString()).Should().BeEmpty();
        await Diagnostics(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Report_A_Struct_That_Is_Not_Partial()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public readonly record struct UserName;
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2001: UserName"], CollectionOrdering.Matching);
        await ( await Fixed<DeclarationCodeFix>(result, "Make the domain primitive partial") ).Should().BeEqualTo(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public readonly partial record struct UserName;
            """
        );
    }

    [Test]
    public async Task Should_Report_A_Class_That_Is_Not_Partial()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [Email]
            public sealed record EmailAddress;
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2001: EmailAddress"], CollectionOrdering.Matching);
        await ( await Fixed<DeclarationCodeFix>(result, "Make the domain primitive partial") ).Should().Contain("public sealed partial record EmailAddress;");
    }

    // The attribute is found by what it derives from, however it is written.
    [Test]
    [Arguments("[StringPrimitive]")]
    [Arguments("[StringPrimitiveAttribute]")]
    [Arguments("[Rocket.Surgery.LaunchPad.Primitives.StringPrimitive]")]
    [Arguments("[global::Rocket.Surgery.LaunchPad.Primitives.StringPrimitiveAttribute]")]
    [Arguments("[Alias]")]
    [Arguments("[System.Serializable, Slug]")]
    public async Task Should_Recognise_The_Attribute_However_It_Is_Written(string attribute)
    {
        var result = await Analyze<DeclarationAnalyzer>(
            $$"""
              using Rocket.Surgery.LaunchPad.Primitives;
              using Alias = Rocket.Surgery.LaunchPad.Primitives.StringPrimitiveAttribute;

              namespace Sample;

              {{attribute}}
              public readonly record struct UserName;
              """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2001: UserName"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Report_A_Struct_That_Is_Not_Readonly()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public partial record struct UserName;
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2002: UserName"], CollectionOrdering.Matching);
        await ( await Fixed<DeclarationCodeFix>(result, "Make the domain primitive readonly") ).Should().Contain("public readonly partial record struct UserName;");
    }

    [Test]
    public async Task Should_Report_A_Struct_That_Is_Neither_Readonly_Nor_Partial()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            record struct UserName;
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2001: UserName", "LPAD2002: UserName"], CollectionOrdering.Matching);
        await ( await Fixed<DeclarationCodeFix>(result, "Make the domain primitive readonly") ).Should().Contain("\nreadonly record struct UserName;");
        await ( await Fixed<DeclarationCodeFix>(result, "Make the domain primitive partial") ).Should().Contain("\npartial record struct UserName;");
    }

    // The generated part is always right, so it must not hide what the user's declaration lacks.
    [Test]
    public async Task Should_Read_Only_The_Declarations_The_User_Wrote()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public partial record struct UserName;
            """,
            builder => builder.AddSource(
                "UserName.g.cs",
                """
                // <auto-generated />
                namespace Sample;

                public readonly partial record struct UserName;
                """
            )
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2002: UserName"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Report_A_Class_That_Is_Not_Sealed()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public partial record UserName;
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2019: UserName"], CollectionOrdering.Matching);
        var fixedSource = await Fixed<DeclarationCodeFix>(result, "Make the domain primitive sealed");
        await fixedSource.Should().Contain("public sealed partial record UserName;");
    }

    [Test]
    public async Task Should_Replace_Abstract_With_Sealed()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, MaxLength(10)]
            public abstract partial record Code;
            """
        );

        await ( await Fixed<DeclarationCodeFix>(result, "Make the domain primitive sealed") ).Should().Contain("public sealed partial record Code;");
    }

    [Test]
    public async Task Should_Report_A_Struct_That_Is_Not_A_Record()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public struct UserName
            {
                public int Length => 0;
            }
            """
        );

        // Only LPAD2003: its fix makes the declaration whole.
        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2003: UserName"], CollectionOrdering.Matching);
        await FixTitles<DeclarationCodeFix>(result).Should().BeEquivalentTo(["Convert to a readonly partial record struct"], CollectionOrdering.Matching);
        await ( await Fixed<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().BeEqualTo(
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
    }

    [Test]
    public async Task Should_Report_A_Class_That_Is_Not_A_Record()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            class UserName { }
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2003: UserName"], CollectionOrdering.Matching);
        await FixTitles<DeclarationCodeFix>(result)
             .Should()
             .BeEquivalentTo(["Convert to a sealed partial record", "Convert to a readonly partial record struct"], CollectionOrdering.Any);
        await ( await Fixed<DeclarationCodeFix>(result, "Convert to a sealed partial record") ).Should().Contain("[StringPrimitive, NotEmpty]\nsealed partial record UserName { }");
        await ( await Fixed<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().Contain("[StringPrimitive, NotEmpty]\nreadonly partial record struct UserName { }");
    }

    [Test]
    public async Task Should_Convert_A_Sealed_Class_Keeping_Its_Modifiers_And_Base_List()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public sealed class UserName : System.IComparable<UserName>
            {
                public int CompareTo(UserName? other) => 0;
            }
            """
        );

        await ( await Fixed<DeclarationCodeFix>(result, "Convert to a sealed partial record") ).Should().Contain("public sealed partial record UserName : System.IComparable<UserName>");
        await ( await Fixed<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().Contain("public readonly partial record struct UserName : System.IComparable<UserName>");
    }

    [Test]
    public async Task Should_Accept_Well_Formed_Domain_Primitives()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public readonly partial record struct UserName;

            [StringPrimitive, NotEmpty]
            public sealed partial record DisplayName;

            [StringPrimitive, NotEmpty]
            internal readonly partial record struct Nested
            {
                private Nested(string value) { }
            }
            """
        );

        await Diagnostics(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Ignore_Types_That_Are_Not_Domain_Primitives()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            namespace Sample;

            public class PlainAttribute : System.Attribute;

            [Plain, System.Serializable]
            public struct Plain
            {
                public Plain(int value) { }
            }

            public class Other;
            """
        );

        await Diagnostics(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Ignore_An_Attribute_Of_The_Same_Name_In_Another_Namespace()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            namespace Custom
            {
                public class StringPrimitiveAttribute : System.Attribute;
            }

            namespace Sample
            {
                [Custom.StringPrimitive]
                public struct UserName;
            }
            """
        );

        await Diagnostics(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Report_A_Public_Constructor()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StrongId<System.Guid>]
            public readonly partial record struct OrderId
            {
                public OrderId(System.Guid value) { }
            }

            [Email]
            public sealed partial record EmailAddress
            {
                public EmailAddress() { }
            }

            [StringPrimitive, NotEmpty]
            public readonly partial record struct UserName
            {
                internal UserName(int length) { }
                private UserName(string value) { }
            }
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2012: OrderId", "LPAD2012: EmailAddress"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Report_The_Primary_Constructor_Of_A_Positional_Record()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public readonly partial record struct UserName(string Value);
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2012: UserName"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Ignore_A_Public_Constructor_The_Generator_Wrote()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public readonly partial record struct UserName;
            """,
            builder => builder.AddSource(
                "UserName.generated.cs",
                """
                namespace Sample;

                public readonly partial record struct UserName
                {
                    public UserName(int value) { }
                }
                """
            )
        );

        await Diagnostics(result).Should().BeEmpty();
    }
}
