using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     Resolves the metadata of value types written by a converter in <see cref="JsonSerializerOptions.Converters" />, such as the NodaTime or the
///     spatial types, without reflection.
/// </summary>
/// <remarks>
///     A source generated <see cref="JsonSerializerContext" /> only knows the types it declares. When such a type is serialized as an
///     <see cref="object" /> (a problem details extension, a health check value) or at the root, the options still need metadata for it; this resolver
///     builds it from the converter the options already have. A type with no converter in the options is left to the next resolver in the chain.
/// </remarks>
[PublicAPI]
public sealed class JsonConverterTypeInfoResolver : IJsonTypeInfoResolver
{
    private readonly Dictionary<Type, Func<JsonSerializerOptions, JsonTypeInfo?>> _factories = [];

    /// <summary>
    ///     Resolves <typeparamref name="T" /> with the converter for it in the options.
    /// </summary>
    /// <typeparam name="T">The type a converter writes.</typeparam>
    /// <returns>This resolver.</returns>
    public JsonConverterTypeInfoResolver Add<T>()
    {
        _factories[typeof(T)] = Create<T>;
        return this;
    }

    /// <summary>
    ///     Resolves <typeparamref name="T" /> and <see cref="Nullable{T}" /> with the converter for <typeparamref name="T" /> in the options.
    /// </summary>
    /// <typeparam name="T">The value type a converter writes.</typeparam>
    /// <returns>This resolver.</returns>
    public JsonConverterTypeInfoResolver AddNullable<T>() where T : struct
    {
        _factories[typeof(T)] = Create<T>;
        _factories[typeof(T?)] = CreateNullable<T>;
        return this;
    }

    /// <summary>
    ///     The types this resolver knows.
    /// </summary>
    public IEnumerable<Type> Types => _factories.Keys;

    /// <inheritdoc />
    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options) => _factories.TryGetValue(type, out var factory) ? factory(options) : null;

    private static JsonTypeInfo? Create<T>(JsonSerializerOptions options)
    {
        foreach (var converter in options.Converters)
        {
            if (!converter.CanConvert(typeof(T))) continue;
            var typed = converter is JsonConverterFactory factory ? factory.CreateConverter(typeof(T), options) : converter;
            if (typed is not JsonConverter<T>) continue;
            return JsonMetadataServices.CreateValueInfo<T>(options, typed);
        }

        return null;
    }

    private static JsonTypeInfo? CreateNullable<T>(JsonSerializerOptions options) where T : struct =>
        Create<T>(options) is JsonTypeInfo<T> underlying
            ? JsonMetadataServices.CreateValueInfo<T?>(options, JsonMetadataServices.GetNullableConverter(underlying))
            : null;
}
