using Newtonsoft.Json;
using Rocket.Surgery.LaunchPad.Foundation;

namespace NodaTime.NewtonsoftJson.Tests;

public class NewtonsoftJsonNodaTimeTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    private readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
       .ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);

    [Test]
    [Arguments("2020-01-01T12:12:12Z")]
    [Arguments("2020-01-01T12:12:12.000000000Z")]
    [Arguments("01/01/2020 12:12:12 +00:00")]
    public void Instant_Tests(string value)
    {
        JsonConvert.DeserializeObject<Instant>("\"" + value + "\"", _settings)
                   .ShouldBe(Instant.FromUtc(2020, 01, 01, 12, 12, 12));
    }

    [Test]
    [Arguments("2020-01-01")]
    [Arguments("2020-01-01 (ISO)")]
    public void LocalDate_Tests(string value)
    {
        JsonConvert.DeserializeObject<LocalDate>("\"" + value + "\"", _settings)
                   .ShouldBe(new LocalDate(2020, 01, 01));
    }

    [Test]
    [Arguments("2020-01-01T12:12:12")]
    [Arguments("2020-01-01T12:12:12.000000000")]
    [Arguments("2020-01-01T12:12:12.0000000")]
    public void LocalDateTime_Tests(string value)
    {
        JsonConvert.DeserializeObject<LocalDateTime>("\"" + value + "\"", _settings)
                   .ShouldBe(new LocalDateTime(2020, 01, 01, 12, 12, 12));
    }

    [Test]
    [Arguments("12:12:12")]
    [Arguments("12:12:12.0000000")]
    [Arguments("12:12:12.000000000")]
    public void LocalTime_Tests(string value)
    {
        JsonConvert.DeserializeObject<LocalTime>("\"" + value + "\"", _settings)
                   .ShouldBe(new LocalTime(12, 12, 12));
    }

    [Test]
    [Arguments("+03:25:45")]
    public void Offset_Tests(string value)
    {
        JsonConvert.DeserializeObject<Offset>("\"" + value + "\"", _settings)
                   .ShouldBe(Offset.FromTimeSpan(TimeSpan.FromSeconds(12345)));
    }

    [Test]
    [Arguments("3:25:45")]
    [Arguments("0:03:25:45")]
    public void Duration_Tests(string value)
    {
        JsonConvert.DeserializeObject<Duration>("\"" + value + "\"", _settings)
                   .ShouldBe(Duration.FromTimeSpan(TimeSpan.FromSeconds(12345)));
    }

    [Test]
    [Arguments("PT12345S")]
    [Arguments("PT3H25M45S")]
    public void Period_Tests(string value)
    {
        JsonConvert.DeserializeObject<Period>("\"" + value + "\"", _settings)!
                   .Normalize().ShouldBe(Period.FromSeconds(12345).Normalize());
    }

    [Test]
    [Arguments("2020-01-01T12:12:12+01")]
    [Arguments("2020-01-01T12:12:12+01 (ISO)")]
    public void OffsetDateTime_Tests(string value)
    {
        JsonConvert.DeserializeObject<OffsetDateTime>("\"" + value + "\"", _settings)
                   .ShouldBe(new OffsetDateTime(new LocalDateTime(2020, 01, 01, 12, 12, 12), Offset.FromHours(1)));
    }

    [Test]
    [Arguments("2020-01-01+01")]
    [Arguments("2020-01-01+01 (ISO)")]
    public void OffsetDate_Tests(string value)
    {
        JsonConvert.DeserializeObject<OffsetDate>("\"" + value + "\"", _settings)
                   .ShouldBe(new OffsetDate(new LocalDate(2020, 01, 01), Offset.FromHours(1)));
    }

    [Test]
    [Arguments("12:12:12+01:00")]
    [Arguments("12:12:12+01")]
    public void OffsetTime_Tests(string value)
    {
        JsonConvert.DeserializeObject<OffsetTime>("\"" + value + "\"", _settings)
                   .ShouldBe(new OffsetTime(new LocalTime(12, 12, 12), Offset.FromHours(1)));
    }

    [Test]
    [Arguments("2020-01-01T07:12:12-05 America/New_York")]
    public void ZonedDateTime_Tests(string value)
    {
        JsonConvert.DeserializeObject<ZonedDateTime>("\"" + value + "\"", _settings)
                   .ShouldBe(new ZonedDateTime(Instant.FromUtc(2020, 01, 01, 12, 12, 12), DateTimeZoneProviders.Tzdb.GetZoneOrNull("America/New_York")!));
    }
}
