using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Rocket.Surgery.LaunchPad.Spatial;

internal partial class WktGeometryConverter : JsonConverter<Geometry?>
{
    /// <summary>
    ///     Gets the default geometry factory to use with this converter.
    /// </summary>
    public static GeometryFactory DefaultGeometryFactory { get; } = new(new PrecisionModel(), 4326);

    private readonly WKTReader _wktReader;

    /// <summary>
    ///     Creates an instance of this class
    /// </summary>
    /// <param name="geometryFactory">The geometry factory to use.</param>
    public WktGeometryConverter(GeometryFactory? geometryFactory)
    {
        var geometryFactory1 = geometryFactory ?? DefaultGeometryFactory;
        _wktReader = new WKTReader(geometryFactory1.GeometryServices);
    }


    public override Geometry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            reader.Read();
            return null;
        }

        return _wktReader.Read(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, Geometry? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.AsText());
    }
}



