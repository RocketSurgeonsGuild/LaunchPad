using FluentAssertions;
using NodaTime;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Foundation;
using System;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests
{
    public class SystemTextJsonNodaTimeTests : LoggerTest
    {
        private JsonSerializerOptions _settings;

        public SystemTextJsonNodaTimeTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            _settings = new JsonSerializerOptions().ConfigureNodaTimeForLaunchPad(DateTimeZoneProviders.Tzdb);
        }

        [Theory]
        [InlineData("2020-01-01T12:12:12Z")]
        [InlineData("2020-01-01T12:12:12.000000000Z")]
        [InlineData("01/01/2020 12:12:12 +00:00")]
        public void Instant_Tests(string value)
        {
            JsonSerializer.Deserialize<Instant>("\"" + value + "\"", _settings)!
               .Should()
               .Be(Instant.FromUtc(2020, 01, 01, 12, 12, 12));
        }

        [Theory]
        [InlineData("2020-01-01")]
        [InlineData("2020-01-01 (ISO)")]
        public void LocalDate_Tests(string value)
        {
            JsonSerializer.Deserialize<LocalDate>("\"" + value + "\"", _settings)!
               .Should()
               .Be(new LocalDate(2020, 01, 01));
        }

        [Theory]
        [InlineData("2020-01-01T12:12:12")]
        [InlineData("2020-01-01T12:12:12.000000000")]
        [InlineData("2020-01-01T12:12:12.0000000")]
        [InlineData("2020-01-01T12:12:12.000000000 (ISO)")]
        public void LocalDateTime_Tests(string value)
        {
            JsonSerializer.Deserialize<LocalDateTime>("\"" + value + "\"", _settings)!
               .Should()
               .Be(new LocalDateTime(2020, 01, 01, 12, 12, 12));
        }

        [Theory]
        [InlineData("12:12:12")]
        [InlineData("12:12:12.0000000")]
        [InlineData("12:12:12.000000000")]
        public void LocalTime_Tests(string value)
        {
            JsonSerializer.Deserialize<LocalTime>("\"" + value + "\"", _settings)!
               .Should()
               .Be(new LocalTime(12, 12, 12));
        }

        [Theory]
        [InlineData("+03:25:45")]
        public void Offset_Tests(string value)
        {
            JsonSerializer.Deserialize<Offset>("\"" + value + "\"", _settings)!
               .Should()
               .Be(Offset.FromTimeSpan(TimeSpan.FromSeconds(12345)));
        }

        [Theory]
        [InlineData("3:25:45")]
        [InlineData("0:03:25:45")]
        public void Duration_Tests(string value)
        {
            JsonSerializer.Deserialize<Duration>("\"" + value + "\"", _settings)!
               .Should()
               .Be(Duration.FromTimeSpan(TimeSpan.FromSeconds(12345)));
        }

        [Theory]
        [InlineData("PT12345S")]
        [InlineData("PT3H25M45S")]
        public void Period_Tests(string value)
        {
            JsonSerializer.Deserialize<Period>("\"" + value + "\"", _settings)!
               .Normalize().Should()
               .Be(Period.FromSeconds(12345).Normalize());
        }

        [Theory]
        [InlineData("2020-01-01T12:12:12+01")]
        [InlineData("2020-01-01T12:12:12+01 (ISO)")]
        public void OffsetDateTime_Tests(string value)
        {
            JsonSerializer.Deserialize<OffsetDateTime>("\"" + value + "\"", _settings)!
               .Should()
               .Be(new OffsetDateTime(new LocalDateTime(2020, 01, 01, 12, 12, 12), Offset.FromHours(1)));
        }

        [Theory]
        [InlineData("2020-01-01+01")]
        [InlineData("2020-01-01+01 (ISO)")]
        public void OffsetDate_Tests(string value)
        {
            JsonSerializer.Deserialize<OffsetDate>("\"" + value + "\"", _settings)!
               .Should()
               .Be(new OffsetDate(new LocalDate(2020, 01, 01), Offset.FromHours(1)));
        }

        [Theory]
        [InlineData("12:12:12+01:00")]
        [InlineData("12:12:12+01")]
        public void OffsetTime_Tests(string value)
        {
            JsonSerializer.Deserialize<OffsetTime>("\"" + value + "\"", _settings)!
               .Should()
               .Be(new OffsetTime(new LocalTime(12, 12, 12), Offset.FromHours(1)));
        }

        [Theory]
        [InlineData("2020-01-01T07:12:12-05 America/New_York")]
        public void ZonedDateTime_Tests(string value)
        {
            JsonSerializer.Deserialize<ZonedDateTime>("\"" + value + "\"", _settings)!
               .Should()
               .Be(new ZonedDateTime(Instant.FromUtc(2020, 01, 01, 12, 12, 12), DateTimeZoneProviders.Tzdb.GetZoneOrNull("America/New_York")));
        }
    }
}