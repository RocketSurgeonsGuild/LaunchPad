using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using NodaTime;

namespace Rocket.Surgery.LaunchPad.NodaTime;

// Builds the metadata of the NodaTime types from the converters in the options, so they need no reflection or source generated context.
internal sealed class NodaTimeJsonTypeInfoResolver : IJsonTypeInfoResolver
{
    private static readonly Dictionary<Type, Func<JsonSerializerOptions, JsonTypeInfo?>> Factories = new()
    {
        [typeof(Instant)] = Create<Instant>,
        [typeof(Instant?)] = CreateNullable<Instant>,
        [typeof(LocalDate)] = Create<LocalDate>,
        [typeof(LocalDate?)] = CreateNullable<LocalDate>,
        [typeof(LocalDateTime)] = Create<LocalDateTime>,
        [typeof(LocalDateTime?)] = CreateNullable<LocalDateTime>,
        [typeof(LocalTime)] = Create<LocalTime>,
        [typeof(LocalTime?)] = CreateNullable<LocalTime>,
        [typeof(Offset)] = Create<Offset>,
        [typeof(Offset?)] = CreateNullable<Offset>,
        [typeof(OffsetDateTime)] = Create<OffsetDateTime>,
        [typeof(OffsetDateTime?)] = CreateNullable<OffsetDateTime>,
        [typeof(OffsetDate)] = Create<OffsetDate>,
        [typeof(OffsetDate?)] = CreateNullable<OffsetDate>,
        [typeof(OffsetTime)] = Create<OffsetTime>,
        [typeof(OffsetTime?)] = CreateNullable<OffsetTime>,
        [typeof(ZonedDateTime)] = Create<ZonedDateTime>,
        [typeof(ZonedDateTime?)] = CreateNullable<ZonedDateTime>,
        [typeof(Duration)] = Create<Duration>,
        [typeof(Duration?)] = CreateNullable<Duration>,
        [typeof(Interval)] = Create<Interval>,
        [typeof(Interval?)] = CreateNullable<Interval>,
        [typeof(AnnualDate)] = Create<AnnualDate>,
        [typeof(AnnualDate?)] = CreateNullable<AnnualDate>,
        [typeof(Period)] = Create<Period>,
        [typeof(DateInterval)] = Create<DateInterval>,
        [typeof(DateTimeZone)] = Create<DateTimeZone>,
    };

    public static void AddTo(JsonSerializerOptions options)
    {
        var chain = options.TypeInfoResolverChain;
        if (chain.Any(x => x is NodaTimeJsonTypeInfoResolver)) return;
        chain.Add(NodaTimeSystemTextJsonSerializationExtensions.TypeInfoResolver);
    }

    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options) => Factories.TryGetValue(type, out var factory) ? factory(options) : null;

    private static JsonTypeInfo? Create<T>(JsonSerializerOptions options)
    {
        foreach (var converter in options.Converters)
        {
            if (!converter.CanConvert(typeof(T))) continue;
            var typed = converter is JsonConverterFactory factory ? factory.CreateConverter(typeof(T), options) : converter;
            if (typed is JsonConverter<T>) return JsonMetadataServices.CreateValueInfo<T>(options, typed);
        }

        return null;
    }

    private static JsonTypeInfo? CreateNullable<T>(JsonSerializerOptions options) where T : struct =>
        Create<T>(options) is JsonTypeInfo<T> underlying
            ? JsonMetadataServices.CreateValueInfo<T?>(options, JsonMetadataServices.GetNullableConverter(underlying))
            : null;
}
