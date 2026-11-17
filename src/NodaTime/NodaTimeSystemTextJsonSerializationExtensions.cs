using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.NodaTime;

/// <summary>
///     Extensions for noda time
/// </summary>
[PublicAPI]
public static class NodaTimeSystemTextJsonSerializationExtensions
{
    /// <summary>
    ///     Resolves the metadata of the NodaTime types from the converters <see cref="ConfigureNodaTimeForLaunchPad" /> adds, without reflection,
    ///     for when they are serialized at the root or as an <see cref="object" />. <see cref="ConfigureNodaTimeForLaunchPad" /> chains it.
    /// </summary>
    public static IJsonTypeInfoResolver TypeInfoResolver { get; } = new NodaTimeJsonTypeInfoResolver();

    /// <summary>
    ///     Configure System.Text.Json with defaults for launchpad
    /// </summary>
    /// <remarks>
    ///     Adds LaunchPad's converter for every NodaTime type, replacing any converter for it already in the options, and
    ///     <see cref="TypeInfoResolver" /> to the options' <see cref="JsonSerializerOptions.TypeInfoResolverChain" /> so the NodaTime types
    ///     also resolve in a trimmed or native AOT application. No reflection based resolver is added: options that should reflect over other
    ///     types need one of their own. The converters are LaunchPad's own, not NodaTime.Serialization.SystemTextJson's, which isn't native
    ///     AOT compatible yet (see JsonConverters.md in the package's source); the JSON is the same.
    /// </remarks>
    /// <param name="options"></param>
    /// <param name="dateTimeZoneProvider"></param>
    /// <returns></returns>
    public static JsonSerializerOptions ConfigureNodaTimeForLaunchPad(this JsonSerializerOptions options, IDateTimeZoneProvider dateTimeZoneProvider)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(dateTimeZoneProvider);
        var instant = ReplaceConverter(
            options.Converters,
            new SystemTextJsonCompositeNodaPatternConverter<Instant>(
                InstantPattern.ExtendedIso,
                new InstantDateTimeOffsetPattern()
            )
        );
        var localDate = ReplaceConverter(
            options.Converters,
            new SystemTextJsonCompositeNodaPatternConverter<LocalDate>(
                LocalDatePattern.Iso,
                LocalDatePattern.FullRoundtrip
            )
        );
        ReplaceConverter(
            options.Converters,
            new SystemTextJsonCompositeNodaPatternConverter<LocalDateTime>(
                LocalDateTimePattern.ExtendedIso,
                LocalDateTimePattern.BclRoundtrip
            )
        );
        ReplaceConverter(
            options.Converters,
            new SystemTextJsonCompositeNodaPatternConverter<LocalTime>(
                LocalTimePattern.ExtendedIso,
                LocalTimePattern.GeneralIso
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
                OffsetDateTimePattern.ExtendedIso,
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
            // NodaTime.Serialization.SystemTextJson's pattern; the format-only ISO patterns can't parse, so they can't be read or written.
            new SystemTextJsonCompositeNodaPatternConverter<ZonedDateTime>(
                ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", dateTimeZoneProvider)
            )
        );
        ReplaceConverter(options.Converters, new SystemTextJsonCompositeNodaPatternConverter<AnnualDate>(AnnualDatePattern.Iso));
        ReplaceConverter(options.Converters, new SystemTextJsonCompositeNodaPatternConverter<YearMonth>(YearMonthPattern.Iso));
        ReplaceConverter(options.Converters, new NodaTimeIntervalJsonConverter(instant));
        ReplaceConverter(options.Converters, new NodaTimeDateIntervalJsonConverter(localDate));
        ReplaceConverter(options.Converters, new NodaTimeDateTimeZoneJsonConverter(dateTimeZoneProvider));
        NodaTimeJsonTypeInfoResolver.AddTo(options);

        return options;
    }

    private static JsonConverter<T> ReplaceConverter<T>(IList<JsonConverter> converters, JsonConverter<T> converter)
    {
        for (var i = converters.Count - 1; i >= 0; i--)
        {
            if (converters[i].CanConvert(typeof(T))) converters.RemoveAt(i);
        }

        converters.Add(converter);
        return converter;
    }
}
