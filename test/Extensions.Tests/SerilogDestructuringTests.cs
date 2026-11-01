using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

using Newtonsoft.Json.Linq;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Spatial;

using Serilog.Context;
using Serilog.Events;

namespace Extensions.Tests;

public class SerilogDestructuringTests : LoggerTest<TestRecord>
{
    [Test]
    public async Task Should_Destructure_NewtonsoftJson_JObject()
    {
        using var _ = CaptureLogs(out var logs);

        Logger.Information("This is just a test {@Data}", JObject.FromObject(new { test = true, system = new { data = "1234" } }));

        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NewtonsoftJson_JArray()
    {
        using var _ = CaptureLogs(out var logs);

        Logger.Information("This is just a test {@Data}", JArray.FromObject(new object[] { 1, "2", 3d }));

        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NewtonsoftJson_JValue()
    {
        var faker = new Faker
        {
            Random = new(17),
        };
        using var _ = CaptureLogs(out var logs);

        Logger.Information("This is just a test {@Data}", new JValue(faker.Random.Guid()));

        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NetTopologySuite_AttributesTable()
    {
        var value = new FeatureFactory().CreateRandomAttributes(
            ("id", TypeCode.Int32),
            ("label", TypeCode.String),
            ("number1", TypeCode.Double),
            ("number2", TypeCode.Int64)
        );

        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", value);
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    [Arguments(OgcGeometryType.Point, 5, false)]
    [Arguments(OgcGeometryType.Point, 5, true)]
    [Arguments(OgcGeometryType.LineString, 5, false)]
    [Arguments(OgcGeometryType.LineString, 5, true)]
    [Arguments(OgcGeometryType.Polygon, 5, false)]
    [Arguments(OgcGeometryType.Polygon, 5, true)]
    [Arguments(OgcGeometryType.MultiPoint, 5, false)]
    [Arguments(OgcGeometryType.MultiPoint, 5, true)]
    [Arguments(OgcGeometryType.MultiLineString, 5, false)]
    [Arguments(OgcGeometryType.MultiLineString, 5, true)]
    [Arguments(OgcGeometryType.MultiPolygon, 5, false)]
    //    [Arguments(OgcGeometryType.MultiPolygon, 5, true)]
    public async Task Should_Destructure_NetTopologySuite_FeatureCollection(OgcGeometryType type, int num, bool threeD)
    {
        var fc = new FeatureCollection();
        for (var i = 0; i < num; i++)
        {
            fc.Add(
                FeatureFactory.Create(
                    type,
                    threeD,
                    ("id", TypeCode.Int32),
                    ("label", TypeCode.String),
                    ("number1", TypeCode.Double),
                    ("number2", TypeCode.Int64)
                )
            );
        }

        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", fc);
        await Verify(logs.Select(z => z.RenderMessage())).UseParameters(type, num, threeD);
    }

    [Test]
    [Arguments(OgcGeometryType.Point, false)]
    [Arguments(OgcGeometryType.Point, true)]
    [Arguments(OgcGeometryType.LineString, false)]
    [Arguments(OgcGeometryType.LineString, true)]
    [Arguments(OgcGeometryType.Polygon, false)]
    [Arguments(OgcGeometryType.Polygon, true)]
    [Arguments(OgcGeometryType.MultiPoint, false)]
    [Arguments(OgcGeometryType.MultiPoint, true)]
    [Arguments(OgcGeometryType.MultiLineString, false)]
    [Arguments(OgcGeometryType.MultiLineString, true)]
    [Arguments(OgcGeometryType.MultiPolygon, false)]
    //    [Arguments(OgcGeometryType.MultiPolygon, true)]
    public async Task Should_Destructure_NetTopologySuite_Geometry(OgcGeometryType type, bool threeD)
    {
        var geometry = new FeatureFactory().CreateRandomGeometry(type, threeD);

        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", geometry);
        await Verify(logs.Select(z => z.RenderMessage())).UseParameters(type, threeD);
    }

    public SerilogDestructuringTests() : base(
        new(
            LogEventLevel.Information,
            configureLogger: (_, configuration) => configuration
                                                  .Destructure.NewtonsoftJsonTypes()
                                                  .Destructure.NetTopologySuiteTypes()
        )
    ) => LogContext.PushProperty("SourceContext", nameof(SerilogDestructuringTests));
}
