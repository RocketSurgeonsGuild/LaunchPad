using Microsoft.CodeAnalysis;

using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>
///     LPAD2001 (partial, and the types that contain it), LPAD2002 (readonly), LPAD2003 (record), LPAD2012 (public constructor) and LPAD2019
///     (sealed), with their fixes. Every fixed project is compiled with the generator.
/// </summary>
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
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive partial") ).Should().BeEmpty();
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
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive partial") ).Should().BeEmpty();
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
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive readonly") ).Should().BeEmpty();
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
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive readonly") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive partial") ).Should().BeEmpty();
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
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive sealed") ).Should().BeEmpty();
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
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive sealed") ).Should().BeEmpty();
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
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().BeEmpty();
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
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a sealed partial record") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Convert_A_Sealed_Class_Keeping_Its_Modifiers_And_Base_List()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            public interface ILabelled
            {
                string Label { get; }
            }

            [StringPrimitive, NotEmpty]
            public sealed class UserName : ILabelled
            {
                public string Label => "user";
            }
            """
        );

        await ( await Fixed<DeclarationCodeFix>(result, "Convert to a sealed partial record") ).Should().Contain("public sealed partial record UserName : ILabelled");
        await ( await Fixed<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().Contain("public readonly partial record struct UserName : ILabelled");
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a sealed partial record") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().BeEmpty();
    }

    // A type declared in several files is one type: converting one declaration would leave the others a class (CS0261).
    [Test]
    public async Task Should_Convert_Every_Declaration_Of_A_Type_Declared_In_Several_Files()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public partial class UserName;
            """,
            builder => builder.AddSource(
                "UserName.Label.cs",
                """
                namespace Sample;

                public partial class UserName
                {
                    public string Label => "user";
                }
                """
            )
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2003: UserName"], CollectionOrdering.Matching);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a sealed partial record") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().BeEmpty();
        await ( await Fixed<DeclarationCodeFix>(result, "Convert to a sealed partial record") ).Should().Contain("public sealed partial record UserName;");
    }

    // sealed on one declaration seals the type, but abstract on another one would contradict it.
    [Test]
    public async Task Should_Remove_Abstract_From_Every_Declaration_When_Sealing()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public partial record UserName;
            """,
            builder => builder.AddSource(
                "UserName.Label.cs",
                """
                namespace Sample;

                public abstract partial record UserName
                {
                    public string Label => "user";
                }
                """
            )
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2019: UserName"], CollectionOrdering.Matching);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive sealed") ).Should().BeEmpty();
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

            internal static partial class Container
            {
                public partial record Inner
                {
                    [StringPrimitive, NotEmpty]
                    internal readonly partial record struct Nested
                    {
                        public int Length => 0;
                    }
                }
            }
            """
        );

        await Diagnostics(result).Should().BeEmpty();
    }

    // The generator writes the part of a nested domain primitive inside a partial declaration of each type that contains it (CS0260 otherwise).
    [Test]
    public async Task Should_Report_A_Type_That_Contains_A_Domain_Primitive_And_Is_Not_Partial()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            public static class Outer
            {
                public partial record Middle
                {
                    public sealed class Inner
                    {
                        [StringPrimitive, NotEmpty]
                        public readonly partial record struct Code;

                        [Email]
                        public sealed partial record Address;
                    }
                }
            }

            public static class Unrelated
            {
                public sealed class Plain;
            }
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2001: Outer", "LPAD2001: Inner"], CollectionOrdering.Matching);
        await Messages(result, "LPAD2001")
             .Should()
             .BeEquivalentTo(
                  [
                      "'Outer' must be declared 'partial' so the generator can complete the domain primitive 'Code' it contains",
                      "'Inner' must be declared 'partial' so the generator can complete the domain primitive 'Code' it contains",
                  ],
                  CollectionOrdering.Any
              );
        await FixTitles<DeclarationCodeFix>(result).Should().BeEquivalentTo(["Make the containing type partial"], CollectionOrdering.Matching);
        var fixedSources = await FixedAll<DeclarationCodeFix>(result, "Make the containing type partial");
        await fixedSources.Should().Contain(s => s.Contains("public static partial class Outer"));
        await fixedSources.Should().Contain(s => s.Contains("public sealed partial class Inner"));
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the containing type partial") ).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Report_The_Domain_Primitive_With_Its_Own_Message()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public readonly record struct UserName;
            """
        );

        await Messages(result, "LPAD2001").Should().BeEquivalentTo(["'UserName' must be declared 'partial' so the generator can complete it"], CollectionOrdering.Matching);
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
    public async Task Should_Fix_A_Public_Constructor_By_Making_It_Private()
    {
        var result = await Fix<DeclarationAnalyzer, MakeConstructorPrivateCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [Email]
            public sealed partial record EmailAddress
            {
                /// <summary>A comment that stays.</summary>
                public EmailAddress(int length) { }
            }
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2012: EmailAddress"], CollectionOrdering.Matching);
        await ( await Fixed<MakeConstructorPrivateCodeFix>(result, "Make the constructor private") )
             .Should()
             .Contain("    /// <summary>A comment that stays.</summary>\n    private EmailAddress(int length) { }");
        await ( await ErrorsAfterFix<MakeConstructorPrivateCodeFix>(result, "Make the constructor private") ).Should().BeEmpty();
    }

    // The primary constructor of a positional record can't be made private; a constructor with the parameters of the generator's is a
    // duplicate whatever its accessibility.
    [Test]
    public async Task Should_Not_Fix_A_Constructor_That_Cannot_Be_Made_Private()
    {
        var result = await Fix<DeclarationAnalyzer, MakeConstructorPrivateCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public readonly partial record struct UserName(string Value);

            [StrongId<System.Guid>]
            public readonly partial record struct OrderId
            {
                public OrderId(System.Guid value) { }
            }
            """,
            builder => builder.AddSource(
                "OrderId.g.cs",
                """
                // <auto-generated />
                namespace Sample;

                public readonly partial record struct OrderId
                {
                    private OrderId(System.Guid value) { }
                }
                """
            )
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2012: UserName", "LPAD2012: OrderId"], CollectionOrdering.Matching);
        await FixTitles<MakeConstructorPrivateCodeFix>(result).Should().BeEmpty();
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
