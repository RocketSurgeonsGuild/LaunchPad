using System.Text.Json;
using System.Text.Json.Serialization;

using NodaTime.Text;

using Rocket.Surgery.LaunchPad.NodaTime;

namespace NodaTime.Tests;

// LaunchPad's own converters (src/NodaTime/JsonConverters.md): the same JSON NodaTime.Serialization.SystemTextJson writes, with no reflection.
// The options have only LaunchPad's NodaTime resolver in their chain, so nothing here can fall back to the reflection resolver.
public class NodaTimeJsonConvertersTests
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions().ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);

    private static readonly JsonSerializerOptions CamelCase = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, }
       .ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);

    private static readonly Instant Start = Instant.FromUtc(2020, 1, 1, 12, 0);
    private static readonly Instant End = Instant.FromUtc(2020, 1, 2, 12, 0);

    [Test]
    public async Task Writes_And_Reads_Every_Single_Value_Type()
    {
        await RoundTrip(Start, "\"2020-01-01T12:00:00Z\"");
        await RoundTrip(new LocalDate(2020, 1, 1), "\"2020-01-01\"");
        await RoundTrip(new LocalDateTime(2020, 1, 1, 12, 30, 15), "\"2020-01-01T12:30:15\"");
        await RoundTrip(new LocalTime(12, 30, 15), "\"12:30:15\"");
        await RoundTrip(Offset.FromHours(2), "\"\\u002B02\"");
        await RoundTrip(Duration.FromHours(25), "\"25:00:00\"");
        await RoundTrip(Period.FromDays(3), "\"P3D\"");
        await RoundTrip(new OffsetDateTime(new(2020, 1, 1, 12, 0), Offset.FromHours(2)), "\"2020-01-01T12:00:00\\u002B02\"");
        await RoundTrip(new OffsetDate(new(2020, 1, 1), Offset.FromHours(2)), "\"2020-01-01\\u002B02\"");
        await RoundTrip(new OffsetTime(new(12, 0), Offset.FromHours(2)), "\"12:00:00\\u002B02:00\"");
        await RoundTrip(new AnnualDate(2, 29), "\"02-29\"");
        await RoundTrip(new YearMonth(2020, 2), "\"2020-02\"");
        await RoundTrip(DateTimeZoneProviders.Tzdb["Europe/London"], "\"Europe/London\"");
        await RoundTrip(Start.InZone(DateTimeZoneProviders.Tzdb["Europe/London"]), "\"2020-01-01T12:00:00Z Europe/London\"");
    }

    // NodaTime.Serialization.SystemTextJson's format: the local date and time with its offset, then the zone id.
    [Test]
    [Arguments("2020-01-01T12:00:00", "Etc/UTC", "\"2020-01-01T12:00:00Z Etc/UTC\"")]
    [Arguments("2020-07-01T12:00:00", "Europe/London", "\"2020-07-01T12:00:00\\u002B01 Europe/London\"")]
    [Arguments("2020-07-01T12:00:00", "America/New_York", "\"2020-07-01T12:00:00-04 America/New_York\"")]
    [Arguments("2020-07-01T12:00:00.123", "Asia/Kolkata", "\"2020-07-01T12:00:00.123\\u002B05:30 Asia/Kolkata\"")]
    public async Task Writes_A_Zoned_Date_Time_In_A_Format_It_Reads(string local, string zone, string json) =>
        await RoundTrip(LocalDateTimePattern.ExtendedIso.Parse(local).Value.InZoneStrictly(DateTimeZoneProviders.Tzdb[zone]), json);

    // Every value, fractions of a second and negative offsets included, reads back as the value written.
    [Test]
    public async Task Reads_Back_Every_Type_It_Writes()
    {
        var instant = Instant.FromUtc(2020, 7, 1, 12, 30, 15).PlusNanoseconds(123_456_789);
        var offset = Offset.FromHoursAndMinutes(-5, -30);

        await ReadsBack(instant);
        await ReadsBack(instant.InUtc().LocalDateTime);
        await ReadsBack(instant.InUtc().TimeOfDay);
        await ReadsBack(instant.InUtc().Date);
        await ReadsBack(offset);
        await ReadsBack(Offset.Zero);
        await ReadsBack(Duration.FromNanoseconds(-90_061_123_456_789));
        await ReadsBack(Period.FromDays(3) + Period.FromHours(4) + Period.FromNanoseconds(5));
        await ReadsBack(instant.WithOffset(offset));
        await ReadsBack(instant.WithOffset(offset).ToOffsetDate());
        await ReadsBack(instant.WithOffset(offset).ToOffsetTime());
        await ReadsBack(instant.InZone(DateTimeZoneProviders.Tzdb["America/St_Johns"]));
        await ReadsBack(instant.InUtc());
        await ReadsBack(new AnnualDate(2, 29));
        await ReadsBack(new YearMonth(2020, 2));
        await ReadsBack(new Interval(instant, instant.PlusTicks(1)));
        await ReadsBack(new DateInterval(new(2020, 1, 1), new(2020, 1, 31)));
        await ReadsBack(DateTimeZoneProviders.Tzdb["America/St_Johns"]);
    }

    [Test]
    public async Task Writes_An_Interval_As_Its_Start_And_End()
    {
        await RoundTrip(new Interval(Start, End), """{"Start":"2020-01-01T12:00:00Z","End":"2020-01-02T12:00:00Z"}""");
        await RoundTrip(new Interval(Start, null), """{"Start":"2020-01-01T12:00:00Z"}""");
        await RoundTrip(new Interval(null, End), """{"End":"2020-01-02T12:00:00Z"}""");
        await RoundTrip(new Interval(null, null), "{}");
    }

    [Test]
    public async Task Writes_A_Date_Interval_As_Its_Start_And_End()
    {
        var interval = new DateInterval(new(2020, 1, 1), new(2020, 1, 31));

        await RoundTrip(interval, """{"Start":"2020-01-01","End":"2020-01-31"}""");
        await Assert.ThrowsAsync<JsonException>(() => Task.FromResult(JsonSerializer.Deserialize<DateInterval>("""{"Start":"2020-01-01"}""", Options)));
    }

    [Test]
    public async Task Names_The_Interval_Properties_With_The_Naming_Policy()
    {
        await JsonSerializer.Serialize(new Interval(Start, End), CamelCase)
                            .Should().BeEqualTo("""{"start":"2020-01-01T12:00:00Z","end":"2020-01-02T12:00:00Z"}""");
        await JsonSerializer.Deserialize<DateInterval>("""{"start":"2020-01-01","end":"2020-01-31"}""", CamelCase)
                            .Should().BeEqualTo(new DateInterval(new(2020, 1, 1), new(2020, 1, 31)));
    }

    [Test]
    public async Task Skips_Properties_Of_An_Interval_It_Does_Not_Know()
    {
        await JsonSerializer.Deserialize<Interval>("""{"Start":"2020-01-01T12:00:00Z","Other":{"a":[1,2]},"End":"2020-01-02T12:00:00Z"}""", Options)
                            .Should().BeEqualTo(new Interval(Start, End));
    }

    [Test]
    public async Task Writes_Values_As_Dictionary_Keys()
    {
        var dates = new Dictionary<LocalDate, int> { [new(2020, 1, 1)] = 1, };
        var zones = new Dictionary<DateTimeZone, int> { [DateTimeZoneProviders.Tzdb["Europe/London"]] = 1, };

        // The dictionaries come from a source generated context; their keys go through LaunchPad's converters.
        var options = new JsonSerializerOptions(Options);
        options.TypeInfoResolverChain.Insert(0, NodaTimeKeysJsonContext.Default);

        await JsonSerializer.Serialize(dates, options).Should().BeEqualTo("""{"2020-01-01":1}""");
        await JsonSerializer.Serialize(zones, options).Should().BeEqualTo("""{"Europe/London":1}""");
        await JsonSerializer.Deserialize<Dictionary<LocalDate, int>>("""{"2020-01-01":1}""", options)!.Keys.Single()
                            .Should().BeEqualTo(new LocalDate(2020, 1, 1));
    }

    [Test]
    public async Task Throws_A_Json_Exception_For_Text_No_Pattern_Parses()
    {
        await Assert.ThrowsAsync<JsonException>(() => Task.FromResult(JsonSerializer.Deserialize<LocalDate>("\"not a date\"", Options)));
        await Assert.ThrowsAsync<JsonException>(() => Task.FromResult(JsonSerializer.Deserialize<Instant>("42", Options)));
    }

    [Test]
    public async Task Resolves_Every_Type_And_Its_Nullable_Without_Reflection()
    {
        await JsonSerializer.Serialize<object>(new Interval(Start, End), Options)
                            .Should().BeEqualTo("""{"Start":"2020-01-01T12:00:00Z","End":"2020-01-02T12:00:00Z"}""");
        await JsonSerializer.Serialize<YearMonth?>(new YearMonth(2020, 2), Options).Should().BeEqualTo("\"2020-02\"");
        await JsonSerializer.Deserialize<AnnualDate?>("null", Options).Should().BeNull();
        await JsonSerializer.Deserialize<Interval?>("""{"Start":"2020-01-01T12:00:00Z"}""", Options).Should().BeEqualTo(new Interval(Start, null));
    }

    private static async Task ReadsBack<T>(T value) =>
        await JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, Options), Options).Should().BeEqualTo(value);

    private static async Task RoundTrip<T>(T value, string json)
    {
        await JsonSerializer.Serialize(value, Options).Should().BeEqualTo(json);
        await JsonSerializer.Deserialize<T>(json, Options).Should().BeEqualTo(value);
    }
}

[JsonSerializable(typeof(Dictionary<LocalDate, int>))]
[JsonSerializable(typeof(Dictionary<DateTimeZone, int>))]
internal sealed partial class NodaTimeKeysJsonContext : JsonSerializerContext;
