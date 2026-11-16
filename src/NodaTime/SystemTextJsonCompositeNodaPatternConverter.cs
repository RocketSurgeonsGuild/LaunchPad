using System.Text.Json;
using System.Text.Json.Serialization;

using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.NodaTime;

/// <summary>
///     A JSON converter for types which can be represented by a single string value, parsed or formatted
///     from an <see cref="IPattern{T}" />.
/// </summary>
/// <typeparam name="T">The type to convert to/from JSON.</typeparam>
/// <remarks>
///     <para>
///         Creates a new instance with a pattern and an optional validator. The validator will be called before each
///         value is written, and may throw an exception to indicate that the value cannot be serialized.
///     </para>
///     <para>
///         The first pattern formats; reading tries each pattern in turn. The value is also read and written as a property name, so the type can
///         be a dictionary key. A value no pattern parses throws <see cref="JsonException" />.
///     </para>
/// </remarks>
/// <param name="validator">The validator to call before writing values. May be null, indicating that no validation is required.</param>
/// <param name="patterns">The patterns to use for parsing and formatting.</param>
public sealed class SystemTextJsonCompositeNodaPatternConverter<T>(Action<T>? validator, params IPattern<T>[] patterns) : JsonConverter<T>
{
    /// <summary>
    ///     Creates a new instance with a pattern and no validator.
    /// </summary>
    /// <param name="patterns">The patterns to use for parsing and formatting.</param>
    public SystemTextJsonCompositeNodaPatternConverter(params IPattern<T>[] patterns) : this(null, patterns) { }

    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Parse(ref reader);

    /// <inheritdoc />
    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Parse(ref reader);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue(Format(value));
    }

    /// <inheritdoc />
    public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WritePropertyName(Format(value));
    }

    private T Parse(ref Utf8JsonReader reader)
    {
        if (reader.TokenType is not ( JsonTokenType.String or JsonTokenType.PropertyName ))
            throw new JsonException($"Unexpected token parsing {typeof(T).Name}. Expected String, got {reader.TokenType}.");

        // ReSharper disable once NullableWarningSuppressionIsUsed
        var text = reader.GetString()!;
        Exception? failure = null;
        foreach (var pattern in _patterns)
        {
            var result = pattern.Parse(text);
            if (result.Success) return result.Value;
            failure ??= result.Exception;
        }

        throw new JsonException($"Unable to parse '{text}' as {typeof(T).Name}.", failure);
    }

    private string Format(T value)
    {
        _validator?.Invoke(value);
        return _patterns[0].Format(value);
    }

    private readonly IPattern<T>[] _patterns = patterns;
    private readonly Action<T>? _validator = validator;
}
