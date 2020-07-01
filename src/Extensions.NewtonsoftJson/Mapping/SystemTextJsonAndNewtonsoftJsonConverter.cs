using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Rocket.Surgery.Extensions.AutoMapper.NewtonsoftJson {
    public class SystemTextJsonAndNewtonsoftJsonConverter :
        ITypeConverter<JsonElement?, JToken>,
        ITypeConverter<JToken?, JsonElement?>,
        ITypeConverter<JsonElement, JToken?>,
        ITypeConverter<JToken?, JsonElement>,
        ITypeConverter<JsonElement?, JArray?>,
        ITypeConverter<JArray?, JsonElement?>,
        ITypeConverter<JsonElement, JArray?>,
        ITypeConverter<JArray?, JsonElement>,
        ITypeConverter<JsonElement?, JObject?>,
        ITypeConverter<JObject?, JsonElement?>,
        ITypeConverter<JsonElement, JObject?>,
        ITypeConverter<JObject?, JsonElement>
    {
        

        public JsonElement Convert(JObject? source, JsonElement destination, ResolutionContext context)
        {
            if (source is null)
            {
                return ConverterHelpers.GetDefaultSjt(destination, context);
            }

            var data = ConverterHelpers.WriteToBytes(source);
            var document = JsonDocument.Parse(data);
            return document.RootElement;
        }

        public JObject? Convert(JsonElement source, JObject? destination, ResolutionContext context)
        {
            if (source.ValueKind == JsonValueKind.Undefined)
                return ConverterHelpers.GetDefault(destination, context);
            if (source.ValueKind != JsonValueKind.Object)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogWarning(
                    "Tried to convert non array JsonElement to JObject"
                );
                return ConverterHelpers.GetDefault(destination, context);
            }

            return JObject.Parse(JsonSerializer.Serialize(source));
        }

        public JsonElement? Convert(JObject? source, JsonElement? destination, ResolutionContext context)
        {
            if (source is null)
            {
                return ConverterHelpers.GetDefaultSjt(destination, context);
            }

            var data = ConverterHelpers.WriteToBytes(source);
            var document = JsonDocument.Parse(data);
            return document.RootElement;
        }

        public JObject? Convert(JsonElement? source, JObject? destination, ResolutionContext context)
        {
            if (!source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined)
                return ConverterHelpers.GetDefault(destination, context);
            if (source.Value.ValueKind != JsonValueKind.Object)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogWarning(
                    "Tried to convert non array JsonElement? to JObject"
                );
                return ConverterHelpers.GetDefault(destination, context);
            }

            return JObject.Parse(JsonSerializer.Serialize(source));
        }

        public JsonElement Convert(JArray? source, JsonElement destination, ResolutionContext context)
        {
            if (source is null)
            {
                return ConverterHelpers.GetDefaultSjt(destination, context);
            }

            var data = ConverterHelpers.WriteToBytes(source);
            var document = JsonDocument.Parse(data);
            return document.RootElement;
        }

        public JArray? Convert(JsonElement source, JArray? destination, ResolutionContext context)
        {
            if (source.ValueKind == JsonValueKind.Undefined)
                return ConverterHelpers.GetDefault(destination, context);
            if (source.ValueKind != JsonValueKind.Array)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogWarning(
                    "Tried to convert non array JsonElement to JArray"
                );
                return ConverterHelpers.GetDefault(destination, context);
            }

            return JArray.Parse(JsonSerializer.Serialize(source));
        }

        public JsonElement? Convert(JArray? source, JsonElement? destination, ResolutionContext context)
        {
            if (source is null)
            {
                return ConverterHelpers.GetDefaultSjt(destination, context);
            }

            var data = ConverterHelpers.WriteToBytes(source);
            var document = JsonDocument.Parse(data);
            return document.RootElement;
        }

        public JArray? Convert(JsonElement? source, JArray? destination, ResolutionContext context)
        {
            if (!source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined)
                return ConverterHelpers.GetDefault(destination, context);
            if (source.Value.ValueKind != JsonValueKind.Array)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogWarning(
                    "Tried to convert non array JsonElement? to JArray"
                );
                return ConverterHelpers.GetDefault(destination, context);
            }

            return JArray.Parse(JsonSerializer.Serialize(source));
        }

        public JsonElement Convert(JToken? source, JsonElement destination, ResolutionContext context)
        {
            if (source is null)
            {
                return ConverterHelpers.GetDefaultSjt(destination, context);
            }

            var data = ConverterHelpers.WriteToBytes(source);
            var document = JsonDocument.Parse(data);
            return document.RootElement;
        }

        public JToken? Convert(JsonElement source, JToken? destination, ResolutionContext context)
        {
            if (source.ValueKind == JsonValueKind.Undefined)
                return ConverterHelpers.GetDefaultToken(destination, context);
            return JToken.Parse(JsonSerializer.Serialize(source));
        }

        public JsonElement? Convert(JToken? source, JsonElement? destination, ResolutionContext context)
        {
            if (source is null)
            {
                return ConverterHelpers.GetDefaultSjt(destination, context);
            }

            var data = ConverterHelpers.WriteToBytes(source);
            var document = JsonDocument.Parse(data);
            return document.RootElement;
        }

        public JToken? Convert(JsonElement? source, JToken? destination, ResolutionContext context)
        {
            if (!source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined)
                return ConverterHelpers.GetDefaultToken(destination, context);
            return JToken.Parse(JsonSerializer.Serialize(source));
        }

    }
}