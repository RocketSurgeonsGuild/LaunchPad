using System.Text.Json;

using NetTopologySuite.Geometries;

using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

/// <summary>
///     Serializes HotChocolate's <c>Position</c> scalar, a GeoJSON position (<c>[x, y]</c> or <c>[x, y, z]</c>), as a <see cref="Coordinate" />
/// </summary>
/// <remarks>
///     The coordinates of the GeoJSON object types (<c>GeoJSONPointType</c> and the rest) are positions, so a client that selects them needs this
///     serializer: without it StrawberryShake reads the unknown scalar as a string and fails on the array.
/// </remarks>
[PublicAPI]
public class PositionSerializer() : ScalarSerializer<JsonElement, Coordinate>("Position")
{
    /// <inheritdoc />
    public override Coordinate Parse(JsonElement serializedValue)
    {
        if (serializedValue.ValueKind != JsonValueKind.Array || serializedValue.GetArrayLength() is < 2 or > 3)
        {
            throw new JsonException($"A Position is an array of two or three numbers, but the value was {serializedValue.GetRawText()}");
        }

        var x = serializedValue[0].GetDouble();
        var y = serializedValue[1].GetDouble();
        return serializedValue.GetArrayLength() == 3 ? new CoordinateZ(x, y, serializedValue[2].GetDouble()) : new Coordinate(x, y);
    }

    /// <inheritdoc />
    protected override JsonElement Format(Coordinate runtimeValue)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            WritePosition(writer, runtimeValue);
        }

        using var document = JsonDocument.Parse(stream.ToArray());
        return document.RootElement.Clone();
    }

    internal static void WritePosition(Utf8JsonWriter writer, Coordinate coordinate)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(coordinate.X);
        writer.WriteNumberValue(coordinate.Y);
        if (!double.IsNaN(coordinate.Z))
        {
            writer.WriteNumberValue(coordinate.Z);
        }

        writer.WriteEndArray();
    }
}
