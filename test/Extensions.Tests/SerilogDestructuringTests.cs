using System.Text.Json;

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

using Newtonsoft.Json.Linq;

using NodaTime;
using NodaTime.Testing;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Serilog;
using Rocket.Surgery.LaunchPad.Spatial;

using Serilog.Context;
using Serilog.Events;

namespace Extensions.Tests;

public class SerilogDestructuringTests : LoggerTest<XUnitTestContext>
{
    [Fact]
    public async Task Should_Destructure_Sjt_Values_JsonElement()
    {
        using var _ = CaptureLogs(out var logs);

        Logger.Information(
            "This is just a test {@Data}",
            JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new { test = true, system = new { data = "1234", }, }), options: null)
        );

        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_Sjt_Values_JsonDocument()
    {
        using var _ = CaptureLogs(out var logs);

        Logger.Information(
            "This is just a test {@Data}",
            JsonSerializer.Deserialize<JsonDocument>(JsonSerializer.Serialize(new { test = true, system = new { data = "1234", }, }), options: null)
        );

        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NewtonsoftJson_JObject()
    {
        using var _ = CaptureLogs(out var logs);

        Logger.Information("This is just a test {@Data}", JObject.FromObject(new { test = true, system = new { data = "1234", }, }));

        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NewtonsoftJson_JArray()
    {
        using var _ = CaptureLogs(out var logs);

        Logger.Information("This is just a test {@Data}", JArray.FromObject(new object[] { 1, "2", 3d, }));

        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
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

    [Fact]
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

    [Fact]
    public async Task Should_Destructure_NodaTime_Instant()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", _clock.GetCurrentInstant());
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_LocalDateTime()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", LocalDateTime.FromDateTime(_clock.GetCurrentInstant().ToDateTimeUtc()));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_LocalDate()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", LocalDate.FromDateTime(_clock.GetCurrentInstant().ToDateTimeUtc()));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_LocalTime()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", LocalTime.FromHoursSinceMidnight(4));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_OffsetDateTime()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", OffsetDateTime.FromDateTimeOffset(_clock.GetCurrentInstant().ToDateTimeOffset()));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_OffsetDate()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", OffsetDateTime.FromDateTimeOffset(_clock.GetCurrentInstant().ToDateTimeOffset()).ToOffsetDate());
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_OffsetTime()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", OffsetDateTime.FromDateTimeOffset(_clock.GetCurrentInstant().ToDateTimeOffset()).ToOffsetTime());
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_ZonedDateTime()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", ZonedDateTime.FromDateTimeOffset(_clock.GetCurrentInstant().ToDateTimeOffset()));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_DateTimeZone()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", DateTimeZoneProviders.Tzdb["America/New_York"]);
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_Duration()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", Duration.FromDays(1));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_Period()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", Period.FromDays(1));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Fact]
    public async Task Should_Destructure_NodaTime_Interval()
    {
        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", new Interval(_clock.GetCurrentInstant(), _clock.GetCurrentInstant()));
        await Verify(logs.Select(z => z.RenderMessage()));
    }

    public SerilogDestructuringTests(ITestOutputHelper outputHelper) : base(
        XUnitTestContext.Create(
            outputHelper,
            LogEventLevel.Information,
            configureLogger: (_, configuration) => configuration
                                                  .Destructure.NewtonsoftJsonTypes()
                                                  .Destructure.SystemTextJsonTypes()
                                                  .Destructure.NetTopologySuiteTypes()
                                                  .Destructure.NodaTimeTypes()
        )
    )

    {
        LogContext.PushProperty("SourceContext", nameof(SerilogDestructuringTests));
        _clock = new(
            Instant.FromUtc(
                2022,
                1,
                1,
                4,
                4,
                4
            )
        );
    }

    private readonly FakeClock _clock;

    [Theory]
    [InlineData(OgcGeometryType.Point, 5, false)]
    [InlineData(OgcGeometryType.Point, 5, true)]
    [InlineData(OgcGeometryType.LineString, 5, false)]
    [InlineData(OgcGeometryType.LineString, 5, true)]
    [InlineData(OgcGeometryType.Polygon, 5, false)]
    [InlineData(OgcGeometryType.Polygon, 5, true)]
    [InlineData(OgcGeometryType.MultiPoint, 5, false)]
    [InlineData(OgcGeometryType.MultiPoint, 5, true)]
    [InlineData(OgcGeometryType.MultiLineString, 5, false)]
    [InlineData(OgcGeometryType.MultiLineString, 5, true)]
    [InlineData(OgcGeometryType.MultiPolygon, 5, false)]
    //    [InlineData(OgcGeometryType.MultiPolygon, 5, true)]
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

    [Theory]
    [InlineData(OgcGeometryType.Point, false)]
    [InlineData(OgcGeometryType.Point, true)]
    [InlineData(OgcGeometryType.LineString, false)]
    [InlineData(OgcGeometryType.LineString, true)]
    [InlineData(OgcGeometryType.Polygon, false)]
    [InlineData(OgcGeometryType.Polygon, true)]
    [InlineData(OgcGeometryType.MultiPoint, false)]
    [InlineData(OgcGeometryType.MultiPoint, true)]
    [InlineData(OgcGeometryType.MultiLineString, false)]
    [InlineData(OgcGeometryType.MultiLineString, true)]
    [InlineData(OgcGeometryType.MultiPolygon, false)]
    //    [InlineData(OgcGeometryType.MultiPolygon, true)]
    public async Task Should_Destructure_NetTopologySuite_Geometry(OgcGeometryType type, bool threeD)
    {
        var geometry = new FeatureFactory().CreateRandomGeometry(type, threeD);

        using var _ = CaptureLogs(out var logs);
        Logger.Information("This is just a test {@Data}", geometry);
        await Verify(logs.Select(z => z.RenderMessage())).UseParameters(type, threeD);
    }
}
