using System.IO;
using System.Text.Json;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Rocket.Surgery.Extensions.AutoMapper.NewtonsoftJson {
    static class ConverterHelpers
    {
        internal static readonly JsonElement _empty = JsonSerializer.Deserialize<JsonElement>("null");

        internal static JsonDefaultValue GetJsonDefaultValue(ResolutionContext context) => JsonDefaultValue.Default;

        internal static JsonElement GetDefaultSjt(JsonElement value, ResolutionContext context)
            => GetJsonDefaultValue(context) switch
            {
                JsonDefaultValue.NotNull => value.ValueKind == JsonValueKind.Undefined ? _empty : value,
                _                        => value
            };

        internal static JsonElement? GetDefaultSjt(JsonElement? value, ResolutionContext context)
            => GetJsonDefaultValue(context) switch
            {
                JsonDefaultValue.NotNull => !value.HasValue || value.Value.ValueKind == JsonValueKind.Undefined
                    ? _empty
                    : value.Value,
                _ => value ?? default
            };

        internal static JToken? GetDefaultToken(JToken? value, ResolutionContext context)
            => GetJsonDefaultValue(context) switch
            {
                JsonDefaultValue.NotNull => value ?? JValue.CreateNull(),
                _                        => value ?? default
            };

        internal static T? GetDefault<T>(T? value, ResolutionContext context)
            where T : JToken, new() => GetJsonDefaultValue(context) switch
        {
            JsonDefaultValue.NotNull => value ?? new T(),
            _                        => value
        };

        internal static byte[] WriteToBytes(JToken source)
        {
            using var memory = new MemoryStream();
            using var sw = new StreamWriter(memory);
            using var jw = new JsonTextWriter(sw) { Formatting = Formatting.None };
            source.WriteTo(jw);
            jw.Flush();
            memory.Position = 0;
            return memory.ToArray();
        }
    }
}