using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Extensions.AutoMapper
{
    public class JsonElementConverter :
        ITypeConverter<JsonElement, byte[]?>,
        ITypeConverter<byte[]?, JsonElement>,
        ITypeConverter<JsonElement, string?>,
        ITypeConverter<string?, JsonElement>,
        ITypeConverter<JsonElement?, byte[]?>,
        ITypeConverter<byte[]?, JsonElement?>,
        ITypeConverter<JsonElement?, string?>,
        ITypeConverter<string?, JsonElement?>,
        ITypeConverter<JsonElement, JsonElement>,
        ITypeConverter<JsonElement?, JsonElement>,
        ITypeConverter<JsonElement, JsonElement?>,
        ITypeConverter<JsonElement?, JsonElement?>
    {
        private static readonly JsonElement _empty = JsonSerializer.Deserialize<JsonElement>("null");

        private static JsonDefaultValue GetJsonDefaultValue(ResolutionContext context)
        {
            return JsonDefaultValue.Default;
        }

        private static JsonElement GetDefault(JsonElement value, ResolutionContext context) => GetJsonDefaultValue(context) switch
        {
            JsonDefaultValue.NotNull => value.ValueKind == JsonValueKind.Undefined ? _empty : value,
            _ => value
        };

        private static JsonElement? GetDefault(JsonElement? value, ResolutionContext context) => GetJsonDefaultValue(context) switch
        {
            JsonDefaultValue.NotNull => !value.HasValue || value.Value.ValueKind == JsonValueKind.Undefined
                ? _empty
                : value.Value,
            _ => value
        };

        public byte[]? Convert(JsonElement source, byte[]? destination, ResolutionContext context)
        {
            try
            {
                return source.ValueKind == JsonValueKind.Undefined
                    ? destination
                    : JsonSerializer.SerializeToUtf8Bytes(source);
            }
            catch (JsonException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse JsonElement to byte[] and failed!"
                );
                return Array.Empty<byte>();
            }
        }

        public JsonElement Convert(byte[]? source, JsonElement destination, ResolutionContext context)
        {
            try
            {
                return source == null || source.Length == 0
                    ? GetDefault(destination, context)
                    : JsonSerializer.Deserialize<JsonElement>(source);
            }
            catch (JsonException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse byte[] to JsonElement and failed!"
                );
                return GetDefault(destination, context);
            }
        }

        public string? Convert(JsonElement source, string? destination, ResolutionContext context)
        {
            try
            {
                return source.ValueKind == JsonValueKind.Undefined
                    ? destination
                    : JsonSerializer.Serialize(source);
            }
            catch (JsonException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse JsonElement to string and failed!"
                );
                return string.Empty;
            }
        }

        public JsonElement Convert(string? source, JsonElement destination, ResolutionContext context)
        {
            try
            {
                return string.IsNullOrEmpty(source)
                    ? GetDefault(destination, context)
                    : JsonSerializer.Deserialize<JsonElement>(source);

            }
            catch (JsonException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse string to JsonElement and failed!"
                );
                return GetDefault(destination, context);
            }
        }

        public byte[]? Convert(JsonElement? source, byte[]? destination, ResolutionContext context)
        {
            try
            {
                return !source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined
                        ? destination
                        : JsonSerializer.SerializeToUtf8Bytes(source);
            }
            catch (JsonException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse JsonElement? to byte[] and failed!"
                );
                return Array.Empty<byte>();
            }
        }

        public JsonElement? Convert(byte[]? source, JsonElement? destination, ResolutionContext context)
        {
            try
            {
                return source == null || source.Length == 0
                    ? GetDefault(destination, context)
                    : JsonSerializer.Deserialize<JsonElement?>(source);
            }
            catch (JsonException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse byte[] to JsonElement? and failed!"
                );
                return GetDefault(destination, context);
            }
        }

        public string? Convert(JsonElement? source, string? destination, ResolutionContext context)
        {
            try
            {
                return !source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined
                        ? destination
                        : JsonSerializer.Serialize(source);
            }
            catch (JsonException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse JsonElement? to string and failed!"
                );
                return string.Empty;
            }
        }

        public JsonElement? Convert(string? source, JsonElement? destination, ResolutionContext context)
        {
            try
            {
                return string.IsNullOrEmpty(source)
                    ? GetDefault(destination, context)
                    : JsonSerializer.Deserialize<JsonElement?>(source);
            }
            catch (JsonException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse string to JsonElement? and failed!"
                );
                return GetDefault(destination, context);
            }
        }

        public JsonElement Convert(JsonElement source, JsonElement destination, ResolutionContext context)
        {
            return source.ValueKind == JsonValueKind.Undefined ? GetDefault(destination, context) : source;
        }

        public JsonElement? Convert(JsonElement? source, JsonElement? destination, ResolutionContext context)
        {
            return source.HasValue && source.Value.ValueKind != JsonValueKind.Undefined ? source : GetDefault(destination, context);
        }

        public JsonElement? Convert(JsonElement source, JsonElement? destination, ResolutionContext context)
        {
            return source.ValueKind == JsonValueKind.Undefined ? GetDefault(destination, context) : source;
        }

        public JsonElement Convert(JsonElement? source, JsonElement destination, ResolutionContext context)
        {
            return source.HasValue && source.Value.ValueKind != JsonValueKind.Undefined ? source.Value : GetDefault(destination, context);
        }
    }
}