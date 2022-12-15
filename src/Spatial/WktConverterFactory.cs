using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;

namespace Rocket.Surgery.LaunchPad.Spatial;

/// <summary>
///     A converter for
/// </summary>
public class WktConverterFactory : GeoJsonConverterFactory
{
    private static readonly HashSet<Type> GeometryTypes = new()
    {
        typeof(Geometry),
        typeof(Point),
        typeof(LineString),
        typeof(Polygon),
        typeof(MultiPoint),
        typeof(MultiLineString),
        typeof(MultiPolygon),
        typeof(GeometryCollection),
    };

    private readonly GeometryFactory _factory;
    private readonly bool _writeGeometryBBox;

    /// <summary>
    ///     Creates an instance of this class using the defaults.
    /// </summary>
    public WktConverterFactory()
        : this(NtsGeometryServices.Instance.CreateGeometryFactory(4326), false)
    {
    }

    /// <summary>
    ///     Creates an instance of this class using the provided <see cref="GeometryFactory" /> and
    ///     defaults for other values.
    /// </summary>
    /// <param name="factory"></param>
    public WktConverterFactory(GeometryFactory factory)
        : this(factory, false)
    {
    }

    /// <summary>
    ///     Creates an instance of this class using the provided <see cref="GeometryFactory" />, the
    ///     given value for whether or not we should write out a "bbox" for a plain geometry, and
    ///     defaults for all other values.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="writeGeometryBBox"></param>
    public WktConverterFactory(GeometryFactory factory, bool writeGeometryBBox)
        : this(factory, writeGeometryBBox, DefaultIdPropertyName)
    {
    }

    /// <summary>
    ///     Creates an instance of this class using the provided <see cref="GeometryFactory" />, the
    ///     given value for whether or not we should write out a "bbox" for a plain geometry, and
    ///     the given "magic" string to signal when an <see cref="IAttributesTable" /> property is
    ///     actually filling in for a Feature's "id".
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="writeGeometryBBox"></param>
    /// <param name="idPropertyName"></param>
    public WktConverterFactory(GeometryFactory factory, bool writeGeometryBBox, string idPropertyName)
        : base(factory, writeGeometryBBox, idPropertyName)
    {
        _factory = factory;
        _writeGeometryBBox = writeGeometryBBox;
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return GeometryTypes.Contains(typeToConvert)
            ? new WktGeometryConverter(_factory, _writeGeometryBBox)
            : base.CreateConverter(typeToConvert, options);
    }
}



