using System.Text.Json;

using Rocket.Surgery.LaunchPad.NodaTime;

namespace NodaTime.Tests;

public class SystemTextJsonNodaTimeTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    [Arguments("2020-01-01T12:12:12Z")]
    [Arguments("2020-01-01T12:12:12.000000000Z")]
    [Arguments("01/01/2020 12:12:12 +00:00")]
    public async Task Instant_Tests(string value) => await JsonSerializer
                                              .Deserialize<Instant>("\"" + value + "\"", _settings)
                                              .Should().BeEqualTo(Instant.FromUtc(
                                                       2020,
                                                       01,
                                                       01,
                                                       12,
                                                       12,
                                                       12
                                                   ));

    [Test]
    [Arguments("2020-01-01")]
    [Arguments("2020-01-01 (ISO)")]
    public async Task LocalDate_Tests(string value) => await JsonSerializer
                                                .Deserialize<LocalDate>("\"" + value + "\"", _settings)
                                                .Should().BeEqualTo(new(2020, 01, 01));

    [Test]
    [Arguments("2020-01-01T12:12:12")]
    [Arguments("2020-01-01T12:12:12.000000000")]
    [Arguments("2020-01-01T12:12:12.0000000")]
    public async Task LocalDateTime_Tests(string value) => await JsonSerializer
                                                    .Deserialize<LocalDateTime>("\"" + value + "\"", _settings)
                                                    .Should().BeEqualTo(new(
                                                             2020,
                                                             01,
                                                             01,
                                                             12,
                                                             12,
                                                             12
                                                         ));

    [Test]
    [Arguments("12:12:12")]
    [Arguments("12:12:12.0000000")]
    [Arguments("12:12:12.000000000")]
    public async Task LocalTime_Tests(string value) => await JsonSerializer
                                                .Deserialize<LocalTime>("\"" + value + "\"", _settings)
                                                .Should().BeEqualTo(new(12, 12, 12));

    [Test]
    [Arguments("+03:25:45")]
    public async Task Offset_Tests(string value) => await JsonSerializer
                                             .Deserialize<Offset>("\"" + value + "\"", _settings)
                                             .Should().BeEqualTo(Offset.FromTimeSpan(TimeSpan.FromSeconds(12345)));

    [Test]
    [Arguments("3:25:45")]
    [Arguments("0:03:25:45")]
    public async Task Duration_Tests(string value) => await JsonSerializer
                                               .Deserialize<Duration>("\"" + value + "\"", _settings)
                                               .Should().BeEqualTo(Duration.FromTimeSpan(TimeSpan.FromSeconds(12345)));

    [Test]
    [Arguments("PT12345S")]
    [Arguments("PT3H25M45S")]
    public async Task Period_Tests(string value) => await JsonSerializer.Deserialize<Period>("\"" + value + "\"", _settings)!
                                                            .Normalize()
                                                            .Should().BeEqualTo(Period.FromSeconds(12345).Normalize());

    [Test]
    [Arguments("2020-01-01T12:12:12+01")]
    [Arguments("2020-01-01T12:12:12+01 (ISO)")]
    public async Task OffsetDateTime_Tests(string value) => await JsonSerializer
                                                     .Deserialize<OffsetDateTime>("\"" + value + "\"", _settings)
                                                     .Should().BeEqualTo(new OffsetDateTime(
                                                              new(
                                                                  2020,
                                                                  01,
                                                                  01,
                                                                  12,
                                                                  12,
                                                                  12
                                                              ),
                                                              Offset.FromHours(1)
                                                          ));

    [Test]
    [Arguments("2020-01-01+01")]
    [Arguments("2020-01-01+01 (ISO)")]
    public async Task OffsetDate_Tests(string value) => await JsonSerializer
                                                 .Deserialize<OffsetDate>("\"" + value + "\"", _settings)
                                                 .Should().BeEqualTo(new OffsetDate(new(2020, 01, 01), Offset.FromHours(1)));

    [Test]
    [Arguments("12:12:12+01:00")]
    [Arguments("12:12:12+01")]
    public async Task OffsetTime_Tests(string value) => await JsonSerializer
                                                 .Deserialize<OffsetTime>("\"" + value + "\"", _settings)
                                                 .Should().BeEqualTo(new OffsetTime(new(12, 12, 12), Offset.FromHours(1)));

    [Test]
    [Arguments("2020-01-01T07:12:12-05 America/New_York")]
    public async Task ZonedDateTime_Tests(string value) => await JsonSerializer
                                                    .Deserialize<ZonedDateTime>("\"" + value + "\"", _settings)
                                                    .Should().BeEqualTo(new ZonedDateTime(
                                                             Instant.FromUtc(
                                                                 2020,
                                                                 01,
                                                                 01,
                                                                 12,
                                                                 12,
                                                                 12
                                                             ),
                                                             DateTimeZoneProviders.Tzdb.GetZoneOrNull("America/New_York")!
                                                         ));

    private readonly JsonSerializerOptions _settings = new JsonSerializerOptions().ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);
}
