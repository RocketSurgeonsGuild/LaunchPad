using System.Text.Json;
using NetTopologySuite.Geometries;
using Rocket.Surgery.LaunchPad.Spatial;

namespace Extensions.Tests;

public class SystemTextJsonNetTopologySuiteGeoJsonTests(ITestContextAccessor outputHelper) : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(outputHelper))
{
    private readonly JsonSerializerOptions _settings = new JsonSerializerOptions().ConfigureGeoJsonForLaunchPad(null);

    // typeof(Geometry),
    // typeof(MultiPoint),
    // typeof(MultiLineString),
    // typeof(MultiPolygon),
    // typeof(GeometryCollection)
    [Theory]
    [InlineData("{\"type\": \"Point\",\"coordinates\": [30, 10]}")]
    public void Geometry_Tests(string geom)
    {
        JsonSerializer.Deserialize<Point>(geom, _settings)
                      .ShouldBe(new Point(30, 10));
    }

    [Theory]
    [InlineData("{\"type\": \"LineString\",\"coordinates\": [[30, 10], [10, 30], [40, 40]]}")]
    public void LineString_Tests(string geom)
    {
        JsonSerializer.Deserialize<LineString>(geom, _settings)
                      .ShouldBe(
                           new LineString(
                               new[]
                               {
                                   new Coordinate(30, 10),
                                   new Coordinate(10, 30),
                                   new Coordinate(40, 40),
                               }
                           )
                       );
    }

    [Theory]
    [InlineData("{\"type\": \"Polygon\",\"coordinates\": [[[30, 10], [40, 40], [20, 40], [10, 20], [30, 10]]]}")]
    public void Polygon_Tests(string geom)
    {
        JsonSerializer.Deserialize<Polygon>(geom, _settings)
                      .ShouldBe(
                           new Polygon(
                               new LinearRing(
                                   new[]
                                   {
                                       new Coordinate(30, 10), new Coordinate(40, 40), new Coordinate(20, 40), new Coordinate(10, 20), new Coordinate(30, 10)
                                   }
                               )
                           )
                       );
    }

    [Theory]
    [InlineData("{\"type\": \"MultiPoint\",\"coordinates\": [[10, 40], [40, 30], [20, 20], [30, 10]]}")]
    public void MultiPoint_Tests(string geom)
    {
                JsonSerializer.Deserialize<MultiPoint>(geom, _settings)

           .ShouldBe(
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
    }

    [Theory]
    [InlineData(
        "{\n    \"type\": \"MultiLineString\", \n    \"coordinates\": [\n        [[10, 10], [20, 20], [10, 40]], \n        [[40, 40], [30, 30], [40, 20], [30, 10]]\n    ]\n}"
    )]
    public void MultiLineString_Tests(string geom)
    {
        JsonSerializer.Deserialize<MultiLineString>(geom, _settings)
                      .ShouldBeOfType<MultiLineString>();
    }

    [Theory]
    [InlineData(
        "{\n    \"type\": \"MultiPolygon\", \n    \"coordinates\": [\n        [\n            [[40, 40], [20, 45], [45, 30], [40, 40]]\n        ], \n        [\n            [[20, 35], [10, 30], [10, 10], [30, 5], [45, 20], [20, 35]], \n            [[30, 20], [20, 15], [20, 25], [30, 20]]\n        ]\n    ]\n}"
    )]
    public void MultiPolygon_Tests(string geom)
    {
        JsonSerializer.Deserialize<MultiPolygon>(geom, _settings)
                      .ShouldBeOfType<MultiPolygon>();
    }

    [Theory]
    [InlineData(
        "{\n    \"type\": \"GeometryCollection\",\n    \"geometries\": [\n        {\n            \"type\": \"Point\",\n            \"coordinates\": [40, 10]\n        },\n        {\n            \"type\": \"LineString\",\n            \"coordinates\": [\n                [10, 10], [20, 20], [10, 40]\n            ]\n        },\n        {\n            \"type\": \"Polygon\",\n            \"coordinates\": [\n                [[40, 40], [20, 45], [45, 30], [40, 40]]\n            ]\n        }\n    ]\n}"
    )]
    public void GeometryCollection_Tests(string geom)
    {
        JsonSerializer.Deserialize<GeometryCollection>(geom, _settings)
                      .ShouldBeOfType<GeometryCollection>();
    }
}
