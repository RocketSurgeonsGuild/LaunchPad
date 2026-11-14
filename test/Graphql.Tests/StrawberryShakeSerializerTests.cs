using System.Text.Json;
using System.Text.Json.Serialization;

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
    public async Task Should_Format_A_Value_As_Its_Json_Element()
    {
        var serializer = new LaunchSerializer(Options.Create(new JsonSerializerOptions(JsonSerializerDefaults.Web) { TypeInfoResolver = LaunchJsonContext.Default }));

        var element = (JsonElement)serializer.Format(new Launch("Artemis", 2))!;

        await element.GetProperty("name").GetString().Should().BeEqualTo("Artemis");
        await element.GetProperty("crew").GetInt32().Should().BeEqualTo(2);
        await serializer.TypeName.Should().BeEqualTo("Launch");
    }

    [Test]
    public async Task Should_Parse_A_Json_Element_With_The_Configured_Options()
    {
        var serializer = new LaunchSerializer(Options.Create(new JsonSerializerOptions(JsonSerializerDefaults.Web) { TypeInfoResolver = LaunchJsonContext.Default }));

        await serializer.Parse(JsonSerializer.SerializeToElement(new { name = "Artemis", crew = 4, })).Should().BeEqualTo(new Launch("Artemis", 4));
    }

    [Test]
    public async Task Should_Format_An_Enum_As_Its_Number()
    {
        var serializer = new StageSerializer();

        await serializer.Format(Stage.Orbit).Should().BeEqualTo(2);
        await serializer.TypeName.Should().BeEqualTo("Stage");
    }

    [Test]
    public async Task Should_Parse_A_Number_As_Its_Enum() => await new StageSerializer().Parse(1).Should().BeEqualTo(Stage.Ascent);

    [Test]
    public async Task Should_Round_Trip_A_Geometry_As_GeoJson()
    {
        var serializer = new GeometrySerializer(SpatialOptions());
        var point = new Point(30, 10) { SRID = 4326, };

        var element = (JsonElement)serializer.Format(point)!;

        await element.GetProperty("type").GetString().Should().BeEqualTo("Point");
        await serializer.Parse(element).EqualsExact(point).Should().BeTrue();
        await serializer.TypeName.Should().BeEqualTo("Geometry");
    }

    // the options the spatial joint configures
    private static IOptions<JsonSerializerOptions> SpatialOptions() =>
        Options.Create(new JsonSerializerOptions(JsonSerializerDefaults.Web).ConfigureGeoJsonForLaunchPad(NtsGeometryServices.Instance.CreateGeometryFactory(4326)));

    private sealed class LaunchSerializer(IOptions<JsonSerializerOptions> options) : JsonElementScalarSerializer<Launch>("Launch", options);

    private enum Stage
    {
        Pad,
        Ascent,
        Orbit,
    }

    private sealed class StageSerializer() : EnumScalarSerializer<Stage>("Stage");
}

// The serializer reads and writes only with the metadata of the options' JSON context: LaunchPad doesn't reflect.
internal sealed record Launch(string Name, int Crew);

[JsonSerializable(typeof(Launch))]
internal sealed partial class LaunchJsonContext : JsonSerializerContext;
