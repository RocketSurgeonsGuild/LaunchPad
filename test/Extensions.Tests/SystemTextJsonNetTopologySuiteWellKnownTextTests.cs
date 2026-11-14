using System.Text.Json;
using NetTopologySuite.Geometries;
using Rocket.Surgery.LaunchPad.Spatial;

namespace Extensions.Tests;

public class SystemTextJsonNetTopologySuiteWellKnownTextTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    private static T DeserializeObject<T>(string geom, JsonSerializerOptions settings) => JsonSerializer.Deserialize<T>("\"" + geom + "\"", settings)!;

    private static string SerializeObject(object geom, JsonSerializerOptions settings) => JsonSerializer.Serialize(geom, settings).Trim('"');

    private readonly JsonSerializerOptions _settings = new JsonSerializerOptions().ConfigureWellKnownTextForLaunchPad(null);

    // typeof(Geometry),
    // typeof(MultiPoint),
    // typeof(MultiLineString),
    // typeof(MultiPolygon),
    // typeof(GeometryCollection)
    [Test]
    [Arguments("POINT (30 10)")]
    public async Task Geometry_Tests(string geom)
    {
        var value = DeserializeObject<Point>(geom, _settings);
        await value.Should().BeEqualTo(new Point(30, 10));
        await SerializeObject(value, _settings).Should().BeEqualTo(geom);
    }

    [Test]
    [Arguments("LINESTRING (30 10, 10 30, 40 40)")]
    public async Task LineString_Tests(string geom)
    {
        var value = DeserializeObject<LineString>(geom, _settings);
        await value.Should().BeEqualTo(new LineString(
                 [
                                new Coordinate(30, 10),
                                new Coordinate(10, 30),
                                new Coordinate(40, 40),
                 ]
             ));
        await SerializeObject(value, _settings).Should().BeEqualTo(geom);
    }

    [Test]
    [Arguments("POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))")]
    public async Task Polygon_Tests(string geom)
    {
        var value = DeserializeObject<Polygon>(geom, _settings);
        await value.Should().BeEqualTo(new Polygon(
                 new LinearRing(
                     [
                                    new Coordinate(30, 10), new Coordinate(40, 40), new Coordinate(20, 40), new Coordinate(10, 20), new Coordinate(30, 10)
                     ]
                 )
             ));
        await SerializeObject(value, _settings).Should().BeEqualTo(geom);
    }

    [Test]
    [Arguments("MULTIPOINT ((10 40), (40 30), (20 20), (30 10))")]
    public async Task MultiPoint_Tests(string geom)
    {
        var value = DeserializeObject<MultiPoint>(geom, _settings);
        await value.Should().BeEqualTo(new MultiPoint(
                 [
                                new Point(10, 40),
                                new Point(40, 30),
                                new Point(20, 20),
                                new Point(30, 10)
                 ]
             ));
        await SerializeObject(value, _settings).Should().BeEqualTo(geom);
    }

    [Test]
    [Arguments("MULTILINESTRING ((10 10, 20 20, 10 40), (40 40, 30 30, 40 20, 30 10))")]
    public async Task MultiLineString_Tests(string geom)
    {
        var value = await Assert.That(DeserializeObject<MultiLineString>(geom, _settings)).IsTypeOf<MultiLineString>();
        await SerializeObject(value, _settings).Should().BeEqualTo(geom);
    }

    [Test]
    [Arguments("MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)), ((20 35, 10 30, 10 10, 30 5, 45 20, 20 35)), ((30 20, 20 15, 20 25, 30 20)))")]
    public async Task MultiPolygon_Tests(string geom)
    {
        var value = await Assert.That(DeserializeObject<MultiPolygon>(geom, _settings)).IsTypeOf<MultiPolygon>();
        await SerializeObject(value, _settings).Should().BeEqualTo(geom);
    }

    [Test]
    [Arguments("GEOMETRYCOLLECTION (POINT (40 10), LINESTRING (10 10, 20 20, 10 40), POLYGON ((40 40, 20 45, 45 30, 40 40)))")]
    public async Task GeometryCollection_Tests(string geom)
    {
        var value = await Assert.That(DeserializeObject<GeometryCollection>(geom, _settings)).IsTypeOf<GeometryCollection>();
        await SerializeObject(value, _settings).Should().BeEqualTo(geom);
    }

    public class ValueOf<T>
    {
        public T Value { get; set; } = default!;
    }
}
