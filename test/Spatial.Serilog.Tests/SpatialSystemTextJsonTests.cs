using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using NetTopologySuite.Geometries;

using Rocket.Surgery.LaunchPad.Spatial;

namespace Spatial.Serilog.Tests;

// Native AOT turns reflection off: the geometry types then resolve from the converter factory alone, also as an object or a derived type.
public class SpatialSystemTextJsonTests
{
    [Test]
    public async Task Writes_GeoJson_Without_Reflection()
    {
        var options = WithoutReflection(new JsonSerializerOptions().ConfigureGeoJsonForLaunchPad(null));

        var json = JsonSerializer.Serialize<object>(new Point(1, 2), options);

        await json.Should().Contain("\"type\":\"Point\"");
        await JsonSerializer.Deserialize<Point>(json, options)!.X.Should().BeEqualTo(1);
        await JsonSerializer.Deserialize<Geometry>(json, options).Should().BeOfType(typeof(Point));
    }

    [Test]
    public async Task Writes_Well_Known_Text_Without_Reflection()
    {
        var options = WithoutReflection(new JsonSerializerOptions().ConfigureWellKnownTextForLaunchPad(null));

        await JsonSerializer.Serialize<Geometry>(new Point(1, 2), options).Should().BeEqualTo("\"POINT (1 2)\"");
        await JsonSerializer.Deserialize<Geometry>("\"POINT (1 2)\"", options).Should().BeOfType(typeof(Point));
    }

    [Test]
    public async Task Chains_Only_Its_Own_Resolver_When_Configured_Standalone()
    {
        var options = new JsonSerializerOptions().ConfigureGeoJsonForLaunchPad(null);

        await options.TypeInfoResolverChain.Should().BeEquivalentTo([NetTopologySuiteSystemTextJsonSerializationExtensions.TypeInfoResolver], CollectionOrdering.Matching);
        await JsonSerializer.Serialize<Geometry>(new Point(1, 2), options).Should().Contain("\"type\":\"Point\"");
    }

    private static JsonSerializerOptions WithoutReflection(JsonSerializerOptions options) =>
        new(options) { TypeInfoResolver = JsonTypeInfoResolver.Combine(options.TypeInfoResolverChain.Where(x => x is not DefaultJsonTypeInfoResolver).ToArray()) };
}
