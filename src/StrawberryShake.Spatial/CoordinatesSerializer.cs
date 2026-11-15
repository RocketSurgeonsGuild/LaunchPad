using System.Text.Json;

using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

/// <summary>
///     Serializes HotChocolate's <c>Coordinates</c> scalar, a list of positions of arbitrary depth (the coordinates of a
///     <c>GeoJSONMultiPolygonType</c>, for example)
/// </summary>
/// <remarks>
///     The depth depends on the geometry, so there is no one runtime type for it: the value is handed over as the JSON it was sent as.
/// </remarks>
[PublicAPI]
public class CoordinatesSerializer() : ScalarSerializer<JsonElement, JsonElement>("Coordinates")
{
    /// <inheritdoc />
    public override JsonElement Parse(JsonElement serializedValue) => serializedValue.Clone();

    /// <inheritdoc />
    protected override JsonElement Format(JsonElement runtimeValue) => runtimeValue;
}
