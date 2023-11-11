using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;
using NodaTime.Text;
using NodaTime.Utility;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     A JSON converter for types which can be represented by a single string value, parsed or formatted
///     from an <see cref="IPattern{T}" />.
/// </summary>
/// <typeparam name="T">The type to convert to/from JSON.</typeparam>
public sealed class NewtonsoftJsonCompositeNodaPatternConverter<T> : NodaConverterBase<T>
{
    private readonly IPattern<T>[] _patterns;
    private readonly Action<T>? _validator;

    /// <summary>
    ///     Creates a new instance with a pattern and no validator.
    /// </summary>
    /// <param name="patterns">The patterns to use for parsing and formatting.</param>
    public NewtonsoftJsonCompositeNodaPatternConverter(params IPattern<T>[] patterns) : this(null, patterns)
    {
    }

    /// <summary>
    ///     Creates a new instance with a pattern and an optional validator. The validator will be called before each
    ///     value is written, and may throw an exception to indicate that the value cannot be serialized.
    /// </summary>
    /// <param name="patterns">The patterns to use for parsing and formatting.</param>
    /// <param name="validator">The validator to call before writing values. May be null, indicating that no validation is required.</param>
    public NewtonsoftJsonCompositeNodaPatternConverter(Action<T>? validator, params IPattern<T>[] patterns)
    {
        _patterns = patterns;
        _validator = validator;
    }

    /// <summary>
    ///     Implemented by concrete subclasses, this performs the final conversion from a non-null JSON value to
    ///     a value of type T.
    /// </summary>
    /// <param name="reader">The JSON reader to pull data from</param>
    /// <param name="serializer">The serializer to use for nested serialization</param>
    /// <returns>The deserialized value of type T.</returns>
    protected override T ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.String)
        {
            throw new InvalidNodaDataException(
                $"Unexpected token parsing {typeof(T).Name}. Expected String, got {reader.TokenType}."
            );
        }

        // ReSharper disable once NullableWarningSuppressionIsUsed
        var text = reader.Value!.ToString()!;

        ParseResult<T> result = null!;
        foreach (var patter in _patterns)
        {
            result = patter.Parse(text);
            if (result.Success)
                break;
        }

        return result.Value;
    }

    /// <summary>
    ///     Writes the formatted value to the writer.
    /// </summary>
    /// <param name="writer">The writer to write JSON data to</param>
    /// <param name="value">The value to serializer</param>
    /// <param name="serializer">The serializer to use for nested serialization</param>
    protected override void WriteJsonImpl(JsonWriter writer, T value, JsonSerializer serializer)
    {
        _validator?.Invoke(value);
        writer.WriteValue(_patterns[0].Format(value));
    }
}
