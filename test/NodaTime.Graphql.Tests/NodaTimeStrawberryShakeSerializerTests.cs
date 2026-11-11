using HotChocolate.Language;
using HotChocolate.Types;
using Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;
using StrawberryShake.Serialization;
using DateTimeType = HotChocolate.Types.NodaTime.DateTimeType;
using DurationSerializer = Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.DurationSerializer;
using DurationType = HotChocolate.Types.NodaTime.DurationType;
using IInputValueFormatter = StrawberryShake.Serialization.IInputValueFormatter;
using LocalDateSerializer = Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.LocalDateSerializer;
using LocalDateTimeSerializer = Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.LocalDateTimeSerializer;
using LocalDateTimeType = HotChocolate.Types.NodaTime.LocalDateTimeType;
using LocalDateType = HotChocolate.Types.NodaTime.LocalDateType;
using LocalTimeSerializer = Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.LocalTimeSerializer;
using LocalTimeType = HotChocolate.Types.NodaTime.LocalTimeType;

namespace NodaTime.Graphql.Tests;

public class NodaTimeStrawberryShakeSerializerTests
{
    public static IEnumerable<Func<(ISerializer Serializer, ScalarType Scalar, object Value, string Serialized)>> Serializers()
    {
        yield return () => (new LocalDateSerializer(), new LocalDateType(), new LocalDate(2022, 1, 31), "2022-01-31");
        yield return () => (new LocalTimeSerializer(), new LocalTimeType(), new LocalTime(4, 5, 6), "04:05:06");
        yield return () => (new LocalTimeSerializer(), new LocalTimeType(), new LocalTime(4, 5, 6, 789), "04:05:06.789");
        yield return () => (new LocalDateTimeSerializer(), new LocalDateTimeType(), new LocalDateTime(2022, 1, 31, 4, 5, 6), "2022-01-31T04:05:06");
        yield return () => (
            new OffsetDateTimeSerializer(),
            new DateTimeType(),
            new OffsetDateTime(new(2022, 1, 31, 4, 5, 6), Offset.FromHours(-5)),
            "2022-01-31T04:05:06-05:00"
        );
        yield return () => (new OffsetDateTimeSerializer(), new DateTimeType(), new OffsetDateTime(new(2022, 1, 31, 4, 5, 6, 250), Offset.Zero), "2022-01-31T04:05:06.25Z");
        yield return () => (new DurationSerializer(), new DurationType(), Duration.FromHours(26) + Duration.FromMilliseconds(500), "P1DT2H0.5S");
        yield return () => (new DurationSerializer(), new DurationType(), Duration.FromMinutes(-90), "-PT1H30M");
        yield return () => (new DurationSerializer(), new DurationType(), Duration.Zero, "PT0S");
    }

    [Test]
    [MethodDataSource(nameof(Serializers))]
    public void Should_Format_The_Value_As_The_Server_Writes_It(ISerializer serializer, ScalarType scalar, object value, string serialized)
    {
        ( (StringValueNode)scalar.ValueToLiteral(value) ).Value.ShouldBe(serialized);
        ( (IInputValueFormatter)serializer ).Format(value).ShouldBe(serialized);
    }

    [Test]
    [MethodDataSource(nameof(Serializers))]
    public void Should_Write_What_The_Server_Reads(ISerializer serializer, ScalarType scalar, object value, string serialized) =>
        scalar.CoerceInputLiteral(new StringValueNode((string)( (IInputValueFormatter)serializer ).Format(value)!)).ShouldBe(value);

    [Test]
    [MethodDataSource(nameof(Serializers))]
    public void Should_Parse_What_The_Server_Writes(ISerializer serializer, ScalarType scalar, object value, string serialized)
    {
        var parsed = serializer switch
        {
            ILeafValueParser<string, LocalDate> parser => parser.Parse(serialized),
            ILeafValueParser<string, LocalTime> parser => parser.Parse(serialized),
            ILeafValueParser<string, LocalDateTime> parser => parser.Parse(serialized),
            ILeafValueParser<string, OffsetDateTime> parser => parser.Parse(serialized),
            ILeafValueParser<string, Duration> parser => (object)parser.Parse(serialized),
            _ => throw new InvalidOperationException($"No parser for {serializer.GetType().Name}"),
        };

        parsed.ShouldBe(value);
    }

    [Test]
    [MethodDataSource(nameof(Serializers))]
    public void Should_Be_Named_For_The_Scalar_It_Serializes(ISerializer serializer, ScalarType scalar, object value, string serialized) =>
        serializer.TypeName.ShouldBe(scalar.Name);

    [Test]
    [Arguments("P1W2DT3H4M5.5S")]
    [Arguments("P-1DT2H")]
    [Arguments("PT0,25S")]
    public void Should_Parse_The_Durations_The_Server_Reads(string serialized) =>
        new DurationSerializer().Parse(serialized).ShouldBe((Duration)new DurationType().CoerceInputLiteral(new StringValueNode(serialized)));

    [Test]
    [Arguments("yesterday")]
    [Arguments("P")]
    [Arguments("PT")]
    [Arguments("P1Y")]
    [Arguments("P1H")]
    [Arguments("PT1S2M")]
    public void Should_Reject_Text_That_Is_Not_A_Duration(string serialized) => Should.Throw<FormatException>(() => new DurationSerializer().Parse(serialized));

    [Test]
    public void Should_Reject_Text_That_Does_Not_Match_The_Pattern() =>
        Should.Throw<Text.UnparsableValueException>(() => new OffsetDateTimeSerializer().Parse("yesterday"));
}
