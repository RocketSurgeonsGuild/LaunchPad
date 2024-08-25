using System.Text.Json;
using Microsoft.Extensions.Options;
using Riok.Mapperly.Abstractions;

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
/// Configures methods for well-known conversions with STJ
/// </summary>
/// <param name="options">The <see cref="IOptions{JsonSerializerOptions}"/> to use for serialization.</param>
[Mapper, PublicAPI]
public partial class SystemTextJsonMapper(IOptionsMonitor<JsonSerializerOptions> options)
{
    /// <summary>
    /// Converts a <see cref="JsonElement"/> to a byte array.
    /// </summary>
    /// <param name="source">The <see cref="JsonElement"/> to convert.</param>
    /// <returns>A byte array representation of the <see cref="JsonElement"/>, or null if the <see cref="JsonElement"/> is undefined.</returns>
    [UserMapping]
    public byte[]? JsonElementToByteArray(JsonElement source) =>
        source is { ValueKind: not JsonValueKind.Undefined }
            ? JsonSerializer.SerializeToUtf8Bytes(source, options.CurrentValue)
            : null;

    /// <summary>
    /// Converts a nullable <see cref="JsonElement"/> to a byte array.
    /// </summary>
    /// <param name="source">The <see cref="JsonElement"/> to convert.</param>
    /// <returns>A byte array representation of the <see cref="JsonElement"/>, or null if the <see cref="JsonElement"/> is undefined.</returns>
    [UserMapping]
    public byte[]? JsonElementToByteArray(JsonElement? source) =>
        source is { ValueKind: not JsonValueKind.Undefined }
            ? JsonSerializer.SerializeToUtf8Bytes(source, options.CurrentValue)
            : null;

    /// <summary>
    /// Converts a <see cref="JsonElement"/> to a string.
    /// </summary>
    /// <param name="source">The <see cref="JsonElement"/> to convert.</param>
    /// <returns>A string representation of the <see cref="JsonElement"/>, or null if the <see cref="JsonElement"/> is undefined.</returns>
    [UserMapping]
    public string? JsonElementToString(JsonElement source) =>
        source is { ValueKind: not JsonValueKind.Undefined  }
            ? JsonSerializer.Serialize(source, options.CurrentValue)
            : null;

    /// <summary>
    /// Converts a nullable <see cref="JsonElement"/> to a string.
    /// </summary>
    /// <param name="source">The <see cref="JsonElement"/> to convert.</param>
    /// <returns>A string representation of the <see cref="JsonElement"/>, or null if the <see cref="JsonElement"/> is undefined.</returns>
    [UserMapping]
    public string? JsonElementToString(JsonElement? source) =>
        source is { ValueKind: not JsonValueKind.Undefined  }
            ? JsonSerializer.Serialize(source.Value, options.CurrentValue)
            : null;

    /// <summary>
    /// Converts a byte array to a <see cref="JsonElement"/>.
    /// </summary>
    /// <param name="source">The byte array to convert.</param>
    /// <returns>A <see cref="JsonElement"/> representation of the byte array, or the default <see cref="JsonElement"/> if the byte array is null or empty.</returns>
    [UserMapping]
    public JsonElement ByteArrayToJsonElement(byte[]? source) =>
        source switch
        {
            null or { Length: 0 } => new(),
            _             => JsonSerializer.Deserialize<JsonElement>(source, options.CurrentValue)
        };

    /// <summary>
    /// Converts a byte array to a <see cref="JsonElement"/>.
    /// </summary>
    /// <param name="source">The byte array to convert.</param>
    /// <returns>A <see cref="JsonElement"/> representation of the byte array, or the default <see cref="JsonElement"/> if the byte array is null or empty.</returns>
    [UserMapping]
    public JsonElement? ByteArrayToNullableJsonElement(byte[]? source) =>
        source switch
        {
            null => null,
            { Length: 0 } => new(),
            _ => JsonSerializer.Deserialize<JsonElement>(source, options.CurrentValue)
        };

    /// <summary>
    /// Converts a string to a <see cref="JsonElement"/>.
    /// </summary>
    /// <param name="source">The string to convert.</param>
    /// <returns>A <see cref="JsonElement"/> representation of the string, or the default <see cref="JsonElement"/> if the string is null or empty.</returns>
    [UserMapping]
    public JsonElement StringToJsonElement(string? source) =>
        source switch
        {
            null or { Length: 0 } => new(),
            _             => JsonSerializer.Deserialize<JsonElement>(source, options.CurrentValue)
        };

    /// <summary>
    /// Converts a string to a <see cref="JsonElement"/>.
    /// </summary>
    /// <param name="source">The string to convert.</param>
    /// <returns>A <see cref="JsonElement"/> representation of the string, or the default <see cref="JsonElement"/> if the string is null or empty.</returns>
    [UserMapping]
    public JsonElement? StringToNullableJsonElement(string? source) =>
        source switch
        {
            null          => null,
            { Length: 0 } => new(),
            _             => JsonSerializer.Deserialize<JsonElement>(source, options.CurrentValue)
        };
}
