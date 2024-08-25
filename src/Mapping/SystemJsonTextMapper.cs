using System.Text.Json;
using Microsoft.Extensions.Options;
using Riok.Mapperly.Abstractions;

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
/// Configures methods for well-known conversions with STJ
/// </summary>
/// <param name="options">The <see cref="IOptions{JsonSerializerOptions}"/> to use for serialization.</param>
[Mapper, PublicAPI]
public partial class SystemJsonTextMapper(IOptionsMonitor<JsonSerializerOptions> options)
{
    /// <summary>
    /// Converts a <see cref="JsonElement"/> to a byte array.
    /// </summary>
    /// <param name="source">The <see cref="JsonElement"/> to convert.</param>
    /// <returns>A byte array representation of the <see cref="JsonElement"/>, or null if the <see cref="JsonElement"/> is undefined.</returns>
    public byte[]? JsonElementToByteArray(JsonElement source) =>
        source.ValueKind == JsonValueKind.Undefined
            ? null
            : JsonSerializer.SerializeToUtf8Bytes(source, options.CurrentValue);

    /// <summary>
    /// Converts a <see cref="JsonElement"/> to a string.
    /// </summary>
    /// <param name="source">The <see cref="JsonElement"/> to convert.</param>
    /// <returns>A string representation of the <see cref="JsonElement"/>, or null if the <see cref="JsonElement"/> is undefined.</returns>
    public string? JsonElementToString(JsonElement source) =>
        source.ValueKind == JsonValueKind.Undefined
            ? null
            : JsonSerializer.Serialize(source, options.CurrentValue);

    /// <summary>
    /// Converts a byte array to a <see cref="JsonElement"/>.
    /// </summary>
    /// <param name="source">The byte array to convert.</param>
    /// <returns>A <see cref="JsonElement"/> representation of the byte array, or the default <see cref="JsonElement"/> if the byte array is null or empty.</returns>
    public JsonElement ByteArrayToJsonElement(byte[]? source) =>
        source is null or { Length: 0 }
            ? default
            : JsonSerializer.Deserialize<JsonElement>(source, options.CurrentValue);

    /// <summary>
    /// Converts a string to a <see cref="JsonElement"/>.
    /// </summary>
    /// <param name="source">The string to convert.</param>
    /// <returns>A <see cref="JsonElement"/> representation of the string, or the default <see cref="JsonElement"/> if the string is null or empty.</returns>
    public JsonElement StringToJsonElement(string? source) =>
        source is null or { Length: 0 }
            ? default
            : JsonSerializer.Deserialize<JsonElement>(source, options.CurrentValue);
}
