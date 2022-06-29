using NetTopologySuite;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Serilog.Core;
using Serilog.Events;

namespace Rocket.Surgery.LaunchPad.Spatial;

internal class NetTopologySuiteDestructuringPolicy : IDestructuringPolicy
{
    public const string defaultIdPropertyName = "_NetTopologySuite_id";


    internal static LogEventProperty WriteBBox(Envelope value, Geometry? geometry)
    {
        // if we don't want to write "null" bounding boxes, bail out.
        if (value == null || value.IsNull)
            return null;

        // Don't clutter export with bounding box if geometry is a point!
        if (geometry is Point)
            return null;

        // if value == null, try to get it from geometry
        if (value == null)
            value = geometry?.EnvelopeInternal ?? new Envelope();

        return new LogEventProperty(
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

    private static readonly HashSet<Type> GeometryTypes = new HashSet<Type>
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
        var props = new List<LogEventProperty> { new("type", new ScalarValue("FeatureCollection")) };
        if (WriteBBox(value.BoundingBox, null) is { } bBox)
        {
            props.Add(bBox);
        }

        var values = new List<LogEventPropertyValue>();
        foreach (var feature in value)
            values.Add(propertyValueFactory.CreatePropertyValue(feature, true));
        props.Add(new LogEventProperty("features", new SequenceValue(values)));

        return new StructureValue(props);
    }

    private static SequenceValue WriteGeometrySequence(Geometry value)
    {
        var values = new LogEventPropertyValue?[value.NumGeometries];
        for (var i = 0; i < value.NumGeometries; i++)
        {
            values[i] = value switch
            {
                MultiPoint      => WriteCoordinateSequence(( (Point)value.GetGeometryN(i) ).CoordinateSequence, false),
                MultiLineString => WriteCoordinateSequence(( (LineString)value.GetGeometryN(i) ).CoordinateSequence, true),
                MultiPolygon    => WritePolygon((Polygon)value.GetGeometryN(i)),
                _               => default
            };
        }

        return new SequenceValue(values);
    }

    private static LogEventPropertyValue WritePolygon(Polygon value)
    {
        var values = new List<LogEventPropertyValue>(value.NumInteriorRings + 1);
        values.Add(WriteCoordinateSequence(value.ExteriorRing.CoordinateSequence, true, OrientationIndex.Clockwise));
        for (var i = 0; i < value.NumInteriorRings; i++)
            values.Add(
                WriteCoordinateSequence(
                    value.GetInteriorRingN(i).CoordinateSequence, true, OrientationIndex.CounterClockwise
                )
            );
        return new SequenceValue(values);
    }

    private static LogEventPropertyValue WriteCoordinateSequence(
        CoordinateSequence? sequence,
        bool multiple,
        OrientationIndex orientation = OrientationIndex.None
    )
    {
        if (sequence == null)
        {
            return new ScalarValue(null);
        }

        var values = new List<LogEventPropertyValue>();

        if (multiple)
        {
            if (( orientation == OrientationIndex.Clockwise && Orientation.IsCCW(sequence) ) ||
                ( orientation == OrientationIndex.CounterClockwise && !Orientation.IsCCW(sequence) ))
            {
                CoordinateSequences.Reverse(sequence);
            }
        }

        var hasZ = sequence.HasZ;
        for (var i = 0; i < sequence.Count; i++)
        {
            var value = new List<LogEventPropertyValue>(3);
            value.Add(new ScalarValue(sequence.GetX(i)));
            value.Add(new ScalarValue(sequence.GetY(i)));

            if (hasZ)
            {
                var z = sequence.GetZ(i);
                if (!double.IsNaN(z))
                    value.Add(new ScalarValue(z));
            }

            if (multiple)
            {
                values.Add(new SequenceValue(value));
            }
            else
            {
                values = value;
                break;
            }
        }

        return new SequenceValue(values);
    }

    private readonly string _idPropertyName;
    private readonly bool _writeGeometryBBox;


    public NetTopologySuiteDestructuringPolicy()
        : this(false)
    {
    }

    public NetTopologySuiteDestructuringPolicy(bool writeGeometryBBox)
        : this(writeGeometryBBox, defaultIdPropertyName)
    {
    }

    public NetTopologySuiteDestructuringPolicy(bool writeGeometryBBox, string? idPropertyName)
    {
        _writeGeometryBBox = writeGeometryBBox;
        _idPropertyName = idPropertyName ?? defaultIdPropertyName;
    }

    public LogEventPropertyValue WriteGeometry(ILogEventPropertyValueFactory propertyValueFactory, Geometry value)
    {
        var props = new List<LogEventProperty> { new("type", propertyValueFactory.CreatePropertyValue(value.OgcGeometryType)) };
        if (value.OgcGeometryType == OgcGeometryType.GeometryCollection)
        {
            var values = new LogEventPropertyValue[value.NumGeometries];
            for (var i = 0; i < value.NumGeometries; i++)
                values[i] = propertyValueFactory.CreatePropertyValue(value.GetGeometryN(i), true);
            props.Add(new LogEventProperty("geometries", new SequenceValue(values)));
        }
        else if (value.IsEmpty)
        {
            props.Add(new LogEventProperty("coordinates", new SequenceValue(Array.Empty<LogEventPropertyValue>())));
        }
        else
        {
            var coordinates = value.OgcGeometryType switch
            {
                OgcGeometryType.Point           => WriteCoordinateSequence(( (Point)value ).CoordinateSequence, false),
                OgcGeometryType.LineString      => WriteCoordinateSequence(( (LineString)value ).CoordinateSequence, true),
                OgcGeometryType.Polygon         => WritePolygon((Polygon)value),
                OgcGeometryType.MultiPoint      => WriteGeometrySequence(value),
                OgcGeometryType.MultiLineString => WriteGeometrySequence(value),
                OgcGeometryType.MultiPolygon    => WriteGeometrySequence(value),
                _                               => null
            };
            props.Add(new LogEventProperty("coordinates", coordinates));
        }

        if (_writeGeometryBBox && WriteBBox(value.EnvelopeInternal, value) is { } bBox)
        {
            props.Add(bBox);
        }

        return new StructureValue(props);
    }

    private LogEventPropertyValue WriteFeature(ILogEventPropertyValueFactory propertyValueFactory, IFeature value)
    {
        var props = new List<LogEventProperty> { new("type", new ScalarValue("Feature")) };
        // Add the id here if present.
        if (value.GetOptionalId(_idPropertyName) is { } id)
        {
            props.Add(new LogEventProperty("id", propertyValueFactory.CreatePropertyValue(id, true)));
        }

        // bbox (optional)
        if (WriteBBox(value.BoundingBox, value.Geometry) is { } bBox)
        {
            props.Add(bBox);
        }

        // geometry
        if (value.Geometry != null)
        {
            props.Add(new LogEventProperty("geometry", propertyValueFactory.CreatePropertyValue(value.Geometry, true)));
        }

        // properties
        if (value.Attributes != null)
        {
            props.Add(new LogEventProperty("properties", propertyValueFactory.CreatePropertyValue(value.Attributes, true)));
        }

        return new StructureValue(props);
    }

    private LogEventPropertyValue WriteAttributes(ILogEventPropertyValueFactory propertyValueFactory, IAttributesTable value)
    {
        var props = new List<LogEventProperty>();
        foreach (var propertyName in value.GetNames())
        {
            // skip id
            if (propertyName != _idPropertyName)
            {
                props.Add(new LogEventProperty(propertyName, propertyValueFactory.CreatePropertyValue(value[propertyName], true)));
            }
        }

        return new StructureValue(props);
    }

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue? result)
    {
        if (value is Geometry geometry && GeometryTypes.Contains(value.GetType()))
        {
            result = WriteGeometry(propertyValueFactory, geometry);
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
