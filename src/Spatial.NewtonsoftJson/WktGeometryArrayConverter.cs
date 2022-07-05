using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;

namespace Rocket.Surgery.LaunchPad.Spatial;

public class WktGeometryArrayConverter : GeometryArrayConverter
{
    private readonly WKTReader _wktReader;


    /// <summary>
    ///     Creates an instance of this class using <see cref="GeoJsonSerializer.Wgs84Factory" /> to create geometries.
    /// </summary>
    public WktGeometryArrayConverter() : this(WktGeometryConverter.Wgs84Factory)
    {
    }

    /// <summary>
    ///     Creates an instance of this class using the provided <see cref="GeometryFactory" /> to create geometries.
    /// </summary>
    /// <param name="geometryFactory">The geometry factory.</param>
    public WktGeometryArrayConverter(GeometryFactory geometryFactory) : this(geometryFactory, 2)
    {
    }

    /// <summary>
    ///     Creates an instance of this class using the provided <see cref="GeometryFactory" /> to create geometries.
    /// </summary>
    /// <param name="geometryFactory">The geometry factory.</param>
    /// <param name="dimension">The number of dimensions to handle.  Must be 2 or 3.</param>
    public WktGeometryArrayConverter(GeometryFactory geometryFactory, int dimension) : base(geometryFactory, dimension)
    {
        _wktReader = new WKTReader(geometryFactory.GeometryServices);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        reader.Read();
        if (reader.TokenType != JsonToken.StartArray)
        {
            throw new Exception();
        }

        reader.Read();
        var geoms = new List<Geometry>();
        while (reader.TokenType != JsonToken.EndArray)
        {
            if (reader.TokenType == JsonToken.String)
            {
                geoms.Add(_wktReader.Read(reader.ReadAsString()));
                continue;
            }

            reader.Read();
        }

        return geoms;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not IEnumerable<Geometry> geometries)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteStartArray();
            foreach (var geometry in geometries)
            {
                serializer.Serialize(writer, geometry);
            }

            writer.WriteEndArray();
        }
    }
}
