using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

namespace Primitives.Tests.Generator;

/// <summary>
///     Smart enums in both forms: a <c>closed partial class</c>, whose members are made with <c>new</c> of a class of their own, which the
///     generator writes, and a <c>readonly partial record struct</c>, whose members are made with <c>new(value, data...)</c>.
/// </summary>
public class SmartEnumGeneratorTests
{
    [Test]
    public async Task Should_Complete_A_Smart_Enum()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class OrderStatus
            {
                public static readonly OrderStatus Pending = new Pending_(1, "Waiting", 0m);
                public static readonly OrderStatus Shipped = new Shipped_(2, "On its way", 5m);
                public static readonly OrderStatus DeliveredToCustomer = new DeliveredToCustomer_(3, "Delivered", 0m);

                public string Label { get; }
                public decimal Fee { get; }

                public virtual bool CanCancel => true;
            }

            public partial class OrderStatus
            {
                public partial class Shipped_
                {
                    public override bool CanCancel => false;
                }
            }

            [SmartEnum<string>]
            public closed partial class Role
            {
                public static readonly Role Admin = new Admin_("admin");
                public static readonly Role User = new User_("user");
            }

            public static class Labels
            {
                public static string Of(OrderStatus status) => status switch
                {
                    OrderStatus.Pending_ => "Waiting",
                    OrderStatus.Shipped_ => "On its way",
                    OrderStatus.DeliveredToCustomer_ => "Delivered",
                };
            }
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_Smart_Enum_With_Field_And_Nullable_Data()
    {
        // Data are get-only auto-properties and readonly fields without an initializer, in declaration order; a member may be a get-only static
        // property. A field whose parameter would be named like the member's value or name gets a parameter of its own name.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class Planet
            {
                public static readonly Planet Mercury = new Mercury_(1, 3.303e+23, null, "first", 0.38m);
                public static Planet Earth { get; } = new Earth_(3, 5.976e+24, "Moon", "third", 1m);

                public readonly double Mass;
                public string? Moon { get; }
                private readonly string name;
                private readonly decimal value;

                public string Ordinal => name;
                public decimal Gravity => value;
                public static Planet Home => Earth;
            }
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_Nested_Smart_Enum()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            public static partial class Catalog
            {
                public partial record Product
                {
                    [SmartEnum<string>]
                    public closed partial class Size
                    {
                        public static readonly Size Small = new Small_("S");
                        public static readonly Size Large = new Large_("L");
                    }
                }
            }

            public static class Sizes
            {
                public static int Of(Catalog.Product.Size size) => size switch
                {
                    Catalog.Product.Size.Small_ => 1,
                    Catalog.Product.Size.Large_ => 2,
                };
            }
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_Members_Named_With_Keywords_And_In_Lower_Case()
    {
        // A member named with a keyword (@class, whose class is class_) and members named in lower case, whose classes are too.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<string>]
            public closed partial class Token
            {
                public static readonly Token @class = new class_("class");
                public static readonly Token lower = new lower_("lower");
                public static readonly Token @event = new @event_("event");
                public static readonly Token Upper = new Upper_("upper");
            }

            public static class Tokens
            {
                public static int Of(Token token) => token switch
                {
                    Token.class_ => 1,
                    Token.lower_ => 2,
                    Token.event_ => 3,
                    Token.Upper_ => 4,
                };

                public static Token Class => Token.@class;
            }
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_Smart_Enum_Of_One_Member_Without_Data()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class Singleton
            {
                public static readonly Singleton Only = new Only_(1);
            }

            public static class Singletons
            {
                public static bool IsOnly(Singleton value) => value switch { Singleton.Only_ => true };
            }
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Warn_About_A_Switch_That_Misses_A_Member()
    {
        // The smart enum is closed: a switch over its members' classes is exhaustive, so one that misses a member is CS8509.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class OrderStatus
            {
                public static readonly OrderStatus Pending = new Pending_(1);
                public static readonly OrderStatus Shipped = new Shipped_(2);
            }

            public static class Labels
            {
                public static string Of(OrderStatus status) => status switch
                {
                    OrderStatus.Pending_ => "Waiting",
                };
            }
            """
        );

        var warnings = result.FinalCompilation.GetDiagnostics().Where(static z => z.Severity >= DiagnosticSeverity.Warning).Select(static z => z.Id).ToArray();
        await warnings.Should().BeEquivalentTo(["CS8509"]);
    }

    [Test]
    public async Task Should_Report_A_Member_Without_A_Class_Of_Its_Own()
    {
        // LPAD2104: a member made with new(), with new of the smart enum itself, with new of a class named after it that isn't nested in the
        // smart enum, with new of a class named after another member (two members naming the same class), and with new of a class not named
        // after it. The smart enum is left alone.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            public sealed class First_;

            [SmartEnum<int>]
            public closed partial class TargetTyped
            {
                public static readonly TargetTyped First = new(1);
            }

            [SmartEnum<int>]
            public closed partial class Itself
            {
                public static readonly Itself First = new Itself(1);
            }

            [SmartEnum<int>]
            public closed partial class Outside
            {
                public static readonly Outside First = new First_();
            }

            [SmartEnum<int>]
            public closed partial class Shared
            {
                public static readonly Shared Left = new Left_(1);
                public static readonly Shared Right = new Left_(2);
            }

            [SmartEnum<int>]
            public closed partial class Misnamed
            {
                public static readonly Misnamed First = new FirstMisnamed(1);
            }
            """
        );

        await Verify(result);
    }

    [Test]
    public async Task Should_Report_Members_That_Share_A_Value()
    {
        // LPAD2105: two members whose value is the same constant. The smart enum is left alone.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<string>]
            public closed partial class Role
            {
                public const string AdminValue = "admin";

                public static readonly Role Admin = new Admin_(AdminValue);
                public static readonly Role User = new User_("user");
                public static readonly Role Root = new Root_("admin");
            }
            """
        );

        await Verify(result);
    }

    [Test]
    public async Task Should_Not_Compare_A_Value_That_Is_Not_A_Constant()
    {
        // Only constants are compared: a value computed when the member is made is the application's to keep apart.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class Level
            {
                public static readonly Level Low = new Low_(1);
                public static readonly Level High = new High_(Levels.Next());
            }

            public static class Levels
            {
                public static int Next() => 1;
            }
            """
        );

        // Without Low_ and High_, which the generator writes, this wouldn't compile.
        await result.ShouldCompileCleanly();
    }

    [Test]
    public async Task Should_Complete_A_Struct_Smart_Enum()
    {
        // A struct can't be closed: its members are its static readonly fields made with new(value), and it gets Match, Map and Switch.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct OrderStatus
            {
                public static readonly OrderStatus Pending = new(1);
                public static readonly OrderStatus Shipped = new(2);
                public static readonly OrderStatus DeliveredToCustomer = new(3);
            }
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_Struct_Smart_Enum_With_Data()
    {
        // A struct smart enum carries data like a closed one: each member gives it after its value, and creating a member from its value
        // returns the member, with its data.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct Size
            {
                public static readonly Size Small = new(1, "S", 0.5m);
                public static readonly Size Large = new(2, "L", 2m);

                public string Label { get; }
                public decimal Weight { get; }
            }
            """
        );

        await ShouldCompileWithTheLayoutSuppressed(result);
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_Nested_Struct_Smart_Enum()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            public static partial class Catalog
            {
                public partial record Product
                {
                    [SmartEnum<string>]
                    public readonly partial record struct Size
                    {
                        public static readonly Size Small = new("S", 1);
                        public static readonly Size Large = new("L", 3);

                        public readonly int Rank;
                    }
                }
            }

            public static class Sizes
            {
                public static int Of(Catalog.Product.Size size) => size.Match(() => 1, () => 2);
                public static int RankOf(string value) => Catalog.Product.Size.Create(value).Rank;
            }
            """
        );

        await ShouldCompileWithTheLayoutSuppressed(result);
        await Verify(result);
    }

    [Test]
    public async Task Should_Leave_A_Smart_Enum_That_Is_A_Record_Class_Alone()
    {
        // A smart enum is a closed partial class or a readonly partial record struct; the analyzers report a record class (LPAD2003), and the
        // generator writes nothing for it.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public sealed partial record Status
            {
                public static readonly Status Active = new(1);
                public static readonly Status Closed = new(2);
            }
            """
        );

        await Assert.That(result.FinalCompilation.SyntaxTrees.Select(static z => Path.GetFileName(z.FilePath)).ToArray()).DoesNotContain("Sample.Status.g.cs");
        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_A_Struct_Smart_Enum_With_Data_Named_Like_Its_Parameters()
    {
        // _label beside Label, _Value beside the member's value and @event, a keyword: each datum gets a parameter of its own, and the
        // constructor assigns @event as a name.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct Tag
            {
                public static readonly Tag First = new(1, "First", "first", 10, "opened");
                public static readonly Tag Second = new(2, "Second", "second", 20, "closed");

                public string Label { get; }
                private readonly string _label;
                private readonly int _Value;
                private readonly string @event;

                public string Lower => _label;
                public int Weight => _Value;
                public string Event => @event;
            }
            """
        );

        await ShouldCompileWithTheLayoutSuppressed(result);
    }

    [Test]
    public async Task Should_Complete_A_Struct_Smart_Enum_With_Members_Named_With_Keywords_And_Properties()
    {
        // Members named with keywords, and members that are get-only static properties with an initializer, as a closed smart enum's can be.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct Token
            {
                public static readonly Token @class = new(1);
                public static readonly Token @event = new(2);
                public static Token Upper { get; } = new(3);

                public static Token Current => Upper;
            }

            public static class Tokens
            {
                public static int Of(Token token) => token.Match(whenclass: () => 1, whenevent: () => 2, whenUpper: () => 3);
                public static int Count => Token.All.Count;
            }
            """
        );

        await result.ShouldCompileCleanly();
    }

    [Test]
    public async Task Should_Take_The_Value_By_Its_Name()
    {
        // The value is the argument named value, wherever it is, else the first one without a name: members whose data are the same and whose
        // values differ share no value.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class Kind
            {
                public static readonly Kind A = new A_(label: "same", value: 1);
                public static readonly Kind B = new B_(label: "same", value: 2);

                public string Label { get; }
            }

            [SmartEnum<int>]
            public readonly partial record struct Shade
            {
                public static readonly Shade Light = new(label: "same", value: 1);
                public static readonly Shade Dark = new(label: "same", value: 2);

                public string Label { get; }
            }
            """
        );

        await ShouldCompileWithTheLayoutSuppressed(result);
    }

    [Test]
    public async Task Should_Report_Members_Whose_Named_Values_Are_The_Same()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class Kind
            {
                public static readonly Kind A = new A_(1, "a");
                public static readonly Kind B = new B_(label: "b", value: 1);

                public string Label { get; }
            }

            [SmartEnum<int>]
            public readonly partial record struct Shade
            {
                public static readonly Shade Light = new(label: "light", value: 1);
                public static readonly Shade Dark = new(1, "dark");

                public string Label { get; }
            }
            """
        );

        await GeneratorDiagnosticsOf(result).Should().BeEquivalentTo(["LPAD2105 Kind's members A and B have the same value", "LPAD2105 Shade's members Light and Dark have the same value"]);
        await GeneratedFilesOf(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Report_Struct_Members_That_Share_A_Value()
    {
        // LPAD2105 for a struct smart enum with data: the same literal, and the same value through a constant.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct Size
            {
                public static readonly Size Small = new(1, "S");
                public static readonly Size Tiny = new(1, "T");

                public string Label { get; }
            }

            [SmartEnum<string>]
            public readonly partial record struct Role
            {
                public const string AdminValue = "admin";

                public static readonly Role Admin = new(AdminValue, 1);
                public static readonly Role Root = new("admin", 2);

                public int Rank { get; }
            }
            """
        );

        await GeneratorDiagnosticsOf(result).Should().BeEquivalentTo(["LPAD2105 Size's members Small and Tiny have the same value", "LPAD2105 Role's members Admin and Root have the same value"]);
        await GeneratedFilesOf(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Not_Compare_A_Struct_Value_That_Is_Not_A_Constant()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct Level
            {
                public static readonly Level Low = new(1);
                public static readonly Level High = new(Levels.Next());
            }

            public static class Levels
            {
                public static int Next() => 1;

                public static int Of(Level level) => level.Match(whenLow: () => 1, whenHigh: () => 2);
            }
            """
        );

        await result.ShouldCompileCleanly();
    }

    [Test]
    public async Task Should_Report_Data_Of_A_Type_Less_Accessible_Than_Internal()
    {
        // Every member's class takes the data in its internal constructor, so a datum of a private type is CS0051 there: LPAD2103, and the
        // smart enum is left alone. A struct smart enum's constructor is private, so it takes one.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class Status
            {
                public static readonly Status Open = new Open_(1, Shade.Light);

                private Shade Color { get; }

                public bool IsLight => Color == Shade.Light;

                private enum Shade { Light, Dark }
            }

            [SmartEnum<int>]
            public readonly partial record struct Level
            {
                public static readonly Level Low = new(1, Shade.Light);

                private readonly Shade _color;

                public bool IsLight => _color == Shade.Light;

                private enum Shade { Light, Dark }
            }
            """
        );

        await GeneratorDiagnosticsOf(result)
           .Should()
           .BeEquivalentTo(["LPAD2103 Status: the data of a smart enum must be of a type at least internal, as every member's class takes it (Color is Sample.Status.Shade)"]);
        await GeneratedFilesOf(result).Should().BeEquivalentTo(["Sample.Level.g.cs"]);
        var problems = result.FinalCompilation.GetDiagnostics().Where(static z => z.Severity == DiagnosticSeverity.Error && z.Location.SourceTree?.FilePath.EndsWith(".g.cs", StringComparison.Ordinal) == true).ToArray();
        await problems.Should().BeEmpty();
    }

    [Test]
    public async Task Should_Accept_A_Member_Class_Named_With_Its_Smart_Enum()
    {
        // new C6.A_(1), new global::Lib.C6.A_(1) and new Alias.A_(1) name the member's own class.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;
            using Alias = Lib.C6;

            namespace Lib;

            [SmartEnum<int>]
            public closed partial class C6
            {
                public static readonly C6 A = new C6.A_(1);
                public static readonly C6 B = new global::Lib.C6.B_(2);
                public static readonly C6 C = new Alias.C_(3);
            }
            """
        );

        await result.ShouldCompileCleanly();
    }

    [Test]
    public async Task Should_Report_A_Member_Class_Named_With_Another_Type()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Lib;

            public static class Other;

            [SmartEnum<int>]
            public closed partial class C6
            {
                public static readonly C6 A = new Other.A_(1);
            }
            """
        );

        await GeneratorDiagnosticsOf(result).Should().BeEquivalentTo(["LPAD2104 C6.A must be made with new A_(value, ...), its own class, which the generator writes"]);
    }

    [Test]
    public async Task Should_Report_Static_Fields_Of_The_Smart_Enum_That_Are_Not_Members()
    {
        // A static field or property of the smart enum's type with an initializer is a member, or an error: not public, not readonly, or
        // with a setter. A computed property isn't a member.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class Status
            {
                public static readonly Status Open = new Open_(1);
                private static readonly Status Hidden = new Hidden_(2);
                public static Status Mutable = new Mutable_(3);

                public static Status Default => Open;
            }

            [SmartEnum<int>]
            public readonly partial record struct Level
            {
                public static readonly Level Low = new(1);
                internal static readonly Level Hidden = new(2);
                public static Level Mutable = new(3);
                public static Level Settable { get; set; } = new(4);

                public static Level Default => Low;
            }
            """
        );

        const string Rule = "isn't a member: a member is a public static readonly field, or a public get-only static property with an initializer";
        await GeneratorDiagnosticsOf(result)
           .Should()
           .BeEquivalentTo(
            [
                $"LPAD2103 Status: Hidden {Rule}",
                $"LPAD2103 Status: Mutable {Rule}",
                $"LPAD2103 Level: Hidden {Rule}",
                $"LPAD2103 Level: Mutable {Rule}",
                $"LPAD2103 Level: Settable {Rule}",
            ]);
        await GeneratedFilesOf(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Report_Data_And_Members_Named_Like_What_The_Generator_Writes()
    {
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class Status
            {
                public static readonly Status Open = new Open_(1, "open");
                public static readonly Status All = new All_(2, "all");

                public string Name { get; }
            }

            [SmartEnum<int>]
            public readonly partial record struct Level
            {
                public static readonly Level Low = new(1, true, "low");
                public static readonly Level Description = new(2, false, "description");

                public bool IsDefault { get; }
                private readonly string _value;

                public string Text => _value;
            }
            """
        );

        const string Rule = "is a name the generator writes into the smart enum, so neither a member nor its data can have it";
        await GeneratorDiagnosticsOf(result)
           .Should()
           .BeEquivalentTo(
            [
                $"LPAD2103 Status: All {Rule}",
                $"LPAD2103 Status: Name {Rule}",
                $"LPAD2103 Level: Description {Rule}",
                $"LPAD2103 Level: IsDefault {Rule}",
                $"LPAD2103 Level: _value {Rule}",
            ]);
        await GeneratedFilesOf(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Leave_A_Class_That_Is_Not_Closed_Or_Not_A_Smart_Enum_Alone()
    {
        // A smart enum that is a partial class but not closed (LPAD2019), and a [StringPrimitive] declared as a sealed partial class, which
        // isn't a record (LPAD2003): the generator writes nothing for either.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public partial class Open
            {
                public static readonly Open First = new(1);

                private Open(int value) { }
            }

            [StringPrimitive]
            public sealed partial class Label;
            """
        );

        await GeneratorDiagnosticsOf(result).Should().BeEmpty();
        await GeneratedFilesOf(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Complete_A_Smart_Enum_With_An_Abstract_Property_Each_Member_Overrides()
    {
        // An abstract property isn't data: each member's class overrides it in a part of its own.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class Shape
            {
                public static readonly Shape Square = new Square_(1, "square");
                public static readonly Shape Circle = new Circle_(2, "circle");

                public string Label { get; }

                public abstract int Corners { get; }

                public partial class Square_
                {
                    public override int Corners => 4;
                }

                public partial class Circle_
                {
                    public override int Corners => 0;
                }
            }

            public static class Shapes
            {
                public static int CornersOf(int value) => Shape.FromValue(value).Corners;
            }
            """
        );

        await result.ShouldCompileCleanly();
    }

    [Test]
    public async Task Should_Report_Members_Whose_Values_Are_The_Same_Once_Converted_To_The_Value_Type()
    {
        // 1 is an int and 1L a long, but both are the long 1 of a SmartEnum<long>: FromValue would throw when it builds its lookup.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<long>]
            public closed partial class Wide
            {
                public static readonly Wide A = new A_(1);
                public static readonly Wide B = new B_(1L);
            }

            [SmartEnum<long>]
            public readonly partial record struct Tall
            {
                public static readonly Tall Low = new(1);
                public static readonly Tall High = new(1L);
            }

            [SmartEnum<double>]
            public readonly partial record struct Ratio
            {
                public static readonly Ratio Whole = new(1);
                public static readonly Ratio One = new(1.0);
            }
            """
        );

        await GeneratorDiagnosticsOf(result)
           .Should()
           .BeEquivalentTo(
            [
                "LPAD2105 Wide's members A and B have the same value",
                "LPAD2105 Tall's members Low and High have the same value",
                "LPAD2105 Ratio's members Whole and One have the same value",
            ]);
        await GeneratedFilesOf(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Report_A_Struct_Member_That_Is_An_Alias_Of_Another()
    {
        // A struct member is made with new(...) or new Size(...): an alias of another member, as a field or a get-only property with an
        // initializer, would be a member of its own, with the same value (LPAD2103). A computed property (Default => Small) is the alias.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct Size
            {
                public static readonly Size Small = new(1);
                public static readonly Size Large = new Size(2);
                public static Size Default { get; } = Small;
                public static readonly Size Alias = Small;
            }

            [SmartEnum<int>]
            public readonly partial record struct Weight
            {
                public static readonly Weight Light = new(1);
                public static readonly Weight Heavy = new Weight(2);

                public static Weight Default => Light;
            }
            """
        );

        await GeneratorDiagnosticsOf(result)
           .Should()
           .BeEquivalentTo(
            [
                "LPAD2103 Size: Default isn't made with new(value, ...), as a member is: an alias of another member must be a computed property (Default => Small)",
                "LPAD2103 Size: Alias isn't made with new(value, ...), as a member is: an alias of another member must be a computed property (Alias => Small)",
            ]);
        await GeneratedFilesOf(result).Should().BeEquivalentTo(["Sample.Weight.g.cs"]);
        var problems = result.FinalCompilation.GetDiagnostics().Where(static z => z.Severity == DiagnosticSeverity.Error && z.Location.SourceTree?.FilePath.EndsWith(".g.cs", StringComparison.Ordinal) == true).ToArray();
        await problems.Should().BeEmpty();
    }

    [Test]
    public async Task Should_Report_A_Closed_Member_That_Is_An_Alias_Of_Another()
    {
        // A closed smart enum's member is made with new of its own class already, so an alias that isn't a computed property is LPAD2104.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public closed partial class Size
            {
                public static readonly Size Small = new Small_(1);
                public static Size Default { get; } = Small;
                public static readonly Size Alias = Small;
            }
            """
        );

        await GeneratorDiagnosticsOf(result)
           .Should()
           .BeEquivalentTo(
            [
                "LPAD2104 Size.Default must be made with new Default_(value, ...), its own class, which the generator writes",
                "LPAD2104 Size.Alias must be made with new Alias_(value, ...), its own class, which the generator writes",
            ]);
        await GeneratedFilesOf(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Report_Data_Of_A_Private_Type_Of_The_Type_A_Protected_Smart_Enum_Is_Nested_In()
    {
        // A protected or private protected smart enum is seen by the classes that derive from Outer, which can't see Outer's private Hidden:
        // the internal constructor of each member's class would be CS0051. A private smart enum is seen only inside Outer, where Hidden is.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            public partial class Outer
            {
                private sealed class Hidden;

                [SmartEnum<int>]
                protected closed partial class Exposed
                {
                    public static readonly Exposed A = new A_(1, null);

                    private readonly Hidden? _hidden;
                }

                [SmartEnum<int>]
                private protected closed partial class Shared
                {
                    public static readonly Shared A = new A_(1, null);

                    private readonly Hidden? _hidden;
                }

                [SmartEnum<int>]
                private closed partial class Inside
                {
                    public static readonly Inside A = new A_(1, null);

                    private readonly Hidden? _hidden;

                    public bool IsHidden => _hidden is not null;
                }
            }
            """
        );

        await GeneratorDiagnosticsOf(result)
           .Should()
           .BeEquivalentTo(
            [
                "LPAD2103 Exposed: the data of a smart enum must be of a type at least internal, as every member's class takes it (_hidden is Sample.Outer.Hidden?)",
                "LPAD2103 Shared: the data of a smart enum must be of a type at least internal, as every member's class takes it (_hidden is Sample.Outer.Hidden?)",
            ]);
        await GeneratedFilesOf(result).Should().BeEquivalentTo(["Sample.Outer.Inside.g.cs"]);
        var problems = result.FinalCompilation.GetDiagnostics().Where(static z => z.Severity == DiagnosticSeverity.Error && z.Location.SourceTree?.FilePath.EndsWith(".g.cs", StringComparison.Ordinal) == true).ToArray();
        await problems.Should().BeEmpty();
    }

    [Test]
    public async Task Should_Complete_A_Struct_Smart_Enum_With_A_Member_Named_Like_A_Type_Parameter()
    {
        // Inside Match<TResult> and Map<TResult>, TResult is the type parameter: the generated code names the member through its smart enum.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct Outcome
            {
                public static readonly Outcome TResult = new(1);
                public static readonly Outcome Other = new(2);
            }

            public static class Outcomes
            {
                public static int Of(Outcome outcome) =>
                    outcome.Match(whenTResult: () => 1, whenOther: () => 2) + outcome.Map(whenTResult: z => z.Value, whenOther: z => 0);
            }
            """
        );

        await result.ShouldCompileCleanly();
    }

    private static string[] GeneratorDiagnosticsOf(GeneratorTestResults result) =>
    [
        .. result.Results.Values.SelectMany(static z => z.Diagnostics).Select(static z => $"{z.Id} {z.GetMessage(System.Globalization.CultureInfo.InvariantCulture)}"),
    ];

    private static string[] GeneratedFilesOf(GeneratorTestResults result) =>
    [
        .. result.Results.Values.SelectMany(static z => z.SyntaxTrees)
                 .Select(static z => Path.GetFileName(z.FilePath))
                 .Where(static z => z is not ( GeneratorTestBuilder.AttributesFile or GeneratorTestBuilder.EmbeddedAttributeFile )),
    ];

    // CS0282 (no defined order between the fields of the two parts of a struct with data) is reported on whichever part comes first, and
    // suppressed by SmartEnumLayoutSuppressor, as the build of an application does: nothing else is left.
    private static async Task ShouldCompileWithTheLayoutSuppressed(GeneratorTestResults result)
    {
        var diagnostics = await result.FinalCompilation
                                      .WithAnalyzers(
                                           [new SmartEnumLayoutSuppressor()],
                                           new CompilationWithAnalyzersOptions(null!, null, false, false, reportSuppressedDiagnostics: true)
                                       )
                                      .GetAllDiagnosticsAsync(TestContext.Current!.Execution.CancellationToken);
        var problems = diagnostics.Where(static z => z.Severity >= DiagnosticSeverity.Warning && !z.IsSuppressed && z.Id is not ( "CS1701" or "CS1702" ))
                                  .Select(static z => z.ToString())
                                  .ToArray();
        await Assert.That(problems).IsEmpty();
        await Assert.That(diagnostics.Where(static z => z.IsSuppressed).Select(static z => z.Id).ToArray()).IsEquivalentTo(["CS0282"]);
    }

    [Test]
    public async Task Should_Accept_Protected_Data_Of_A_Protected_Smart_Enum()
    {
        // Protected data seen wherever the protected smart enum is: the compiler accepts the member classes' internal constructors, so the
        // generator doesn't report them.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            public partial class Outer
            {
                protected enum Tier { One, Two }

                [SmartEnum<int>]
                protected closed partial class Level
                {
                    public static readonly Level Low = new Low_(1, Tier.One);
                    public static readonly Level High = new High_(2, Tier.Two);

                    public Tier Tier { get; }
                }
            }
            """
        );

        await GeneratorDiagnosticsOf(result).Should().BeEmpty();
        await GeneratedFilesOf(result).Should().BeEquivalentTo(["Sample.Outer.Level.g.cs"]);
        await result.ShouldCompileCleanly();
    }

    [Test]
    public async Task Should_Complete_Smart_Enums_Of_Value_Types_A_Constant_Converts_To_Only_In_Code()
    {
        // Int128 and Half have no SpecialType: their members' constants are compared as written, and the generator doesn't fail.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<System.Int128>]
            public closed partial class Huge
            {
                public static readonly Huge One = new One_(1);
                public static readonly Huge Two = new Two_(2);
            }

            [SmartEnum<System.Half>]
            public readonly partial record struct Small
            {
                public static readonly Small One = new((System.Half)1);
                public static readonly Small Two = new((System.Half)2);
            }
            """
        );

        await GeneratorDiagnosticsOf(result).Should().BeEmpty();
        await GeneratedFilesOf(result).Should().BeEquivalentTo(["Sample.Huge.g.cs", "Sample.Small.g.cs"]);
    }

    [Test]
    public async Task Should_Report_A_Char_And_A_Number_Of_The_Same_Value()
    {
        // C# converts 'a' to 97 in a double: the two members have the same value.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<double>]
            public closed partial class Score
            {
                public static readonly Score A = new A_('a');
                public static readonly Score B = new B_(97);
            }

            [SmartEnum<double>]
            public readonly partial record struct Mark
            {
                public static readonly Mark A = new('a');
                public static readonly Mark B = new(97);
            }
            """
        );

        await GeneratorDiagnosticsOf(result)
           .Should()
           .BeEquivalentTo(["LPAD2105 Score's members A and B have the same value", "LPAD2105 Mark's members A and B have the same value"]);
        await GeneratedFilesOf(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Report_A_Struct_Member_Made_Without_A_Value()
    {
        // new() and new Size() are the default, which is none of the members.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public readonly partial record struct Size
            {
                public static readonly Size Small = new(1);
                public static readonly Size None = new();
                public static readonly Size Empty = new Size();
            }
            """
        );

        await GeneratorDiagnosticsOf(result)
           .Should()
           .BeEquivalentTo(
            [
                "LPAD2103 Size: None isn't made with new(value, ...), as a member is",
                "LPAD2103 Size: Empty isn't made with new(value, ...), as a member is",
            ]);
        await GeneratedFilesOf(result).Should().BeEmpty();
    }
}
