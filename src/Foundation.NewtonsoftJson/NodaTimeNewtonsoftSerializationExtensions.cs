using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Extensions for noda time
/// </summary>
[PublicAPI]
public static class NodaTimeSerializationExtensions
{
    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="dateTimeZoneProvider"></param>
    /// <returns></returns>
    public static JsonSerializer ConfigureNodaTimeForLaunchPad(this JsonSerializer options, IDateTimeZoneProvider dateTimeZoneProvider)
    {
        options.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        options.ConfigureForNodaTime(dateTimeZoneProvider);
        ReplaceConverters(options.Converters, dateTimeZoneProvider);
        return options;
    }

    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <param name="options"></param>
    /// <param name="dateTimeZoneProvider"></param>
    /// <returns></returns>
    public static JsonSerializerSettings ConfigureNodaTimeForLaunchPad(this JsonSerializerSettings options, IDateTimeZoneProvider dateTimeZoneProvider)
    {
        options.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        options.ConfigureForNodaTime(dateTimeZoneProvider);
        ReplaceConverters(options.Converters, dateTimeZoneProvider);

        return options;
    }

    private static void ReplaceConverters(IList<JsonConverter> converters, IDateTimeZoneProvider dateTimeZoneProvider)
    {
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<Instant>(
                InstantPattern.ExtendedIso,
                InstantPattern.General,
                new InstantDateTimeOffsetPattern()
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<LocalDate>(
                LocalDatePattern.Iso,
                LocalDatePattern.FullRoundtrip
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<LocalDateTime>(
                LocalDateTimePattern.ExtendedIso,
                LocalDateTimePattern.GeneralIso,
                LocalDateTimePattern.BclRoundtrip
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<LocalTime>(
                LocalTimePattern.ExtendedIso,
                LocalTimePattern.GeneralIso
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<Offset>(
                OffsetPattern.GeneralInvariant,
                OffsetPattern.GeneralInvariantWithZ
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<Duration>(
                DurationPattern.JsonRoundtrip,
                DurationPattern.Roundtrip
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<Duration>(
                DurationPattern.JsonRoundtrip,
                DurationPattern.Roundtrip
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<Period>(
                PeriodPattern.Roundtrip,
                PeriodPattern.NormalizingIso
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<OffsetDateTime>(
                OffsetDateTimePattern.GeneralIso,
                OffsetDateTimePattern.FullRoundtrip
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<OffsetDate>(
                OffsetDatePattern.GeneralIso,
                OffsetDatePattern.FullRoundtrip
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<OffsetTime>(
                OffsetTimePattern.Rfc3339,
                OffsetTimePattern.GeneralIso,
                OffsetTimePattern.ExtendedIso
            )
        );
        ReplaceConverter(
            converters,
            new NewtonsoftJsonCompositeNodaPatternConverter<ZonedDateTime>(
                ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", dateTimeZoneProvider)
            )
        );
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
