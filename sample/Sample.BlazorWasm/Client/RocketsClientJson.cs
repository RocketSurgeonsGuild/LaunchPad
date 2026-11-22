using System.Text.Json;
using System.Text.Json.Serialization;

using Refit;

namespace Sample.BlazorWasm.Client;

/// <summary>
///     The JSON of <see cref="IRocketsClient" />: Refit reads and writes the models Refitter generates through this context, as LaunchPad never
///     serializes JSON through reflection.
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(RocketModel))]
[JsonSerializable(typeof(RocketType))]
[JsonSerializable(typeof(CreateRocketRequest))]
[JsonSerializable(typeof(CreateRocketResponse))]
[JsonSerializable(typeof(EditRocketRequest))]
[JsonSerializable(typeof(ProblemDetails))]
internal sealed partial class RocketsClientJson : JsonSerializerContext
{
    /// <summary>The settings the generated registration takes from the container.</summary>
    /// <remarks>Created when first read: <see cref="Default" /> is initialized in the generated part, after a field initializer of this one may run.</remarks>
    public static RefitSettings Settings => field ??= new(new SystemTextJsonContentSerializer(Default.Options));
}
