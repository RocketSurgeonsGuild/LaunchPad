using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

using Rocket.Surgery.LaunchPad.Primitives;
using Rocket.Surgery.LaunchPad.Primitives.Advanced;
using Rocket.Surgery.LaunchPad.Primitives.Diagnostics;

namespace Primitives.Behavior.Tests;

/// <summary>
///     The counters and trace events the generated Create and TryCreate record. Other tests create domain primitives at the same time, so each
///     test counts only the measurements of a type no other test uses.
/// </summary>
[NotInParallel(nameof(InstrumentationTests))]
public class InstrumentationTests
{
    [Test]
    public async Task Should_Count_Every_Value_Accepted_And_Rejected()
    {
        var measurements = new ConcurrentBag<(string Instrument, long Value, string? Primitive, string? Error)>();
        using var listener = new MeterListener();
        listener.InstrumentPublished = static (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == DomainPrimitivesInstrumentation.Name) meterListener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>(
            (instrument, value, tags, _) =>
            {
                string? primitive = null, error = null;
                foreach (var tag in tags)
                {
                    if (tag.Key == "primitive_type") primitive = tag.Value as string;
                    if (tag.Key == "error_type") error = tag.Value as string;
                }

                if (primitive == nameof(InstrumentedName)) measurements.Add((instrument.Name, value, primitive, error));
            }
        );
        listener.Start();

        InstrumentedName.Create("abcdef");
        InstrumentedName.TryCreate("abcdefg", out _, out _);
        InstrumentedName.TryCreate("ab", out _, out _);

        var counts = measurements.GroupBy(z => (z.Instrument, z.Error)).ToDictionary(z => z.Key, z => z.Sum(x => x.Value));
        await counts[("domain_primitive.validation.success", null)].Should().BeEqualTo(2);
        await counts[("domain_primitive.creation", null)].Should().BeEqualTo(2);
        await counts[("domain_primitive.validation.failure", "LENGTH")].Should().BeEqualTo(1);
    }

    [Test]
    public async Task Should_Add_An_Event_For_A_Rejected_Value_To_The_Current_Activity()
    {
        using var listener = new ActivityListener
        {
            ShouldListenTo = static source => source.Name is DomainPrimitivesInstrumentation.Name or nameof(InstrumentationTests),
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
        };
        ActivitySource.AddActivityListener(listener);
        using var source = new ActivitySource(nameof(InstrumentationTests));
        using var activity = source.StartActivity("validate")!;

        ThemeColor.TryCreate("not a color", out _, out _);
        ThemeColor.TryCreate("#FFF", out _, out _);

        var failure = await activity.Events.Should().HaveSingleItem();
        await failure!.Name.Should().BeEqualTo("domain_primitive.validation.failure");
        var tags = failure.Tags.ToDictionary(z => z.Key, z => z.Value);
        await tags["primitive_type"].Should().BeEqualTo<object?>(nameof(ThemeColor));
        await tags["error_type"].Should().BeEqualTo<object?>("FORMAT");
        await tags["error_message"].Should().BeEqualTo<object?>("ThemeColor has an invalid format.");
    }

    [Test]
    public async Task Should_Name_The_Meter_And_The_Activity_Source()
    {
        await DomainPrimitivesInstrumentation.Meter.Name.Should().BeEqualTo("Rocket.Surgery.LaunchPad.Primitives");
        await DomainPrimitivesInstrumentation.ActivitySource.Name.Should().BeEqualTo("Rocket.Surgery.LaunchPad.Primitives");
        // nothing is recorded, and nothing throws, without a listener
        DomainPrimitivesInstrumentation.RecordCreation("Nothing");
        DomainPrimitivesInstrumentation.RecordValidationFailure("Nothing", new("CODE", "Message"));
    }
}

/// <summary>A string domain primitive only <see cref="InstrumentationTests" /> creates.</summary>
[StringPrimitive, MinLength(3)]
public readonly partial record struct InstrumentedName;

public class PrimitiveBuilderTests
{
    [Test]
    public async Task Should_Add_Rules_Of_The_Callers_Own()
    {
        var builder = PrimitiveBuilder<FirstName, string>.For().WithValue("Ada").Must(static z => z.StartsWith('A'), "A_NAME", "Starts with A.");

        await builder.Build(out var name).Should().BeTrue();
        await name.Value.Should().BeEqualTo("Ada");
        await builder.BuildOrThrow().Should().BeEqualTo(FirstName.Create("Ada"));
    }

    [Test]
    public async Task Should_Reject_A_Value_Its_Rules_Reject()
    {
        var builder = PrimitiveBuilder<FirstName, string>.For().WithValue("Bea").Must(static z => z.StartsWith('A'), "A_NAME", "Starts with A.");

        await builder.Build(out _).Should().BeFalse();
        var exception = await PrimitiveAssert.Throws<DomainPrimitiveValidationException>(() => builder.BuildOrThrow());
        await exception!.Error.Should().BeEqualTo(new PrimitiveError("A_NAME", "Starts with A."));
    }

    [Test]
    public async Task Should_Still_Apply_The_Rules_Of_The_Domain_Primitive()
    {
        var builder = PrimitiveBuilder<Score, int>.For().WithValue(500).Must(static _ => true, "ANY", "Any.");

        await builder.Build(out _).Should().BeFalse();
        var exception = await PrimitiveAssert.Throws<DomainPrimitiveValidationException>(() => builder.BuildOrThrow());
        await exception!.Error.Code.Should().BeEqualTo("RANGE");
    }

    [Test]
    public async Task Should_Need_A_Value()
    {
        var builder = PrimitiveBuilder<LastName, string>.For();

        await builder.Build(out var nothing).Should().BeFalse();
        await nothing.Should().BeNull();
        var exception = await PrimitiveAssert.Throws<DomainPrimitiveValidationException>(() => builder.BuildOrThrow());
        await exception!.Error.Code.Should().BeEqualTo("NULL_INPUT");
        await PrimitiveAssert.Throws<ArgumentNullException>(() => builder.Must(null!, "X", "X"));
    }
}

public class PrimitiveCollectionExtensionsTests
{
    [Test]
    public async Task Should_Create_Every_Value()
    {
        var list = Scores.ToDomainPrimitiveList<Score, int>();
        var array = Names.Select(static z => z).ToDomainPrimitiveArray<FirstName, string>();
        var fromSpan = Create([4, 5]);

        await list.SequenceEqual([Score.Create(1), Score.Create(2), Score.Create(3)]).Should().BeTrue();
        await array.SequenceEqual([FirstName.Create("Ada"), FirstName.Create("Bea")]).Should().BeTrue();
        await fromSpan.SequenceEqual([Score.Create(4), Score.Create(5)]).Should().BeTrue();
        await Array.Empty<int>().ToDomainPrimitiveList<Score, int>().Should().BeEmpty();
    }

    private static readonly int[] Scores = [1, 2, 3];
    private static readonly string[] Names = ["Ada", "Bea"];
    private static readonly int[] Rejected = [1, 200, 300];

    private static Score[] Create(ReadOnlySpan<int> values) => values.ToDomainPrimitiveArray<Score, int>();

    [Test]
    public async Task Should_Name_The_Index_Of_The_First_Value_It_Rejects()
    {
        var exception = await PrimitiveAssert.Throws<ArgumentException>(() => Rejected.ToDomainPrimitiveList<Score, int>());

        await exception!.Message.Should().StartWith("Element at index 1 failed validation: [RANGE]");
        await exception.ParamName.Should().BeEqualTo("values");
        await Assert.That(exception.InnerException).IsTypeOf<DomainPrimitiveValidationException>();
        await PrimitiveAssert.Throws<ArgumentException>(() => Create([1, -1]));
        await PrimitiveAssert.Throws<ArgumentNullException>(() => ( (IEnumerable<int>)null! ).ToDomainPrimitiveList<Score, int>());
    }
}

public class PrimitiveErrorTests
{
    [Test]
    public async Task Should_Tell_An_Error_From_None()
    {
        await PrimitiveError.None.IsError.Should().BeFalse();
        await default(PrimitiveError).Should().BeEqualTo(PrimitiveError.None);
        await PrimitiveError.Create("CODE", "Message").IsError.Should().BeTrue();
        await PrimitiveError.Create("CODE", "Message").Should().BeEqualTo(new PrimitiveError("CODE", "Message"));
    }

    [Test]
    public async Task Should_Carry_The_Error_In_The_Exception()
    {
        var error = new PrimitiveError("CODE", "The message.");
        var inner = new InvalidOperationException();

        var exception = new DomainPrimitiveValidationException(error, inner, "input");

        await exception.Error.Should().BeEqualTo(error);
        await exception.Message.Should().StartWith("[CODE] The message.");
        await exception.ParamName.Should().BeEqualTo("input");
        await exception.InnerException.Should().BeSameReferenceAs(inner);
        await new DomainPrimitiveValidationException(error).ParamName.Should().BeEqualTo("value");
        await new DomainPrimitiveValidationException(error, (Exception?)null).ParamName.Should().BeEqualTo("value");
    }
}
