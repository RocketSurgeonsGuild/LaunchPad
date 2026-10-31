using System.Text.Json;

using Rocket.Surgery.LaunchPad.NodaTime;

namespace NodaTime.Tests;

public class SystemTextJsonNodaTimeTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    [Arguments("2020-01-01T12:12:12Z")]
    [Arguments("2020-01-01T12:12:12.000000000Z")]
    [Arguments("01/01/2020 12:12:12 +00:00")]
    public void Instant_Tests(string value) => JsonSerializer
                                              .Deserialize<Instant>("\"" + value + "\"", _settings)
                                              .ShouldBe(
                                                   Instant.FromUtc(
                                                       2020,
                                                       01,
                                                       01,
                                                       12,
                                                       12,
                                                       12
                                                   )
                                               );

    [Test]
    [Arguments("2020-01-01")]
    [Arguments("2020-01-01 (ISO)")]
    public void LocalDate_Tests(string value) => JsonSerializer
                                                .Deserialize<LocalDate>("\"" + value + "\"", _settings)
                                                .ShouldBe(new(2020, 01, 01));

    [Test]
    [Arguments("2020-01-01T12:12:12")]
    [Arguments("2020-01-01T12:12:12.000000000")]
    [Arguments("2020-01-01T12:12:12.0000000")]
    public void LocalDateTime_Tests(string value) => JsonSerializer
                                                    .Deserialize<LocalDateTime>("\"" + value + "\"", _settings)
                                                    .ShouldBe(
                                                         new(
                                                             2020,
                                                             01,
                                                             01,
                                                             12,
                                                             12,
                                                             12
                                                         )
                                                     );

    [Test]
    [Arguments("12:12:12")]
    [Arguments("12:12:12.0000000")]
    [Arguments("12:12:12.000000000")]
    public void LocalTime_Tests(string value) => JsonSerializer
                                                .Deserialize<LocalTime>("\"" + value + "\"", _settings)
                                                .ShouldBe(new(12, 12, 12));

    [Test]
    [Arguments("+03:25:45")]
    public void Offset_Tests(string value) => JsonSerializer
                                             .Deserialize<Offset>("\"" + value + "\"", _settings)
                                             .ShouldBe(Offset.FromTimeSpan(TimeSpan.FromSeconds(12345)));

    [Test]
    [Arguments("3:25:45")]
    [Arguments("0:03:25:45")]
    public void Duration_Tests(string value) => JsonSerializer
                                               .Deserialize<Duration>("\"" + value + "\"", _settings)
                                               .ShouldBe(Duration.FromTimeSpan(TimeSpan.FromSeconds(12345)));

    [Test]
    [Arguments("PT12345S")]
    [Arguments("PT3H25M45S")]
    public void Period_Tests(string value) => JsonSerializer.Deserialize<Period>("\"" + value + "\"", _settings)!
                                                            .Normalize()
                                                            .ShouldBe(Period.FromSeconds(12345).Normalize());

    [Test]
    [Arguments("2020-01-01T12:12:12+01")]
    [Arguments("2020-01-01T12:12:12+01 (ISO)")]
    public void OffsetDateTime_Tests(string value) => JsonSerializer
                                                     .Deserialize<OffsetDateTime>("\"" + value + "\"", _settings)
                                                     .ShouldBe(
                                                          new OffsetDateTime(
                                                              new(
                                                                  2020,
                                                                  01,
                                                                  01,
                                                                  12,
                                                                  12,
                                                                  12
                                                              ),
                                                              Offset.FromHours(1)
                                                          )
                                                      );

    [Test]
    [Arguments("2020-01-01+01")]
    [Arguments("2020-01-01+01 (ISO)")]
    public void OffsetDate_Tests(string value) => JsonSerializer
                                                 .Deserialize<OffsetDate>("\"" + value + "\"", _settings)
                                                 .ShouldBe(new OffsetDate(new(2020, 01, 01), Offset.FromHours(1)));

    [Test]
    [Arguments("12:12:12+01:00")]
    [Arguments("12:12:12+01")]
    public void OffsetTime_Tests(string value) => JsonSerializer
                                                 .Deserialize<OffsetTime>("\"" + value + "\"", _settings)
                                                 .ShouldBe(new OffsetTime(new(12, 12, 12), Offset.FromHours(1)));

    [Test]
    [Arguments("2020-01-01T07:12:12-05 America/New_York")]
    public void ZonedDateTime_Tests(string value) => JsonSerializer
                                                    .Deserialize<ZonedDateTime>("\"" + value + "\"", _settings)
                                                    .ShouldBe(
                                                         new ZonedDateTime(
                                                             Instant.FromUtc(
                                                                 2020,
                                                                 01,
                                                                 01,
                                                                 12,
                                                                 12,
                                                                 12
                                                             ),
                                                             DateTimeZoneProviders.Tzdb.GetZoneOrNull("America/New_York")!
                                                         )
                                                     );

    private readonly JsonSerializerOptions _settings = new JsonSerializerOptions().ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);
}
