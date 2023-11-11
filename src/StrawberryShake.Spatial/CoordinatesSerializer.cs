using System.Text.Json;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

/// <summary>
/// A general purpose serializer for Coordinates
/// </summary>
public class CoordinatesSerializer : JsonElementScalarSerializer<Coordinate>
{
    /// <summary>
    /// Create a new CoordinatesSerializer
    /// </summary>
    /// <param name="options"></param>
    public CoordinatesSerializer(IOptions<JsonSerializerOptions> options) : base("Coordinates", options)
    {
    }
}
