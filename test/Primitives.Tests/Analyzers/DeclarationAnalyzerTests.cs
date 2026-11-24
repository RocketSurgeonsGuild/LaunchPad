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

        await VerifyAnalysis(result);
        await result.FinalDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.ToString()).Should().BeEmpty();
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
        // Without partial, the generator doesn't complete the domain primitive; with it, it writes the readonly part.
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive readonly") )
           .Should()
           .Contain("The generator did not complete the domain primitive 'Sample.UserName'.");
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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
        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a sealed partial record") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().BeEmpty();
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the containing type partial") ).Should().BeEmpty();
    }

    // A domain primitive that contains another is reported once, for itself: its fix makes it partial, which completes both.
    [Test]
    public async Task Should_Report_A_Domain_Primitive_That_Contains_Another_Only_For_Itself()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [ValueObject]
            public sealed record Address
            {
                public required City Town { get; init; }

                [StringPrimitive, NotEmpty]
                public readonly partial record struct City;
            }

            [ValueObject]
            public sealed class Contact
            {
                public required Name FullName { get; init; }

                [StringPrimitive, NotEmpty]
                public readonly partial record struct Name;
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive partial", "Convert to a sealed partial record") ).Should().BeEmpty();
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
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

        await VerifyAnalysis(result);
    }
}
