using Microsoft.CodeAnalysis;

using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>
///     LPAD2001 (partial, and the types that contain it), LPAD2002 (readonly), LPAD2003 (record, or a closed class or a record struct for a
///     smart enum), LPAD2012 (public constructor) and LPAD2019 (sealed, or closed for a smart enum class), with their fixes. Every fixed project
///     is compiled with the generator.
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

    // A smart enum may be a closed partial class; closed may be on any of its declarations. The application's own part of a member's class,
    // where it changes behaviour, isn't a domain primitive of its own.
    [Test]
    public async Task Should_Accept_A_Smart_Enum_Declared_As_A_Closed_Partial_Class()
    {
        var result = await AnalyzeWithGenerator<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class OrderStatus
            {
                public static readonly OrderStatus Pending = new Pending_(1, "Waiting");
                public static readonly OrderStatus Shipped = new Shipped_(2, "On its way");

                public string Label { get; }

                public virtual bool CanCancel => true;

                public partial class Shipped_
                {
                    internal Shipped_(string label) : this(2, label) { }

                    public override bool CanCancel => false;
                }
            }

            [SmartEnum<string>]
            public partial class Role
            {
                public static readonly Role Admin = new Admin_("admin");
            }

            public closed partial class Role;
            """
        );

        await VerifyAnalysis(result);
        await result.FinalDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.ToString()).Should().BeEmpty();
    }

    // A smart enum that is a record class is neither form: its fixes make it a closed partial class or a readonly partial record struct, never a
    // sealed record, and make each member as that form makes it (new Low_(1) in the class, new(1) in the struct).
    [Test]
    public async Task Should_Report_A_Smart_Enum_That_Is_A_Record()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public sealed partial record Level
            {
                public static readonly Level Low = new(1), Medium = new Level(2);
                public static Level High { get; } = new(3);
            }

            [SmartEnum<string>]
            public closed partial record Role
            {
                public static readonly Role Admin = new Admin_("admin", 10);

                public int Rank { get; }
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a closed partial class") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().BeEmpty();
        // Compiling isn't enough: a member the fix dropped, or one the generator no longer reads as a member (High, a property), compiles too.
        foreach (var title in (string[])["Convert to a closed partial class", "Convert to a readonly partial record struct"])
        {
            await ( await MembersAfterFix<DeclarationCodeFix>(result, "Sample.Level", title) ).Should().BeEquivalentTo(["Low", "Medium", "High"]);
            await ( await MembersAfterFix<DeclarationCodeFix>(result, "Sample.Role", title) ).Should().BeEquivalentTo(["Admin"]);
        }
    }

    [Test]
    public async Task Should_Report_A_Smart_Enum_That_Is_A_Struct_But_Not_A_Record()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public struct Size
            {
                public static readonly Size Small = new(1, "S");

                public string Label { get; }
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a closed partial class") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().BeEmpty();
    }

    // The struct form: a readonly partial record struct whose members are values of it, made with their value and then their data, or with
    // their value alone. readonly may be on any of its declarations.
    [Test]
    public async Task Should_Accept_A_Smart_Enum_Declared_As_A_Readonly_Partial_Record_Struct()
    {
        var result = await AnalyzeWithGenerator<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct OrderStatus
            {
                public static readonly OrderStatus Pending = new(1, "Waiting", 0m);
                public static readonly OrderStatus Shipped = new(2, "On its way", 5m);

                public string Label { get; }

                public decimal Fee { get; }

                public bool CanCancel => this == Pending;
            }

            [SmartEnum<string>]
            public partial record struct Role
            {
                public static readonly Role Admin = new("admin");
            }

            public readonly partial record struct Role;
            """
        );

        await VerifyAnalysis(result);
        await result.FinalDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.ToString()).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Report_A_Struct_Smart_Enum_That_Is_Not_Readonly()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public partial record struct Level
            {
                public static readonly Level Low = new(1);
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive readonly") ).Should().BeEmpty();
    }

    // A public constructor of a struct smart enum makes values that aren't among its members, as it lets invalid values into any other kind.
    [Test]
    public async Task Should_Report_A_Public_Constructor_Of_A_Struct_Smart_Enum()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct Level
            {
                public static readonly Level Low = new(1);

                public Level(string name) { }
            }
            """
        );

        await VerifyAnalysis(result);
    }

    // closed on one declaration closes the type, but abstract or sealed on another one would contradict it (CS9381, CS9384).
    [Test]
    public async Task Should_Report_A_Smart_Enum_That_Is_Not_Closed()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public partial class Level
            {
                public static readonly Level Low = new Low_(1);
            }

            [SmartEnum<int>]
            public sealed partial class Size
            {
                public static readonly Size Small = new Small_(1);
            }
            """,
            builder => builder.AddSource(
                "Level.Rank.cs",
                """
                namespace Sample;

                public abstract partial class Level
                {
                    public int Rank => 0;
                }
                """
            )
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the smart enum closed") ).Should().BeEmpty();
    }

    // A closed class makes each member with new of its own class: making the smart enum closed makes each member that way too, in every part,
    // or the generator wouldn't complete it (LPAD2104) and new(1) would make an instance of an abstract class (CS0144).
    [Test]
    public async Task Should_Make_Each_Member_With_Its_Own_Class_When_Closing_A_Smart_Enum()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public partial class Level
            {
                public static readonly Level Low = new(1), Medium = new Level(2);
                public static Level High { get; } = new(3);
            }
            """,
            builder => builder.AddSource(
                "Level.Top.cs",
                """
                namespace Sample;

                public sealed partial class Level
                {
                    public static readonly Level Top = new Level(4);
                }
                """
            )
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the smart enum closed") ).Should().BeEmpty();
        await ( await MembersAfterFix<DeclarationCodeFix>(result, "Sample.Level", "Make the smart enum closed") )
           .Should()
           .BeEquivalentTo(["Low", "Medium", "High", "Top"]);
    }

    // A member is a static field or property of the smart enum's type however that type is spelled, and new Sample.Level(2) makes the smart
    // enum as new Level(2) does: making it closed makes each of them with new of its own class, or the generator wouldn't complete it (LPAD2104)
    // and new Sample.Level(2) would make an instance of an abstract class (CS0144).
    [Test]
    public async Task Should_Make_Each_Member_With_Its_Own_Class_However_The_Smart_Enum_Is_Spelled()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;
            using Alias = Sample.Level;

            namespace Sample;

            [SmartEnum<int>]
            public partial class Level
            {
                public static readonly Sample.Level Low = new(1);
                public static readonly global::Sample.Level Medium = new Sample.Level(2);
                public static Level High { get; } = new global::Sample.Level(3);
                public static readonly Alias Top = new Alias(4);
            }

            public static partial class Outer
            {
                [SmartEnum<int>]
                public partial class Level
                {
                    public static readonly Outer.Level Low = new Outer.Level(1);
                    public static readonly Level High = new(2);
                }
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the smart enum closed") ).Should().BeEmpty();
        await ( await MembersAfterFix<DeclarationCodeFix>(result, "Sample.Level", "Make the smart enum closed") )
           .Should()
           .BeEquivalentTo(["Low", "Medium", "High", "Top"]);
        await ( await MembersAfterFix<DeclarationCodeFix>(result, "Sample.Outer+Level", "Make the smart enum closed") ).Should().BeEquivalentTo(["Low", "High"]);
    }

    // The same spellings in a smart enum that is a record: either form its fixes give it keeps every member.
    [Test]
    public async Task Should_Keep_Every_Member_However_The_Smart_Enum_Is_Spelled_When_Converting_A_Record()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public sealed partial record Level
            {
                public static readonly Sample.Level Low = new(1);
                public static readonly global::Sample.Level Medium = new Sample.Level(2);
                public static Level High { get; } = new global::Sample.Level(3);
            }

            public static partial class Outer
            {
                [SmartEnum<int>]
                public sealed partial record Level
                {
                    public static readonly Outer.Level Low = new Outer.Level(1);
                }
            }
            """
        );

        await VerifyAnalysis(result);
        foreach (var title in (string[])["Convert to a closed partial class", "Convert to a readonly partial record struct"])
        {
            await ( await ErrorsAfterFix<DeclarationCodeFix>(result, title) ).Should().BeEmpty();
            await ( await MembersAfterFix<DeclarationCodeFix>(result, "Sample.Level", title) ).Should().BeEquivalentTo(["Low", "Medium", "High"]);
            await ( await MembersAfterFix<DeclarationCodeFix>(result, "Sample.Outer+Level", title) ).Should().BeEquivalentTo(["Low"]);
        }
    }

    // A record struct makes each member with new(...): converting to one turns new Low_(1) back into new(1) however the member's class is
    // named (Level.Low_, Sample.Level.Low_, global::Sample.Level.Low_), or the class no longer exists (CS0426).
    [Test]
    public async Task Should_Make_Each_Member_With_New_However_Its_Class_Is_Named_When_Converting_To_A_Record_Struct()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial record Level
            {
                public static readonly Level Low = new Level.Low_(1);
                public static readonly Sample.Level Medium = new Sample.Level.Medium_(2);
                public static Level High { get; } = new global::Sample.Level.High_(3);
                public static readonly Level Top = new Top_(4);
            }
            """
        );

        await VerifyAnalysis(result);
        foreach (var title in (string[])["Convert to a closed partial class", "Convert to a readonly partial record struct"])
        {
            await ( await ErrorsAfterFix<DeclarationCodeFix>(result, title) ).Should().BeEmpty();
            await ( await MembersAfterFix<DeclarationCodeFix>(result, "Sample.Level", title) ).Should().BeEquivalentTo(["Low", "Medium", "High", "Top"]);
        }
    }

    // Level? is the smart enum in a class, but Nullable<Level> in a record struct, which isn't a member: the conversion drops the `?`.
    [Test]
    public async Task Should_Keep_A_Member_Typed_With_A_Nullable_Annotation_When_Converting_To_A_Record_Struct()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            #nullable enable
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public sealed partial record Level
            {
                public static readonly Level? Low = new(1);
                public static Level? High { get; } = new Level(2);
            }
            """
        );

        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a readonly partial record struct") ).Should().BeEmpty();
        await ( await MembersAfterFix<DeclarationCodeFix>(result, "Sample.Level", "Convert to a readonly partial record struct") ).Should().BeEquivalentTo(["Low", "High"]);
    }

    [Test]
    public async Task Should_Report_A_Smart_Enum_That_Is_Not_Partial()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed class Level
            {
                public static readonly Level Low = new Low_(1);
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Make the domain primitive partial") ).Should().BeEmpty();
    }

    // Only a smart enum is a closed class: any other kind is a record, and its fixes take closed away.
    [Test]
    public async Task Should_Report_Another_Kind_Declared_Closed()
    {
        var result = await Fix<DeclarationAnalyzer, DeclarationCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public closed partial class UserName;

            [Email]
            public closed partial record EmailAddress;
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a sealed partial record", "Make the domain primitive sealed") ).Should().BeEmpty();
        await ( await ErrorsAfterFix<DeclarationCodeFix>(result, "Convert to a readonly partial record struct", "Make the domain primitive sealed") ).Should().BeEmpty();
    }

    // A public constructor of the application's part of a member's class makes members that aren't among the smart enum's; the generator's
    // are internal. One of the smart enum itself creates nothing: a closed class is abstract.
    [Test]
    public async Task Should_Report_A_Public_Constructor_Of_A_Member_Class()
    {
        var result = await AnalyzeWithGenerator<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class OrderStatus
            {
                public static readonly OrderStatus Pending = new Pending_(1, "Waiting");
                public static readonly OrderStatus Shipped = new Shipped_(2, "On its way");

                public OrderStatus() : this(0, "", "") { }

                public string Label { get; }

                public partial class Shipped_
                {
                    public Shipped_() : this(2, "On its way") { }

                    internal Shipped_(string label) : this(2, label) { }
                }

                public sealed class Helper
                {
                    public Helper() { }
                }
            }
            """
        );

        await VerifyAnalysis(result);
    }

    // Before the generator has completed it, the class of a member is known by its name: the member's with an underscore. Another nested class
    // whose name ends with one isn't a member's.
    [Test]
    public async Task Should_Report_A_Public_Constructor_Of_A_Member_Class_Before_The_Generator_Runs()
    {
        var result = await Analyze<DeclarationAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class OrderStatus
            {
                public static readonly OrderStatus Shipped = new Shipped_(2);

                public partial class Shipped_
                {
                    public Shipped_() { }
                }

                public sealed class Other_
                {
                    public Other_() { }
                }
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
