using System.Text.Json;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

public class GeometrySerializer : JsonElementScalarSerializer<Geometry>
{
    public GeometrySerializer(IOptions<JsonSerializerOptions> options) : base("Geometry", options)
    {
    }
}