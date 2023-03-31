using System.Text.Json;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

public class CoordinatesSerializer : JsonElementScalarSerializer<Coordinate>
{
    public CoordinatesSerializer(IOptions<JsonSerializerOptions> options) : base("Coordinates", options)
    {
    }
}
