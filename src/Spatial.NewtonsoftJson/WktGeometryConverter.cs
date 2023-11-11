using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;

namespace Rocket.Surgery.LaunchPad.Spatial;

/// <summary>
///     WKT text converter
/// </summary>
public class WktGeometryConverter : GeometryConverter
{
    /// <summary>
    ///     Gets a default GeometryFactory
    /// </summary>
    internal static GeometryFactory Wgs84Factory { get; } = new (new PrecisionModel(), 4326);

    private readonly WKTReader _wktReader;

    /// <summary>
    ///     Creates an instance of this class using <see cref="WktGeometryConverter.Wgs84Factory" /> to create geometries.
    /// </summary>
    public WktGeometryConverter() : this(Wgs84Factory)
    {
    }

    /// <summary>
    ///     Creates an instance of this class using the provided <see cref="GeometryFactory" /> to create geometries.
    /// </summary>
    /// <param name="geometryFactory">The geometry factory.</param>
    public WktGeometryConverter(GeometryFactory geometryFactory) : this(geometryFactory, 2)
    {
    }

    /// <summary>
    ///     Creates an instance of this class using the provided <see cref="GeometryFactory" /> to create geometries.
    /// </summary>
    /// <param name="geometryFactory">The geometry factory.</param>
    /// <param name="dimension">The number of dimensions to handle.  Must be 2 or 3.</param>
    public WktGeometryConverter(GeometryFactory geometryFactory, int dimension) : base(geometryFactory, dimension)
    {
        _wktReader = new WKTReader(geometryFactory.GeometryServices);
    }

    /// <inheritdoc />
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        return _wktReader.Read(reader.Value?.ToString());
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not Geometry geom)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteValue(geom.AsText());
    }
}



