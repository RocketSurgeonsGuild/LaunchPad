using Microsoft.CodeAnalysis;

namespace Primitives.Tests.Generator;

/// <summary>
///     Smart enums of an int and of a string.
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
            public readonly partial record struct OrderStatus
            {
                public static readonly OrderStatus Pending = new(1);
                public static readonly OrderStatus Shipped = new(2);
                public static readonly OrderStatus DeliveredToCustomer = new(3);
            }

            [SmartEnum<string>]
            public sealed partial record Role
            {
                public static readonly Role Admin = new("admin");
                public static readonly Role User = new("user");
            }
            """
        );

        await result.ShouldCompileCleanly();
        await Verify(result);
    }

    [Test]
    public async Task Should_Warn_About_A_Switch_That_Misses_A_Member()
    {
        // A smart enum is a C# 15 union of its case types, so the compiler checks a switch over them: CS8509 names the member it misses.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public sealed partial record OrderStatus
            {
                public static readonly OrderStatus Pending = new(1);
                public static readonly OrderStatus Shipped = new(2);
            }

            public static class Labels
            {
                public static string Complete(OrderStatus status) => status switch
                {
                    OrderStatus.Cases.Pending => "Waiting",
                    OrderStatus.Cases.Shipped => "On its way",
                };

                public static string Incomplete(OrderStatus status) => status switch
                {
                    OrderStatus.Cases.Pending => "Waiting",
                };

                public static OrderStatus FromCase(OrderStatus.Cases.Shipped shipped) => shipped;
            }
            """
        );

        var problems = result.FinalCompilation
                             .GetDiagnostics()
                             .Where(static z => z.Severity >= DiagnosticSeverity.Warning && z.Id is not ( "CS1701" or "CS1702" ))
                             .Select(static z => $"{z.Id} {z.Location.GetLineSpan().StartLinePosition.Line + 1}: {z.GetMessage(System.Globalization.CultureInfo.InvariantCulture)}")
                             .ToArray();
        await Verify(problems);
    }

    [Test]
    public async Task Should_Report_A_Member_Named_After_The_Union_Members()
    {
        // LPAD2104: the type is left alone.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public sealed partial record Shape
            {
                public static readonly Shape Cases = new(1);
            }

            [SmartEnum<int>]
            public readonly partial record struct Size
            {
                public static readonly Size Small = new(1);

                public interface IUnionMembers;
            }

            [SmartEnum<int>]
            public sealed partial record Cases
            {
                public static readonly Cases Civil = new(1);
            }
            """
        );

        await Verify(result);
    }

    [Test]
    public async Task Should_Complete_Members_Named_With_Keywords()
    {
        // A case type is named after its member, so a member in lower case or named with a keyword (one a type can't be named without @)
        // still compiles.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<string>]
            public sealed partial record Word
            {
                public static readonly Word red = new("red");
                public static readonly Word @class = new("class");
                public static readonly Word file = new("file");
                public static readonly Word scoped = new("scoped");
                public static readonly Word required = new("required");
                public static readonly Word extension = new("extension");
                public static readonly Word record = new("record");
                public static readonly Word @var = new("var");
            }

            public static class Labels
            {
                public static int Of(Word word) => word switch
                {
                    Word.Cases.red => 1,
                    Word.Cases.@class => 2,
                    Word.Cases.@file => 3,
                    Word.Cases.@scoped => 4,
                    Word.Cases.@required => 5,
                    Word.Cases.@extension => 6,
                    Word.Cases.@record => 7,
                    Word.Cases.@var => 8,
                };
            }
            """
        );

        await result.ShouldCompileCleanly();
    }

    [Test]
    public async Task Should_Report_Members_That_Share_A_Value()
    {
        // LPAD2105: the type is left alone. Members made from values that aren't constants aren't compared.
        var result = await GeneratorTestBuilder.Generate(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [SmartEnum<int>]
            public sealed partial record Priority
            {
                public static readonly Priority High = new(1);
                public static readonly Priority Urgent = new(1);
                public static readonly Priority Low = new(2);
            }

            [SmartEnum<string>]
            public sealed partial record Region
            {
                private const string Default = "eu";
                public static readonly Region Europe = new(Default);
                public static readonly Region Primary = new("eu");
            }

            [SmartEnum<int>]
            public sealed partial record Computed
            {
                public static readonly Computed One = new(System.Environment.ProcessorCount);
                public static readonly Computed Two = new(System.Environment.ProcessorCount + 1);
            }
            """
        );

        await Verify(result);
    }
}
