using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Rocket.Surgery.LaunchPad.Spatial;

internal partial class WktGeometryConverter : JsonConverter<Geometry>
{
    /// <summary>
    ///     Gets the default geometry factory to use with this converter.
    /// </summary>
    public static GeometryFactory DefaultGeometryFactory { get; } = new GeometryFactory(new PrecisionModel(), 4326);

    private readonly GeometryFactory _geometryFactory;
    private readonly bool _writeGeometryBBox;
    private readonly WKTReader _wktReader;

    /// <summary>
    ///     Creates an instance of this class
    /// </summary>
    /// <param name="geometryFactory">The geometry factory to use.</param>
    /// <param name="writeGeometryBBox">Whether or not to write "bbox" with the geometry.</param>
    public WktGeometryConverter(GeometryFactory geometryFactory, bool writeGeometryBBox)
    {
        _geometryFactory = geometryFactory ?? DefaultGeometryFactory;
        _writeGeometryBBox = writeGeometryBBox;
        _wktReader = new WKTReader(_geometryFactory.GeometryServices);
    }


    public override Geometry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            reader.Read();
            return null;
        }

        return _wktReader.Read(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, Geometry value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.AsText());
    }
}
