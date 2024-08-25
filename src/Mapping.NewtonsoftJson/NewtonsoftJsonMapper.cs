using System.Text.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Riok.Mapperly.Abstractions;
using static Rocket.Surgery.LaunchPad.Mapping.ConverterHelpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
/// Configures methods for handling well-known conversions for Newtonsoft.Json
/// </summary>
[Mapper, PublicAPI]
public partial class NewtonsoftJsonMapper(IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions)
{
    /// <summary>
    /// Converts a <see cref="JObject"/> to a <see cref="JsonElement"/>.
    /// </summary>
    /// <param name="source">The <see cref="JObject"/> to convert.</param>
    /// <returns>A <see cref="JsonElement"/> representation of the <see cref="JObject"/>, or the default <see cref="JsonElement"/> if the <see cref="JObject"/> is null.</returns>
    public JsonElement FromJObject(JObject? source) => source == null ? default : JsonDocument.Parse(WriteToBytes(source)).RootElement.Clone();

    /// <summary>
    /// Converts a <see cref="JsonElement"/> to a <see cref="JObject"/>.
    /// </summary>
    /// <param name="source">The <see cref="JsonElement"/> to convert.</param>
    /// <returns>A <see cref="JObject"/> representation of the <see cref="JsonElement"/>, or null if the <see cref="JsonElement"/> is undefined.</returns>
    public JObject? ToJObject(JsonElement source) => source.ValueKind == JsonValueKind.Undefined
        ? default
        : JObject.Parse(JsonSerializer.Serialize(source, jsonSerializerOptions.CurrentValue));

    /// <summary>
    /// Converts a <see cref="JArray"/> to a <see cref="JsonElement"/>.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to convert.</param>
    /// <returns>A <see cref="JsonElement"/> representation of the <see cref="JArray"/>, or the default <see cref="JsonElement"/> if the <see cref="JArray"/> is null.</returns>
    public JsonElement FromJArray(JArray? source) => source == default ? default : JsonDocument.Parse(WriteToBytes(source)).RootElement.Clone();

    /// <summary>
    /// Converts a <see cref="JsonElement"/> to a <see cref="JArray"/>.
    /// </summary>
    /// <param name="source">The <see cref="JsonElement"/> to convert.</param>
    /// <returns>A <see cref="JArray"/> representation of the <see cref="JsonElement"/>, or null if the <see cref="JsonElement"/> is undefined.</returns>
    public JArray? ToJArray(JsonElement source) => source.ValueKind == JsonValueKind.Undefined
        ? default
        : JArray.Parse(JsonSerializer.Serialize(source, jsonSerializerOptions.CurrentValue));

    /// <summary>
    /// Converts a <see cref="JToken"/> to a <see cref="JsonElement"/>.
    /// </summary>
    /// <param name="source">The <see cref="JToken"/> to convert.</param>
    /// <returns>A <see cref="JsonElement"/> representation of the <see cref="JToken"/>, or the default <see cref="JsonElement"/> if the <see cref="JToken"/> is null.</returns>
    public JsonElement FromJToken(JToken? source) => source == null ? default : JsonDocument.Parse(WriteToBytes(source)).RootElement.Clone();

    /// <summary>
    /// Converts a <see cref="JsonElement"/> to a <see cref="JToken"/>.
    /// </summary>
    /// <param name="source">The <see cref="JsonElement"/> to convert.</param>
    /// <returns>A <see cref="JToken"/> representation of the <see cref="JsonElement"/>, or null if the <see cref="JsonElement"/> is undefined.</returns>
    public JToken? ToJToken(JsonElement source) => source.ValueKind == JsonValueKind.Undefined
        ? default
        : JToken.Parse(JsonSerializer.Serialize(source, jsonSerializerOptions.CurrentValue));
}
