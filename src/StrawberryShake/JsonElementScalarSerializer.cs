using System.Text.Json;
using Microsoft.Extensions.Options;
using StrawberryShake.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// Base class for JsonElement Scalar Serializers
/// </summary>
/// <typeparam name="TRuntime"></typeparam>
public abstract class JsonElementScalarSerializer<TRuntime> : ScalarSerializer<JsonElement, TRuntime>
{
    private readonly IOptions<JsonSerializerOptions> _options;

    /// <summary>
    /// Constructor for JsonElement Scalar Serializers
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="options"></param>
    protected JsonElementScalarSerializer(string typeName, IOptions<JsonSerializerOptions> options) : base(typeName) => _options = options;

    /// <inheritdoc />
    public override TRuntime Parse(JsonElement serializedValue)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        return serializedValue.Deserialize<TRuntime>(_options.Value)!;
    }

    /// <inheritdoc />
    protected override JsonElement Format(TRuntime runtimeValue)
    {
        return JsonSerializer.SerializeToElement(runtimeValue, _options.Value);
    }
}
