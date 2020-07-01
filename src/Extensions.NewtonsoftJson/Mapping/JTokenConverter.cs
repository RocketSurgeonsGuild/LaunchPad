using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonException = System.Text.Json.JsonException;
using static Rocket.Surgery.Extensions.AutoMapper.NewtonsoftJson.ConverterHelpers;

namespace Rocket.Surgery.Extensions.AutoMapper.NewtonsoftJson
{
    public class JTokenConverter :
        ITypeConverter<JToken, byte[]?>,
        ITypeConverter<byte[]?, JToken?>,
        ITypeConverter<JToken?, string?>,
        ITypeConverter<string?, JToken?>,
        ITypeConverter<JArray?, byte[]?>,
        ITypeConverter<byte[]?, JArray?>,
        ITypeConverter<JArray?, string?>,
        ITypeConverter<string?, JArray?>,
        ITypeConverter<JObject?, byte[]?>,
        ITypeConverter<byte[]?, JObject?>,
        ITypeConverter<JObject?, string?>,
        ITypeConverter<string?, JObject?>,
        ITypeConverter<JObject?, JObject?>,
        ITypeConverter<JArray?, JArray?>,
        ITypeConverter<JToken?, JToken?>
    {
        public byte[]? Convert(JToken? source, byte[]? destination, ResolutionContext context)
        {
            if (source == null || source.Type == JTokenType.None)
                return destination;
            return WriteToBytes(source);
        }

        public JToken? Convert(byte[]? source, JToken? destination, ResolutionContext context)
        {
            return source == null || source.Length == 0
                ? GetDefaultToken(destination, context)
                : JToken.Parse(Encoding.UTF8.GetString(source));
        }


        public string? Convert(JToken? source, string? destination, ResolutionContext context)
            => source?.ToString(Formatting.None) ?? destination;


        public JToken? Convert(string? source, JToken? destination, ResolutionContext context)
        {
            return string.IsNullOrEmpty(source) ? GetDefaultToken(destination, context) : JToken.Parse(source);
        }

        public byte[]? Convert(JArray? source, byte[]? destination, ResolutionContext context)
        {
            if (source == null || source.Type == JTokenType.None)
                return destination;
            return WriteToBytes(source);
        }

        public JArray? Convert(byte[]? source, JArray? destination, ResolutionContext context)
        {
            try
            {
                return source == null || source.Length == 0
                    ? GetDefault(destination, context)
                    : JArray.Parse(Encoding.UTF8.GetString(source));
            }
            catch (JsonReaderException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse byte[] to JArray and failed!"
                );
                return GetDefault(destination, context);
            }
        }

        public string? Convert(JArray? source, string? destination, ResolutionContext context)
            => source?.ToString(Formatting.None) ?? destination;

        public JArray? Convert(string? source, JArray? destination, ResolutionContext context)
        {
            try
            {
                return string.IsNullOrEmpty(source) ? GetDefault(destination, context) : JArray.Parse(source);
            }
            catch (JsonReaderException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse string to JArray and failed!"
                );
                return GetDefault(destination, context);
            }
        }

        public byte[]? Convert(JObject? source, byte[]? destination, ResolutionContext context)
        {
            if (source == null || source.Type == JTokenType.None)
                return destination;
            return WriteToBytes(source);
        }

        public JObject? Convert(byte[]? source, JObject? destination, ResolutionContext context)
        {
            try
            {
                return source == null || source.Length == 0
                    ? GetDefault(destination, context)
                    : JObject.Parse(Encoding.UTF8.GetString(source));
            }
            catch (JsonReaderException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse byte[] to JObject and failed!"
                );
                return GetDefault(destination, context);
            }
        }

        public string? Convert(JObject? source, string? destination, ResolutionContext context)
            => source?.ToString(Formatting.None) ?? destination;

        public JObject? Convert(string? source, JObject? destination, ResolutionContext context)
        {
            try
            {
                return string.IsNullOrEmpty(source) ? GetDefault(destination, context) : JObject.Parse(source);
            }
            catch (JsonReaderException e)
            {
                context.ConfigurationProvider.Features?.Get<AutoMapperLogger>().LogError(
                    e,
                    "Tried to parse string to JToken and failed!"
                );
                return GetDefault(destination, context);
            }
        }

        public JObject? Convert(JObject? source, JObject? destination, ResolutionContext context) => source ?? GetDefault(destination, context);

        public JArray? Convert(JArray? source, JArray? destination, ResolutionContext context) => source ?? GetDefault(destination, context);

        public JToken? Convert(JToken? source, JToken? destination, ResolutionContext context) => source ?? GetDefaultToken(destination, context);
    }
}