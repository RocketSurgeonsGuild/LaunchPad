using System.Text.Json;

using NodaTime.Serialization.SystemTextJson;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     A JSON converter for types which can be represented by a single string value, parsed or formatted
///     from an <see cref="IPattern{T}" />.
/// </summary>
/// <typeparam name="T">The type to convert to/from JSON.</typeparam>
/// <remarks>
///     Creates a new instance with a pattern and an optional validator. The validator will be called before each
///     value is written, and may throw an exception to indicate that the value cannot be serialized.
/// </remarks>
/// <param name="validator">The validator to call before writing values. May be null, indicating that no validation is required.</param>
/// <param name="patterns">The patterns to use for parsing and formatting.</param>
public sealed class SystemTextJsonCompositeNodaPatternConverter<T>(Action<T>? validator, params IPattern<T>[] patterns) : NodaConverterBase<T>
{
    /// <summary>
    ///     Creates a new instance with a pattern and no validator.
    /// </summary>
    /// <param name="patterns">The patterns to use for parsing and formatting.</param>
    public SystemTextJsonCompositeNodaPatternConverter(params IPattern<T>[] patterns) : this(null, patterns) { }

    /// <inheritdoc />
    protected override T ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var text = reader.GetString()!;

        // ReSharper disable once NullableWarningSuppressionIsUsed
        ParseResult<T> result = null!;
        foreach (var patter in _patterns)
        {
            result = patter.Parse(text);
            if (result.Success)
                break;
        }

        return result.Value;
    }

    /// <inheritdoc />
    protected override void WriteJsonImpl(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        _validator?.Invoke(value);
        writer.WriteStringValue(_patterns[0].Format(value));
    }

    private readonly IPattern<T>[] _patterns = patterns;
    private readonly Action<T>? _validator = validator;
}
