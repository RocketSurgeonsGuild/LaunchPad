using System.Text;
using System.Text.Json;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Rocket.Surgery.LaunchPad.Mapping.Profiles;

/// <summary>
///     Configures methods for handling well known conversions for newtonsoft json
/// </summary>
public class NewtonsoftJsonProfile : Profile
{
    /// <summary>
    ///     The default constructor
    /// </summary>
    public NewtonsoftJsonProfile()
    {
        {
            CreateMap<JToken?, byte[]?>().ConvertUsing(source => source == null || source.Type == JTokenType.None ? default : WriteToBytes(source));
            CreateMap<byte[]?, JToken?>()
               .ConvertUsing(source => source == null || source.Length == 0 ? default : JToken.Parse(Encoding.UTF8.GetString(source)));
            CreateMap<JToken?, string?>().ConvertUsing(source => source == default ? default : source.ToString(Formatting.None));
            CreateMap<string?, JToken?>().ConvertUsing(source => string.IsNullOrEmpty(source) ? default : JToken.Parse(source));
            CreateMap<JArray?, byte[]?>().ConvertUsing(source => source == null || source.Type == JTokenType.None ? default : WriteToBytes(source));
            CreateMap<byte[]?, JArray?>()
               .ConvertUsing(source => source == null || source.Length == 0 ? default : JArray.Parse(Encoding.UTF8.GetString(source)));
            CreateMap<JArray?, string?>().ConvertUsing(source => source == null ? default : source.ToString(Formatting.None));
            CreateMap<string?, JArray?>().ConvertUsing(source => string.IsNullOrEmpty(source) ? default : JArray.Parse(source));
            CreateMap<JObject?, byte[]?>().ConvertUsing(source => source == null || source.Type == JTokenType.None ? default : WriteToBytes(source));
            CreateMap<byte[]?, JObject?>()
               .ConvertUsing(source => source == null || source.Length == 0 ? default : JObject.Parse(Encoding.UTF8.GetString(source)));
            CreateMap<JObject?, string?>().ConvertUsing(source => source == null ? default : source.ToString(Formatting.None));
            CreateMap<string?, JObject?>().ConvertUsing(source => string.IsNullOrEmpty(source) ? default : JObject.Parse(source));
        }
        {
            CreateMap<JObject?, JsonElement>()
               .ConvertUsing(source => source == null ? default : JsonDocument.Parse(WriteToBytes(source), default).RootElement.Clone());
            CreateMap<JsonElement, JObject?>()
               .ConvertUsing(
                    source => source.ValueKind == JsonValueKind.Undefined
                        ? default
                        : JObject.Parse(JsonSerializer.Serialize(source, (JsonSerializerOptions?)default))
                );
            CreateMap<JObject?, JsonElement?>().ConvertUsing(
                source => source == default ? default(JsonElement?) : JsonDocument.Parse(WriteToBytes(source), default).RootElement.Clone()
            );
            CreateMap<JsonElement?, JObject?>().ConvertUsing(
                source => !source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined
                    ? default
                    : JObject.Parse(JsonSerializer.Serialize(source, (JsonSerializerOptions?)default))
            );
            CreateMap<JArray?, JsonElement>()
               .ConvertUsing(source => source == default ? default : JsonDocument.Parse(WriteToBytes(source), default).RootElement.Clone());
            CreateMap<JsonElement, JArray?>()
               .ConvertUsing(
                    source => source.ValueKind == JsonValueKind.Undefined
                        ? default
                        : JArray.Parse(JsonSerializer.Serialize(source, (JsonSerializerOptions?)default))
                );
            CreateMap<JArray?, JsonElement?>().ConvertUsing(
                source => source == null ? default(JsonElement?) : JsonDocument.Parse(WriteToBytes(source), default).RootElement.Clone()
            );
            CreateMap<JsonElement?, JArray?>().ConvertUsing(
                source => !source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined
                    ? default
                    : JArray.Parse(JsonSerializer.Serialize(source, (JsonSerializerOptions?)default))
            );
            CreateMap<JToken?, JsonElement>()
               .ConvertUsing(source => source == null ? default : JsonDocument.Parse(WriteToBytes(source), default).RootElement.Clone());
            CreateMap<JsonElement, JToken?>()
               .ConvertUsing(
                    source => source.ValueKind == JsonValueKind.Undefined
                        ? default
                        : JToken.Parse(JsonSerializer.Serialize(source, (JsonSerializerOptions?)default), default)
                );
            CreateMap<JToken?, JsonElement?>().ConvertUsing(
                source => source == default ? default(JsonElement?) : JsonDocument.Parse(WriteToBytes(source), default).RootElement.Clone()
            );
            CreateMap<JsonElement?, JToken?>().ConvertUsing(
                source => !source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined
                    ? default
                    : JToken.Parse(JsonSerializer.Serialize(source, (JsonSerializerOptions?)default))
            );
        }
    }

    /// <summary>
    ///     Gets the name of the profile.
    /// </summary>
    /// <value>The name of the profile.</value>
    public override string ProfileName => nameof(NewtonsoftJsonProfile);
}
