using System.Text.Json;
using AutoMapper;

namespace Rocket.Surgery.LaunchPad.Mapping.Profiles;

/// <summary>
///     Configures methods for well know conversions with STJ
/// </summary>
public class SystemJsonTextProfile : Profile
{
    /// <summary>
    ///     The default constructor
    /// </summary>
#pragma warning disable IL2026
    public SystemJsonTextProfile()
    {
        CreateMap<JsonElement, byte[]?>().ConvertUsing(
            source => source.ValueKind == JsonValueKind.Undefined
                ? null
                : JsonSerializer.SerializeToUtf8Bytes(source, (JsonSerializerOptions?)null)
        );
        CreateMap<JsonElement, string?>().ConvertUsing(
            source => source.ValueKind == JsonValueKind.Undefined
                ? null
                : JsonSerializer.Serialize(source, (JsonSerializerOptions?)null)
        );
        CreateMap<JsonElement?, byte[]?>().ConvertUsing(
            source =>
                !source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined
                    ? null
                    : JsonSerializer.SerializeToUtf8Bytes(source, (JsonSerializerOptions?)null)
        );
        CreateMap<JsonElement?, string?>().ConvertUsing(
            source => !source.HasValue || source.Value.ValueKind == JsonValueKind.Undefined
                ? null
                : JsonSerializer.Serialize(source, (JsonSerializerOptions?)null)
        );
        CreateMap<byte[]?, JsonElement>().ConvertUsing(
            source => source == null || source.Length == 0
                ? default
                : JsonSerializer.Deserialize<JsonElement>(source, (JsonSerializerOptions?)null)
        );
        CreateMap<string?, JsonElement>().ConvertUsing(
            source => source == null || source.Length == 0
                ? default
                : JsonSerializer.Deserialize<JsonElement>(source, (JsonSerializerOptions?)null)
        );
        CreateMap<byte[]?, JsonElement?>().ConvertUsing(
            source => source == null || source.Length == 0
                ? null
                : JsonSerializer.Deserialize<JsonElement>(source, (JsonSerializerOptions?)null)
        );
        CreateMap<string?, JsonElement?>().ConvertUsing(
            source => string.IsNullOrEmpty(source)
                ? null
                : JsonSerializer.Deserialize<JsonElement>(source, (JsonSerializerOptions?)null)
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
#pragma warning restore IL2026

    /// <summary>
    ///     Gets the name of the profile.
    /// </summary>
    /// <value>The name of the profile.</value>
    public override string ProfileName => nameof(SystemJsonTextProfile);
}
