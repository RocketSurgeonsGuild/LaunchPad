using JetBrains.Annotations;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using NodaTime.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rocket.Surgery.LaunchPad.Foundation
{
    /// <summary>
    /// Extensions for noda time
    /// </summary>
    [PublicAPI]
    public static class NodaTimeSystemTextJsonSerializationExtensions
    {
        /// <summary>
        /// Configure System.Text.Json with defaults for launchpad
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dateTimeZoneProvider"></param>
        /// <returns></returns>
        public static JsonSerializerOptions ConfigureNodaTimeForLaunchPad(this JsonSerializerOptions options, IDateTimeZoneProvider dateTimeZoneProvider)
        {
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.ConfigureForNodaTime(dateTimeZoneProvider);
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<Instant>(
                    InstantPattern.General,
                    InstantPattern.ExtendedIso,
                    new SystemTextJsonDateTimeOffsetPattern()
                )
            );
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<LocalDate>(
                    LocalDatePattern.Iso,
                    LocalDatePattern.FullRoundtrip
                )
            );
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<LocalDateTime>(
                    LocalDateTimePattern.GeneralIso,
                    LocalDateTimePattern.ExtendedIso,
                    LocalDateTimePattern.BclRoundtrip,
                    LocalDateTimePattern.FullRoundtrip,
                    LocalDateTimePattern.FullRoundtripWithoutCalendar
                )
            );
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<LocalTime>(
                    LocalTimePattern.ExtendedIso,
                    LocalTimePattern.GeneralIso,
                    LocalTimePattern.LongExtendedIso
                )
            );
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<Offset>(
                    OffsetPattern.GeneralInvariant,
                    OffsetPattern.GeneralInvariantWithZ
                )
            );
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<Duration>(
                    DurationPattern.JsonRoundtrip,
                    DurationPattern.Roundtrip
                )
            );
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<Period>(
                    PeriodPattern.Roundtrip,
                    PeriodPattern.NormalizingIso
                )
            );
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<OffsetDateTime>(
                    OffsetDateTimePattern.GeneralIso,
                    OffsetDateTimePattern.FullRoundtrip
                )
            );
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<OffsetDate>(
                    OffsetDatePattern.GeneralIso,
                    OffsetDatePattern.FullRoundtrip
                )
            );
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<OffsetTime>(
                    OffsetTimePattern.Rfc3339,
                    OffsetTimePattern.GeneralIso,
                    OffsetTimePattern.ExtendedIso
                )
            );
            ReplaceConverter(
                options.Converters,
                new SystemTextJsonCompositeNodaPatternConverter<ZonedDateTime>(
                    ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", DateTimeZoneProviders.Tzdb)
                )
            );

            return options;
        }

        private static void ReplaceConverter<T>(ICollection<JsonConverter> converters, SystemTextJsonCompositeNodaPatternConverter<T> converter)
        {
            foreach (var c in converters.Where(z => z.CanConvert(typeof(T))).ToArray())
            {
                converters.Remove(c);
            }

            converters.Add(converter);
        }
    }
}