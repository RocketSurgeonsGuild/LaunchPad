using System.Text.Json;
using System.Text.Json.Serialization;

using NodaTime;

namespace Rocket.Surgery.LaunchPad.NodaTime;

// LaunchPad's own NodaTime converters for the types that aren't a single pattern (see JsonConverters.md). They write the same JSON as
// NodaTime.Serialization.SystemTextJson's, but read and write the parts with the converter they are given rather than looking it up with
// JsonSerializerOptions.GetConverter(Type), which requires unreferenced and dynamic code.

// An interval as { "Start": instant, "End": instant }, either one left out when the interval is unbounded at that end.
internal sealed class NodaTimeIntervalJsonConverter(JsonConverter<Instant> instantConverter) : JsonConverter<Interval>
{
    public override Interval Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Instant? start = null;
        Instant? end = null;
        NodaTimeJsonObject.ReadProperties(
            ref reader,
            options,
            (ref Utf8JsonReader r, bool isStart) =>
            {
                var instant = instantConverter.Read(ref r, typeof(Instant), options);
                if (isStart) start = instant;
                else end = instant;
            }
        );
        return new(start, end);
    }

    public override void Write(Utf8JsonWriter writer, Interval value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStartObject();
        if (value.HasStart)
        {
            writer.WritePropertyName(NodaTimeJsonObject.Start(options));
            instantConverter.Write(writer, value.Start, options);
        }

        if (value.HasEnd)
        {
            writer.WritePropertyName(NodaTimeJsonObject.End(options));
            instantConverter.Write(writer, value.End, options);
        }

        writer.WriteEndObject();
    }
}

// A date interval as { "Start": local date, "End": local date }; both are required.
internal sealed class NodaTimeDateIntervalJsonConverter(JsonConverter<LocalDate> localDateConverter) : JsonConverter<DateInterval>
{
    public override DateInterval Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        LocalDate? start = null;
        LocalDate? end = null;
        NodaTimeJsonObject.ReadProperties(
            ref reader,
            options,
            (ref Utf8JsonReader r, bool isStart) =>
            {
                var date = localDateConverter.Read(ref r, typeof(LocalDate), options);
                if (isStart) start = date;
                else end = date;
            }
        );
        return new(
            start ?? throw new JsonException("Expected date interval; start date was missing."),
            end ?? throw new JsonException("Expected date interval; end date was missing.")
        );
    }

    public override void Write(Utf8JsonWriter writer, DateInterval value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);
        writer.WriteStartObject();
        writer.WritePropertyName(NodaTimeJsonObject.Start(options));
        localDateConverter.Write(writer, value.Start, options);
        writer.WritePropertyName(NodaTimeJsonObject.End(options));
        localDateConverter.Write(writer, value.End, options);
        writer.WriteEndObject();
    }
}

// A time zone as its id, resolved through the provider. Any DateTimeZone subclass converts, so a zone serialized as an object writes too.
internal sealed class NodaTimeDateTimeZoneJsonConverter(IDateTimeZoneProvider provider) : JsonConverter<DateTimeZone>
{
    public override bool CanConvert(Type typeToConvert) => typeof(DateTimeZone).IsAssignableFrom(typeToConvert);

    public override DateTimeZone Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => provider[ReadId(ref reader)];

    public override DateTimeZone ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        provider[ReadId(ref reader)];

    public override void Write(Utf8JsonWriter writer, DateTimeZone value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);
        writer.WriteStringValue(value.Id);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, DateTimeZone value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);
        writer.WritePropertyName(value.Id);
    }

    private static string ReadId(ref Utf8JsonReader reader) =>
        reader.TokenType is JsonTokenType.String or JsonTokenType.PropertyName
            // ReSharper disable once NullableWarningSuppressionIsUsed
            ? reader.GetString()!
            : throw new JsonException($"Unexpected token parsing time zone. Expected String, got {reader.TokenType}.");
}

internal static class NodaTimeJsonObject
{
    public delegate void ReadPart(ref Utf8JsonReader reader, bool isStart);

    // The property names NodaTime uses, through the options' naming policy.
    public static string Start(JsonSerializerOptions options) => options.PropertyNamingPolicy?.ConvertName(nameof(Interval.Start)) ?? nameof(Interval.Start);
    public static string End(JsonSerializerOptions options) => options.PropertyNamingPolicy?.ConvertName(nameof(Interval.End)) ?? nameof(Interval.End);

    // Reads an object, handing the value of each Start or End property to read, and skipping any other property.
    public static void ReadProperties(ref Utf8JsonReader reader, JsonSerializerOptions options, ReadPart read)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"Unexpected token parsing an interval. Expected StartObject, got {reader.TokenType}.");

        var comparison = options.PropertyNameCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var start = Start(options);
        var end = End(options);
        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            var name = reader.GetString();
            if (!reader.Read()) break;
            if (string.Equals(name, start, comparison)) read(ref reader, true);
            else if (string.Equals(name, end, comparison)) read(ref reader, false);
            else reader.Skip();
        }

        if (reader.TokenType != JsonTokenType.EndObject)
            throw new JsonException($"Unexpected token parsing an interval. Expected EndObject, got {reader.TokenType}.");
    }
}
