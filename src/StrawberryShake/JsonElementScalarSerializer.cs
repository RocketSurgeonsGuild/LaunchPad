using System.Text.Json;
using Microsoft.Extensions.Options;
using StrawberryShake.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public abstract class JsonElementScalarSerializer<TRuntime> : ScalarSerializer<JsonElement, TRuntime>
{
    private readonly IOptions<JsonSerializerOptions> _options;
    protected JsonElementScalarSerializer(string typeName, IOptions<JsonSerializerOptions> options) : base(typeName) => _options = options;

    public override TRuntime Parse(JsonElement serializedValue)
    {
//            Console.WriteLine("=== START ===");
//        foreach (var item in _options.Value.Converters)
//        {
//            Console.WriteLine(item.GetType());
//        }
//            Console.WriteLine("=== END ===");
        return serializedValue.Deserialize<TRuntime>(_options.Value)!;
    }

    protected override JsonElement Format(TRuntime runtimeValue)
    {
        return JsonSerializer.SerializeToElement(runtimeValue, _options.Value);
    }
}