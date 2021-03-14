using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.Text;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.Foundation
{
    /// <summary>
    /// Extensions for noda time
    /// </summary>
    [PublicAPI]
    public static class NodaTimeSerializationExtensions
    {
        /// <summary>
        /// Configure System.Text.Json with defaults for launchpad
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dateTimeZoneProvider"></param>
        /// <returns></returns>
        public static JsonSerializerSettings ConfigureNodaTimeForLaunchPad(this JsonSerializerSettings options, IDateTimeZoneProvider dateTimeZoneProvider)
        {
            options.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            options.ConfigureForNodaTime(dateTimeZoneProvider);
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<Instant>(
                    InstantPattern.ExtendedIso,
                    InstantPattern.General,
                    new NewtonsoftJsonDateTimeOffsetPattern()
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<LocalDate>(
                    LocalDatePattern.Iso,
                    LocalDatePattern.FullRoundtrip
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<LocalDateTime>(
                    LocalDateTimePattern.ExtendedIso,
                    LocalDateTimePattern.GeneralIso,
                    LocalDateTimePattern.BclRoundtrip,
                    LocalDateTimePattern.FullRoundtrip,
                    LocalDateTimePattern.FullRoundtripWithoutCalendar
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<LocalTime>(
                    LocalTimePattern.ExtendedIso,
                    LocalTimePattern.GeneralIso,
                    LocalTimePattern.LongExtendedIso
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<Offset>(
                    OffsetPattern.GeneralInvariant,
                    OffsetPattern.GeneralInvariantWithZ
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<Duration>(
                    DurationPattern.JsonRoundtrip,
                    DurationPattern.Roundtrip
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<Duration>(
                    DurationPattern.JsonRoundtrip,
                    DurationPattern.Roundtrip
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<Period>(
                    PeriodPattern.Roundtrip,
                    PeriodPattern.NormalizingIso
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<OffsetDateTime>(
                    OffsetDateTimePattern.GeneralIso,
                    OffsetDateTimePattern.FullRoundtrip
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<OffsetDate>(
                    OffsetDatePattern.GeneralIso,
                    OffsetDatePattern.FullRoundtrip
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<OffsetTime>(
                    OffsetTimePattern.Rfc3339,
                    OffsetTimePattern.GeneralIso,
                    OffsetTimePattern.ExtendedIso
                )
            );
            ReplaceConverter(
                options.Converters,
                new NewtonsoftJsonCompositeNodaPatternConverter<ZonedDateTime>(
                    ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", DateTimeZoneProviders.Tzdb)
                )
            );

            return options;
        }

        private static void ReplaceConverter<T>(ICollection<JsonConverter> converters, NewtonsoftJsonCompositeNodaPatternConverter<T> converter)
        {
            foreach (var c in converters.Where(z => z.CanConvert(typeof(T))).ToArray())
            {
                converters.Remove(c);
            }

            converters.Add(converter);
        }
    }
}