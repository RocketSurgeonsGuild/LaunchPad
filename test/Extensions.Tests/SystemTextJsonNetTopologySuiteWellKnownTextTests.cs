using System.Text.Json;
using NetTopologySuite.Geometries;
using Rocket.Surgery.LaunchPad.Spatial;

namespace Extensions.Tests;

public class SystemTextJsonNetTopologySuiteWellKnownTextTests(ITestContextAccessor outputHelper) : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(outputHelper))
{
    private static T DeserializeObject<T>(string geom, JsonSerializerOptions settings)
    {
        return JsonSerializer.Deserialize<T>("\"" + geom + "\"", settings)!;
    }

    private static string SerializeObject(object geom, JsonSerializerOptions settings)
    {
        return JsonSerializer.Serialize(geom, settings).Trim('"');
    }

    private readonly JsonSerializerOptions _settings = new JsonSerializerOptions().ConfigureWellKnownTextForLaunchPad(null);

    // typeof(Geometry),
    // typeof(MultiPoint),
    // typeof(MultiLineString),
    // typeof(MultiPolygon),
    // typeof(GeometryCollection)
    [Theory]
    [InlineData("POINT (30 10)")]
    public void Geometry_Tests(string geom)
    {
        var value = DeserializeObject<Point>(geom, _settings);
                   value.ShouldBe(new Point(30, 10));
        SerializeObject(value, _settings).ShouldBe(geom);
    }

    [Theory]
    [InlineData("LINESTRING (30 10, 10 30, 40 40)")]
    public void LineString_Tests(string geom)
    {
        var value = DeserializeObject<LineString>(geom, _settings);
                   value.ShouldBe(
                        new LineString(
                            new[]
                            {
                                new Coordinate(30, 10),
                                new Coordinate(10, 30),
                                new Coordinate(40, 40),
                            }
                        )
                    );
        SerializeObject(value, _settings).ShouldBe(geom);
    }

    [Theory]
    [InlineData("POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))")]
    public void Polygon_Tests(string geom)
    {
        var value = DeserializeObject<Polygon>(geom, _settings);
                   value.ShouldBe(
                        new Polygon(
                            new LinearRing(
                                new[]
                                {
                                    new Coordinate(30, 10), new Coordinate(40, 40), new Coordinate(20, 40), new Coordinate(10, 20), new Coordinate(30, 10)
                                }
                            )
                        )
                    );
        SerializeObject(value, _settings).ShouldBe(geom);
    }

    [Theory]
    [InlineData("MULTIPOINT ((10 40), (40 30), (20 20), (30 10))")]
    public void MultiPoint_Tests(string geom)
    {
        var value = DeserializeObject<MultiPoint>(geom, _settings);
                   value.ShouldBe(
                        new MultiPoint(
                            new[]
                            {
                                new Point(10, 40),
                                new Point(40, 30),
                                new Point(20, 20),
                                new Point(30, 10)
                            }
                        )
                    );
        SerializeObject(value, _settings).ShouldBe(geom);
    }

    [Theory]
    [InlineData("MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))")]
    public void MultiLineString_Tests(string geom)
    {
        var value = DeserializeObject<MultiLineString>(geom, _settings)
                   .ShouldBeOfType<MultiLineString>();
        SerializeObject(value, _settings).ShouldBe(geom);
    }

    [Theory]
    [InlineData("MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)), ((20 35, 10 30, 10 10, 30 5, 45 20, 20 35)), ((30 20, 20 15, 20 25, 30 20)))")]
    public void MultiPolygon_Tests(string geom)
    {
        var value = DeserializeObject<MultiPolygon>(geom, _settings)
                   .ShouldBeOfType<MultiPolygon>();
        SerializeObject(value, _settings).ShouldBe(geom);
    }

    [Theory]
    [InlineData("GEOMETRYCOLLECTION (POINT (40 10), LINESTRING (10 10, 20 20, 10 40), POLYGON ((40 40, 20 45, 45 30, 40 40)))")]
    public void GeometryCollection_Tests(string geom)
    {
        var value = DeserializeObject<GeometryCollection>(geom, _settings)
                   .ShouldBeOfType<GeometryCollection>();
        SerializeObject(value, _settings).ShouldBe(geom);
    }

    public class ValueOf<T>
    {
        public T Value { get; set; } = default!;
    }
}
