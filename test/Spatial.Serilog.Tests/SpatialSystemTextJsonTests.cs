using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using NetTopologySuite.Geometries;

using Rocket.Surgery.LaunchPad.Spatial;

namespace Spatial.Serilog.Tests;

// Native AOT turns reflection off: the geometry types then resolve from the converter factory alone, also as an object or a derived type.
public class SpatialSystemTextJsonTests
{
    [Test]
    public void Writes_GeoJson_Without_Reflection()
    {
        var options = WithoutReflection(new JsonSerializerOptions().ConfigureGeoJsonForLaunchPad(null));

        var json = JsonSerializer.Serialize<object>(new Point(1, 2), options);

        json.ShouldContain("\"type\":\"Point\"");
        JsonSerializer.Deserialize<Point>(json, options)!.X.ShouldBe(1);
        JsonSerializer.Deserialize<Geometry>(json, options).ShouldBeOfType<Point>();
    }

    [Test]
    public void Writes_Well_Known_Text_Without_Reflection()
    {
        var options = WithoutReflection(new JsonSerializerOptions().ConfigureWellKnownTextForLaunchPad(null));

        JsonSerializer.Serialize<Geometry>(new Point(1, 2), options).ShouldBe("\"POINT (1 2)\"");
        JsonSerializer.Deserialize<Geometry>("\"POINT (1 2)\"", options).ShouldBeOfType<Point>();
    }

    [Test]
    public void Chains_Only_Its_Own_Resolver_When_Configured_Standalone()
    {
        var options = new JsonSerializerOptions().ConfigureGeoJsonForLaunchPad(null);

        options.TypeInfoResolverChain.ShouldBe([NetTopologySuiteSystemTextJsonSerializationExtensions.TypeInfoResolver]);
        JsonSerializer.Serialize<Geometry>(new Point(1, 2), options).ShouldContain("\"type\":\"Point\"");
    }

    private static JsonSerializerOptions WithoutReflection(JsonSerializerOptions options) =>
        new(options) { TypeInfoResolver = JsonTypeInfoResolver.Combine(options.TypeInfoResolverChain.Where(x => x is not DefaultJsonTypeInfoResolver).ToArray()) };
}
