using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Serilog.Core;
using Serilog.Events;

namespace Rocket.Surgery.LaunchPad.Spatial;

internal class NetTopologySuiteDestructuringPolicy(string? idPropertyName) : IDestructuringPolicy
{
    public const string defaultIdPropertyName = "_NetTopologySuite_id";

    public static LogEventPropertyValue WriteGeometry(Geometry value)
    {
        return new ScalarValue(value.AsText());
    }

    private static LogEventProperty? WriteBBox(Envelope? value, Geometry? geometry)
    {
        // if we don't want to write "null" bounding boxes, bail out.
        if (value == null || value.IsNull)
            return null;

        // Don't clutter export with bounding box if geometry is a point!
        if (geometry is Point)
            return null;

        return new(
            "bbox",
            value.IsNull
                ? new ScalarValue(null)
                : new SequenceValue(
                    new[]
                    {
                        new ScalarValue(value.MinX),
                        new ScalarValue(value.MinY),
                        new ScalarValue(value.MaxX),
                        new ScalarValue(value.MaxY),
                    }
                )
        );
    }

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

    private static LogEventPropertyValue WriteFeatureCollection(ILogEventPropertyValueFactory propertyValueFactory, FeatureCollection value)
    {
        var props = new List<LogEventProperty> { new("type", new ScalarValue("FeatureCollection")), };
        if (WriteBBox(value.BoundingBox, null) is { } bBox) props.Add(bBox);

        var values = new List<LogEventPropertyValue>();
        foreach (var feature in value)
        {
            values.Add(propertyValueFactory.CreatePropertyValue(feature, true));
        }

        props.Add(new("features", new SequenceValue(values)));

        return new StructureValue(props);
    }

    private readonly string _idPropertyName = idPropertyName ?? defaultIdPropertyName;

    public NetTopologySuiteDestructuringPolicy()
        : this(defaultIdPropertyName) { }

    private LogEventPropertyValue WriteFeature(ILogEventPropertyValueFactory propertyValueFactory, IFeature value)
    {
        var props = new List<LogEventProperty> { new("type", new ScalarValue("Feature")), };
        // Add the id here if present.
        if (value.GetOptionalId(_idPropertyName) is { } id) props.Add(new("id", propertyValueFactory.CreatePropertyValue(id, true)));

        // bbox (optional)
        if (WriteBBox(value.BoundingBox, value.Geometry) is { } bBox) props.Add(bBox);

        // geometry
        if (value.Geometry != null) props.Add(new("geometry", propertyValueFactory.CreatePropertyValue(value.Geometry, true)));

        // properties
        if (value.Attributes != null) props.Add(new("properties", propertyValueFactory.CreatePropertyValue(value.Attributes, true)));

        return new StructureValue(props);
    }

    private LogEventPropertyValue WriteAttributes(ILogEventPropertyValueFactory propertyValueFactory, IAttributesTable value)
    {
        var props = new List<LogEventProperty>();
        foreach (var propertyName in value.GetNames())
        {
            // skip id
            if (propertyName != _idPropertyName) props.Add(new(propertyName, propertyValueFactory.CreatePropertyValue(value[propertyName], true)));
        }

        return new StructureValue(props);
    }

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        if (value is Geometry geometry && GeometryTypes.Contains(value.GetType()))
        {
            result = WriteGeometry(geometry);
            return true;
        }

        if (value is FeatureCollection featureCollection)
        {
            result = WriteFeatureCollection(propertyValueFactory, featureCollection);
            return true;
        }

        if (value is IFeature feature)
        {
            result = WriteFeature(propertyValueFactory, feature);
            return true;
        }

        if (value is IAttributesTable attributesTable)
        {
            result = WriteAttributes(propertyValueFactory, attributesTable);
            return true;
        }

        result = null;
        return false;
    }
}