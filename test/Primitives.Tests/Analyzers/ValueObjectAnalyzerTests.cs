using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>LPAD2008 (init) and LPAD2018 (mutable collection) on value objects.</summary>
public class ValueObjectAnalyzerTests
{
    [Test]
    public async Task Should_Report_A_Property_With_A_Setter()
    {
        var result = await Analyze<ValueObjectAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [ValueObject]
            public readonly partial record struct Address
            {
                public string Street { get; set; }
                public string City { get; init; }
            }

            [ValueObject]
            public sealed partial record Contact
            {
                public required string Name { get; set; }
            }
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2008: Street", "LPAD2008: Name"], CollectionOrdering.Matching);
        await Messages(result, "LPAD2008").Should().Contain("Property 'Street' of value object 'Address' must use 'init' instead of 'set'");
    }

    [Test]
    public async Task Should_Fix_A_Setter_By_Making_It_Init()
    {
        var result = await Fix<ValueObjectAnalyzer, UseInitCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [ValueObject]
            public sealed partial record Address
            {
                public required string Street { get; set; }
                public string City { get; private set; } = "";
                public string Note
                {
                    get => field;
                    set => field = value.Trim();
                } = "";
            }
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2008: Street", "LPAD2008: City", "LPAD2008: Note"], CollectionOrdering.Matching);
        var fixedSources = await FixedAll<UseInitCodeFix>(result, "Use 'init' instead of 'set'");
        await fixedSources.Should().Contain(s => s.Contains("public required string Street { get; init; }"));
        await fixedSources.Should().Contain(s => s.Contains("public string City { get; private init; } = \"\";"));
        await fixedSources.Should().Contain(s => s.Contains("init => field = value.Trim();"));
        await ( await ErrorsAfterFix<UseInitCodeFix>(result, "Use 'init' instead of 'set'") ).Should().BeEmpty();
    }

    // An init accessor can be called where the object is initialized: its constructors and init accessors on this, an object initializer, a
    // with expression.
    [Test]
    public async Task Should_Fix_A_Setter_Assigned_Only_Where_The_Object_Is_Initialized()
    {
        var result = await Fix<ValueObjectAnalyzer, UseInitCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [ValueObject]
            public sealed partial record Address
            {
                public string Street { get; set; } = "";
                public string City { get; set; } = "";
                public string Line
                {
                    get => $"{Street}, {City}";
                    init => (Street, this.City) = (value, value);
                }
            }

            public static class Addresses
            {
                public static Address Create(string street) => new() { Street = street, City = "Paris" };
                public static Address Move(Address address, string city) => address with { City = city };
            }
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2008: Street", "LPAD2008: City"], CollectionOrdering.Matching);
        await ( await FixedAll<UseInitCodeFix>(result, "Use 'init' instead of 'set'") ).Should().HaveCount(2);
        await ( await ErrorsAfterFix<UseInitCodeFix>(result, "Use 'init' instead of 'set'") ).Should().BeEmpty();
    }

    // With init, the assignment in a method, a lambda in the constructor or a constructor of another type would not compile (CS8852).
    [Test]
    public async Task Should_Not_Fix_A_Setter_Assigned_After_The_Object_Is_Initialized()
    {
        var result = await Fix<ValueObjectAnalyzer, UseInitCodeFix>(
            """
            using System;
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [ValueObject]
            public sealed partial record Address
            {
                public string Street { get; set; } = "";
                public string City { get; private set; } = "";
                public string Country { get; set; } = "";
                public string Region { get; set; } = "";

                public void Move(string city) => City = city;
            }

            public sealed class Mover
            {
                public Mover(Address address)
                {
                    address.Country = "France";
                    Action<string> rename = name => address.Region = name;
                }
            }
            """
        );

        await Diagnostics(result)
             .Should()
             .BeEquivalentTo(["LPAD2008: Street", "LPAD2008: City", "LPAD2008: Country", "LPAD2008: Region"], CollectionOrdering.Matching);
        var fixedSources = await FixedAll<UseInitCodeFix>(result, "Use 'init' instead of 'set'");
        await fixedSources.Should().HaveCount(1);
        await fixedSources[0].Should().Contain("public string Street { get; init; }");
        await ( await ErrorsAfterFix<UseInitCodeFix>(result, "Use 'init' instead of 'set'") ).Should().BeEmpty();
    }

    // A property of a record's parameter has no accessor to change.
    [Test]
    public async Task Should_Not_Fix_A_Property_Declared_By_A_Parameter()
    {
        var result = await Fix<ValueObjectAnalyzer, UseInitCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [ValueObject]
            public partial record struct Point(int X, int Y);
            """
        );

        await FixTitles<UseInitCodeFix>(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Accept_Immutable_Properties()
    {
        var result = await Analyze<ValueObjectAnalyzer>(
            """
            using System.Collections.Immutable;
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [ValueObject]
            public sealed partial record Address
            {
                public required string Street { get; init; }
                public string Display => Street;
                public ImmutableArray<string> Lines { get; init; }
                public System.Collections.Generic.IReadOnlyList<string> Tags { get; init; } = [];
                public static string Country { get; set; } = "";
                private string Secret { get; set; } = "";
                internal string Internal { get; set; } = "";
            }
            """
        );

        await Diagnostics(result).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Ignore_Types_That_Are_Not_Value_Objects()
    {
        var result = await Analyze<ValueObjectAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [StringPrimitive, NotEmpty]
            public sealed partial record UserName
            {
                public string[] Parts { get; set; } = [];
            }

            public record struct Plain(string[] Parts)
            {
                public string Name { get; set; }
            }
            """
        );

        await Diagnostics(result).Should().BeEmpty();
    }

    [Test]
    [Arguments("string[]")]
    [Arguments("List<string>")]
    [Arguments("Dictionary<string, int>")]
    [Arguments("HashSet<string>")]
    [Arguments("Queue<string>")]
    [Arguments("Stack<string>")]
    [Arguments("LinkedList<string>")]
    [Arguments("SortedDictionary<string, int>")]
    [Arguments("SortedList<string, int>")]
    [Arguments("SortedSet<string>")]
    [Arguments("System.Collections.ObjectModel.Collection<string>")]
    public async Task Should_Report_A_Mutable_Collection(string type)
    {
        var result = await Analyze<ValueObjectAnalyzer>(
            $$"""
              using System.Collections.Generic;
              using Rocket.Surgery.LaunchPad.Primitives;

              namespace Sample;

              [ValueObject]
              public sealed partial record Basket
              {
                  public required {{type}} Items { get; init; }
              }
              """,
            builder => builder.AddReferences(typeof(LinkedList<>).Assembly, typeof(System.Collections.ObjectModel.Collection<>).Assembly)
        );

        await Diagnostics(result).Should().BeEquivalentTo([$"LPAD2018: {type}"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task Should_Report_A_Mutable_Collection_Of_A_Positional_Value_Object()
    {
        var result = await Analyze<ValueObjectAnalyzer>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [ValueObject]
            public readonly partial record struct Basket(int[] Items, string Name);
            """
        );

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2018: int[]"], CollectionOrdering.Matching);
        await Messages(result, "LPAD2018")
             .Should()
             .BeEquivalentTo(
                  ["Property 'Items' of value object 'Basket' is a 'int[]', which can be changed after the value object is created; use an immutable collection"],
                  CollectionOrdering.Matching
              );
    }
}
