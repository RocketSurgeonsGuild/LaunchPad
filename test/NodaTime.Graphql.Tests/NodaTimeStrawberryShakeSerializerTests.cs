using NodaTime.Testing.TimeZones;
using NodaTime.Text;
using NodaTime.TimeZones;

using Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

using StrawberryShake.Serialization;

using LocalDateSerializer = Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.LocalDateSerializer;
using LocalDateTimeSerializer = Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.LocalDateTimeSerializer;
using LocalTimeSerializer = Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.LocalTimeSerializer;

namespace NodaTime.Graphql.Tests;

public class NodaTimeStrawberryShakeSerializerTests
{
    public static IEnumerable<Func<(ISerializer Serializer, object Value, object Serialized)>> Serializers()
    {
        yield return () => (new InstantSerializer(), Instant.FromUtc(2022, 1, 1, 4, 4, 4), "2022-01-01T04:04:04Z");
        yield return () => (new LocalDateSerializer(), new LocalDate(2022, 1, 31), "2022-01-31");
        yield return () => (new LocalTimeSerializer(), new LocalTime(4, 5, 6), "04:05:06");
        yield return () => (new LocalDateTimeSerializer(), new LocalDateTime(2022, 1, 31, 4, 5, 6), "2022-01-31T04:05:06");
        yield return () => (new OffsetDateTimeSerializer(), new OffsetDateTime(new(2022, 1, 31, 4, 5, 6), Offset.FromHours(-5)), "2022-01-31T04:05:06-05");
        yield return () => (new OffsetDateSerializer(), new OffsetDate(new(2022, 1, 31), Offset.FromHours(2)), "2022-01-31+02");
        yield return () => (new OffsetTimeSerializer(), new OffsetTime(new(4, 5, 6), Offset.FromHoursAndMinutes(5, 30)), "04:05:06+05:30");
        yield return () => (new PeriodSerializer(), Period.FromDays(3) + Period.FromHours(4), "P3DT4H");
        yield return () => (new DurationSerializer(), Duration.FromHours(26) + Duration.FromMilliseconds(500), "26:00:00.5");
        yield return () => (new OffsetSerializer(), Offset.FromHours(-7), "-07");
        yield return () => (new IsoDayOfWeekSerializer(), IsoDayOfWeek.Friday, 5);
        yield return () => (new DateTimeZoneSerializer(Provider()), Provider()[TestZoneId], TestZoneId);
        yield return () => (
            new ZonedDateTimeSerializer(Provider()),
            new LocalDateTime(2022, 1, 31, 4, 5, 6).InZoneStrictly(Provider()[TestZoneId]),
            $"2022-01-31T04:05:06+03 {TestZoneId}"
        );
    }

    [Test]
    [MethodDataSource(nameof(Serializers))]
    public void Should_Format_The_Value_As_The_Server_Writes_It(ISerializer serializer, object value, object serialized)
    {
        ((IInputValueFormatter)serializer).Format(value).ShouldBe(serialized);
    }

    [Test]
    [MethodDataSource(nameof(Serializers))]
    public void Should_Parse_What_The_Server_Writes(ISerializer serializer, object value, object serialized)
    {
        var parsed = serializer switch
        {
            ILeafValueParser<string, Instant> parser => parser.Parse((string)serialized),
            ILeafValueParser<string, LocalDate> parser => parser.Parse((string)serialized),
            ILeafValueParser<string, LocalTime> parser => parser.Parse((string)serialized),
            ILeafValueParser<string, LocalDateTime> parser => parser.Parse((string)serialized),
            ILeafValueParser<string, OffsetDateTime> parser => parser.Parse((string)serialized),
            ILeafValueParser<string, OffsetDate> parser => parser.Parse((string)serialized),
            ILeafValueParser<string, OffsetTime> parser => parser.Parse((string)serialized),
            ILeafValueParser<string, Period> parser => parser.Parse((string)serialized),
            ILeafValueParser<string, Duration> parser => parser.Parse((string)serialized),
            ILeafValueParser<string, Offset> parser => parser.Parse((string)serialized),
            ILeafValueParser<int, IsoDayOfWeek> parser => parser.Parse((int)serialized),
            ILeafValueParser<string, DateTimeZone> parser => parser.Parse((string)serialized),
            ILeafValueParser<string, ZonedDateTime> parser => (object)parser.Parse((string)serialized),
            _ => throw new InvalidOperationException($"No parser for {serializer.GetType().Name}"),
        };

        parsed.ShouldBe(value);
    }

    [Test]
    [MethodDataSource(nameof(Serializers))]
    public void Should_Be_Named_For_The_Scalar_It_Serializes(ISerializer serializer, object value, object serialized)
    {
        serializer.TypeName.ShouldBe(value is DateTimeZone ? "DateTimeZone" : value.GetType().Name);
    }

    [Test]
    public void Should_Reject_Text_That_Does_Not_Match_The_Pattern()
    {
        Should.Throw<UnparsableValueException>(() => new InstantSerializer().Parse("yesterday"));
    }

    [Test]
    public void Should_Reject_An_Unknown_Time_Zone()
    {
        Should.Throw<DateTimeZoneNotFoundException>(() => new DateTimeZoneSerializer(Provider()).Parse("Nowhere/Never"));
    }

    private const string TestZoneId = "Test/Plus3";

    // one provider for every case: a zone is equal only to itself
    private static IDateTimeZoneProvider Provider() => _provider.Value;

    private static readonly Lazy<IDateTimeZoneProvider> _provider = new(
        () =>
        {
            var builder = new FakeDateTimeZoneSource.Builder { VersionId = "test", };
            builder.Add(new SingleTransitionDateTimeZone(Instant.FromUtc(2000, 1, 1, 0, 0), Offset.FromHours(3), Offset.FromHours(3), TestZoneId));
            return new DateTimeZoneCache(builder.Build());
        }
    );
}
