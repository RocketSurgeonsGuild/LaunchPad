using AutoMapper;
using System.Text.Json;

namespace Rocket.Surgery.LaunchPad.Mapping.Profiles
{
    /// <summary>
    /// Configures methods for well know conversions with STJ
    /// </summary>
    public class SystemJsonTextProfile : Profile
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public SystemJsonTextProfile()
        {
            CreateMap<JsonElement, byte[]?>().ConvertUsing(
                source => source.ValueKind == JsonValueKind.Undefined
                    ? null
                    : JsonSerializer.SerializeToUtf8Bytes(source, null)
            );
            CreateMap<JsonElement, string?>().ConvertUsing(
                source => source.ValueKind == JsonValueKind.Undefined
                    ? null
                    : JsonSerializer.Serialize(source, null)
            );
            CreateMap<JsonElement?, byte[]?>().ConvertUsing(
                source =>
                    !source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined
                        ? null
                        : JsonSerializer.SerializeToUtf8Bytes(source, null)
            );
            CreateMap<JsonElement?, string?>().ConvertUsing(
                source => !source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined
                    ? null
                    : JsonSerializer.Serialize(source, null)
            );
            CreateMap<byte[]?, JsonElement>().ConvertUsing(
                source => source == null || source.Length == 0
                    ? default
                    : JsonSerializer.Deserialize<JsonElement>(source, null)
            );
            CreateMap<string?, JsonElement>().ConvertUsing(
                source => source == null || source.Length == 0
                    ? default
                    : JsonSerializer.Deserialize<JsonElement>(source, null)
            );
            CreateMap<byte[]?, JsonElement?>().ConvertUsing(
                source => source == null || source.Length == 0
                    ? null
                    : JsonSerializer.Deserialize<JsonElement>(source, null)
            );
            CreateMap<string?, JsonElement?>().ConvertUsing(
                source => string.IsNullOrEmpty(source)
                    ? null
                    : JsonSerializer.Deserialize<JsonElement>(source, null)
            );
            CreateMap<JsonElement, JsonElement?>().ConvertUsing(
                source => source.ValueKind == JsonValueKind.Undefined
                    ? null
                    : source
            );
            CreateMap<JsonElement?, JsonElement>().ConvertUsing(
                source => !source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined
                    ? default
                    : source.Value
            );
        }

        /// <summary>
        /// Gets the name of the profile.
        /// </summary>
        /// <value>The name of the profile.</value>
        public override string ProfileName => nameof(SystemJsonTextProfile);
    }
}