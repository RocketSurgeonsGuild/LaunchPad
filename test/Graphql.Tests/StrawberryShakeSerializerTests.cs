using System.Text.Json;

using Microsoft.Extensions.Options;

using NetTopologySuite;
using NetTopologySuite.Geometries;

using Rocket.Surgery.LaunchPad.Spatial;
using Rocket.Surgery.LaunchPad.StrawberryShake;
using Rocket.Surgery.LaunchPad.StrawberryShake.Spatial;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Graphql.Tests;

public class StrawberryShakeSerializerTests
{
    [Test]
    public void Should_Format_A_Value_As_Its_Json_Element()
    {
        var serializer = new LaunchSerializer(Options.Create(new JsonSerializerOptions(JsonSerializerDefaults.Web)));

        var element = (JsonElement)serializer.Format(new Launch("Artemis", 2))!;

        element.GetProperty("name").GetString().ShouldBe("Artemis");
        element.GetProperty("crew").GetInt32().ShouldBe(2);
        serializer.TypeName.ShouldBe("Launch");
    }

    [Test]
    public void Should_Parse_A_Json_Element_With_The_Configured_Options()
    {
        var serializer = new LaunchSerializer(Options.Create(new JsonSerializerOptions(JsonSerializerDefaults.Web)));

        serializer.Parse(JsonSerializer.SerializeToElement(new { name = "Artemis", crew = 4, })).ShouldBe(new Launch("Artemis", 4));
    }

    [Test]
    public void Should_Format_An_Enum_As_Its_Number()
    {
        var serializer = new StageSerializer();

        serializer.Format(Stage.Orbit).ShouldBe(2);
        serializer.TypeName.ShouldBe("Stage");
    }

    [Test]
    public void Should_Parse_A_Number_As_Its_Enum() => new StageSerializer().Parse(1).ShouldBe(Stage.Ascent);

    [Test]
    public void Should_Round_Trip_A_Geometry_As_GeoJson()
    {
        var serializer = new GeometrySerializer(SpatialOptions());
        var point = new Point(30, 10) { SRID = 4326, };

        var element = (JsonElement)serializer.Format(point)!;

        element.GetProperty("type").GetString().ShouldBe("Point");
        serializer.Parse(element).EqualsExact(point).ShouldBeTrue();
        serializer.TypeName.ShouldBe("Geometry");
    }

    // the options the spatial joint configures
    private static IOptions<JsonSerializerOptions> SpatialOptions() =>
        Options.Create(new JsonSerializerOptions(JsonSerializerDefaults.Web).ConfigureGeoJsonForLaunchPad(NtsGeometryServices.Instance.CreateGeometryFactory(4326)));

    private sealed record Launch(string Name, int Crew);

    private sealed class LaunchSerializer(IOptions<JsonSerializerOptions> options) : JsonElementScalarSerializer<Launch>("Launch", options);

    private enum Stage
    {
        Pad,
        Ascent,
        Orbit,
    }

    private sealed class StageSerializer() : EnumScalarSerializer<Stage>("Stage");
}
