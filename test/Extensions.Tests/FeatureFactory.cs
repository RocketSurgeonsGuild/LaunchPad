#pragma warning disable CA5394
#if NET6_0_OR_GREATER
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Mathematics;

namespace Extensions.Tests;

internal class FeatureFactory
{
    public static IFeature Create(OgcGeometryType geometryType, in bool threeD = false, params (string, TypeCode)[] properties)
    {
        var factory = new FeatureFactory();
        var geom = factory.CreateRandomGeometry(geometryType, threeD);
        var att = factory.CreateRandomAttributes(properties);
        return new Feature(geom, att);
    }

    private const int PrecisionScale = 100;
    private static readonly PrecisionModel PM = new PrecisionModel(PrecisionScale);
    private static readonly GeometryFactory GF = new GeometryFactory(PM, 4326);
    private static readonly string[] RNDNames = { "Random", "Zufall", "Hasard", "Caso", "Azar" };
    private static readonly Geometry Bounds = GF.ToGeometry(new Envelope(-BoundX, BoundX, -BoundY, BoundY));

    private const int BoundX = 180 * PrecisionScale;
    private const int BoundY = 90 * PrecisionScale;
    private const int BoundZ = 100 * PrecisionScale;

    private static Coordinate Move(Coordinate centre, double radians, double distance)
    {
        centre = centre.Copy();
        var dx = distance * Math.Cos(radians);
        var dy = distance * Math.Sin(radians);
        centre.X += dx;
        centre.Y += dy;

        return centre;
    }

    private static void Clamp(Coordinate coord)
    {
        if (coord.X < -BoundX) coord.X = -BoundX;
        if (coord.X > BoundX) coord.X = BoundX;
        if (coord.Y < -BoundY) coord.Y = -BoundY;
        if (coord.Y > BoundY) coord.Y = BoundY;
    }

    private static void SetCoordinate(CoordinateSequence cs, int index, Coordinate coord)
    {
        cs.SetX(index, coord.X);
        cs.SetY(index, coord.Y);
        if (coord is CoordinateZ)
            cs.SetZ(index, coord.Z);
    }

    private readonly Random _random;

    public FeatureFactory()
    {
        _random = new Random(17);
    }

    public IAttributesTable CreateRandomAttributes(params (string, TypeCode)[] properties)
    {
        var res = new AttributesTable();
        foreach (var (name, type) in properties)
        {
            var value = type switch
            {
                TypeCode.Boolean => _random.NextDouble() > 0.5d,
                TypeCode.Double  => 500d * _random.NextDouble(),
                TypeCode.Single  => 500f * (float)_random.NextDouble(),
                TypeCode.Empty   => null,
                TypeCode.Int16   => _random.Next(short.MinValue, short.MaxValue),
                TypeCode.Int32   => _random.Next(int.MinValue, int.MaxValue),
                TypeCode.Int64   => 5L * _random.Next(int.MinValue, int.MaxValue),
                TypeCode.String  => RandomString(),
                TypeCode.Object  => Guid.NewGuid(),
                _                => _random.NextDouble() > 0.5d ? RandomString() : null
            };

            res.Add(name, value);
        }

        return res;
    }

    private object RandomString()
    {
        return $"{RNDNames[_random.Next(0, RNDNames.Length)]} - {_random.Next(0, 500)}";
    }

    public Geometry CreateRandomGeometry(in OgcGeometryType geometryType, in bool threeD = false)
    {
        switch (geometryType)
        {
            case OgcGeometryType.Point:
                return CreatePoint(threeD);
            case OgcGeometryType.LineString:
                return CreateLineString(threeD);
            case OgcGeometryType.Polygon:
                return CreatePolygon(threeD);
            case OgcGeometryType.MultiPoint:
                return CreateMultiPoint(threeD);
            case OgcGeometryType.MultiLineString:
                return CreateMultiLineString(threeD);
            case OgcGeometryType.MultiPolygon:
                return CreateMultiPolygon(threeD);
            case OgcGeometryType.GeometryCollection:
                return CreateGeometryCollection(threeD);
        }

        throw new NotSupportedException();
    }

    private Coordinate CreateRandomCoordinate(in bool threeD = false)
    {
        var x = (double)_random.Next(-BoundX, BoundX) / PrecisionScale;
        var y = (double)_random.Next(-BoundY, BoundY) / PrecisionScale;

        var res = threeD
            ? new CoordinateZ(x, y, (double)_random.Next(0, BoundZ) / PrecisionScale)
            : new Coordinate(x, y);
        PM.MakePrecise(res);
        return res;
    }


    public Point CreatePoint(in bool threeD)
    {
        return GF.CreatePoint(CreateRandomCoordinate(threeD));
    }

    public LineString CreateLineString(bool threeD)
    {
        var start = CreateRandomCoordinate(threeD);
        var end = CreateRandomCoordinate(threeD);
        var pointsAt = CreatePositions(_random.Next(6));

        var v = Vector2D.Create(start, end).Normalize();
        v = Vector2D.Create(v.Y, v.X);

        var ls = new LineSegment(start, end);
        var dz = ( end.Z - start.Z ) / ls.Length;

        var cs = GF.CoordinateSequenceFactory.Create(pointsAt.Length + 2, threeD ? 3 : 2, 0);
        var j = 0;
        SetCoordinate(cs, j++, start);
        for (var i = 0; i < pointsAt.Length; i++)
        {
            var pt = ls.PointAlong(pointsAt[i]);
            var tmp = v.Multiply(10d * _random.NextDouble());
            if (threeD)
                pt = new CoordinateZ(pt.X + tmp.X, pt.Y + tmp.Y, start.Z + ( dz * pointsAt[i] ));
            else
                pt = new Coordinate(pt.X + tmp.X, pt.Y + tmp.Y);

            Clamp(pt);
            PM.MakePrecise(pt);
            SetCoordinate(cs, j++, pt);
        }

        SetCoordinate(cs, j, end);

        return GF.CreateLineString(cs);
    }

    private Polygon CreatePolygon(in bool threeD)
    {
        var centre = CreateRandomCoordinate(threeD);
        var radius = PM.MakePrecise(0.5 + ( 3d * _random.NextDouble() ));

        var polygon = (Polygon)GF.CreatePoint(centre).Buffer(radius, 3);
        if (_random.NextDouble() < 0.5d)
            return polygon;

        var numHoles = _random.Next(1, 3);
        radius *= 0.3;
        Geometry? hole = null;
        for (var i = 0; i < numHoles; i++)
        {
            centre = Move(centre, 2 * Math.PI * _random.NextDouble(), _random.NextDouble() * 1.5 * radius);
            var tmp = GF.CreatePoint(centre).Buffer(radius, 3);
            if (hole == null)
                hole = tmp;
            else
                hole = hole.Union(tmp);
        }

        polygon = (Polygon)polygon.Difference(hole);

        return (Polygon)Bounds.Intersection(polygon);
    }

    private MultiPoint CreateMultiPoint(in bool threeD)
    {
        var geoms = new Point[_random.Next(2, 10)];
        for (var i = 0; i < geoms.Length; i++)
        {
            geoms[i] = CreatePoint(threeD);
        }

        return GF.CreateMultiPoint(geoms);
    }

    private MultiLineString CreateMultiLineString(in bool threeD)
    {
        var geoms = new LineString[_random.Next(2, 10)];
        for (var i = 0; i < geoms.Length; i++)
        {
            geoms[i] = CreateLineString(threeD);
        }

        return GF.CreateMultiLineString(geoms);
    }

    private MultiPolygon CreateMultiPolygon(in bool threeD)
    {
        var geoms = new Polygon[_random.Next(2, 10)];
        for (var i = 0; i < geoms.Length; i++)
        {
            geoms[i] = CreatePolygon(threeD);
        }

        return GF.CreateMultiPolygon(geoms);
    }

    private Geometry CreateGeometryCollection(in bool threeD)
    {
        var geoms = new Geometry[_random.Next(2, 5)];
        for (var i = 0; i < geoms.Length; i++)
        {
            switch (_random.Next(2))
            {
                case 0:
                    geoms[i] = CreatePoint(threeD);
                    break;
                case 1:
                    geoms[i] = CreateLineString(threeD);
                    break;
                case 2:
                    geoms[i] = CreatePolygon(threeD);
                    break;
            }
        }

        return GF.CreateGeometryCollection(geoms);
    }

    private double[] CreatePositions(int num)
    {
        var tmp = new List<double>(num);
        for (var i = 0; i < num; i++)
        {
            tmp.Add(_random.NextDouble());
        }

        tmp.Sort();
        return tmp.ToArray();
    }
}
#endif
