using System.Text.Json;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

/// <summary>
/// General purpose Geometry Serializer
/// </summary>
public class GeometrySerializer : JsonElementScalarSerializer<Geometry>
{
    /// <summary>
    /// Construct a new GeometrySerializer
    /// </summary>
    /// <param name="options"></param>
    public GeometrySerializer(IOptions<JsonSerializerOptions> options) : base("Geometry", options)
    {
    }
}
