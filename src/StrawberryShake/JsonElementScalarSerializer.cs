using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Options;
using StrawberryShake.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// Base class for JsonElement Scalar Serializers
/// </summary>
/// <remarks>
///     Values are read and written with the metadata the options' resolver chain has for <typeparamref name="TRuntime" />, so no reflection is
///     needed once a resolver for it is chained (LaunchPad's NodaTime and spatial packages chain theirs).
/// </remarks>
/// <typeparam name="TRuntime"></typeparam>
/// <param name="typeName"></param>
/// <param name="options"></param>
public abstract class JsonElementScalarSerializer<TRuntime>(string typeName, IOptions<JsonSerializerOptions> options) : ScalarSerializer<JsonElement, TRuntime>(typeName)
{
    /// <inheritdoc />
    public override TRuntime Parse(JsonElement serializedValue) =>
        // ReSharper disable once NullableWarningSuppressionIsUsed
        serializedValue.Deserialize(TypeInfo)!;

    /// <inheritdoc />
    protected override JsonElement Format(TRuntime runtimeValue) => JsonSerializer.SerializeToElement(runtimeValue, TypeInfo);

    private JsonTypeInfo<TRuntime> TypeInfo => field ??= TypeInfoFor(_options.Value);

    private readonly IOptions<JsonSerializerOptions> _options = options;

#if NET11_0_OR_GREATER
    private static JsonTypeInfo<TRuntime> TypeInfoFor(JsonSerializerOptions options) => options.GetTypeInfo<TRuntime>();
#else
    private static JsonTypeInfo<TRuntime> TypeInfoFor(JsonSerializerOptions options) => (JsonTypeInfo<TRuntime>)options.GetTypeInfo(typeof(TRuntime));
#endif
}
