using Rocket.Surgery.LaunchPad.Spatial;

namespace Spatial.Serilog.Tests;

public class SpatialBoundaryTests
{
    [Test]
    public async Task Spatial_Carries_Its_Serilog_Destructuring()
    {
        var references = typeof(WktConverterFactory).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        // Serilog is a library every LaunchPad application references, so the destructuring ships in the spatial package itself.
        await references.Should().Contain("Serilog");
        await references.Should().Contain("Joinery.Dovetail.Serilog");
        await typeof(NetTopologySuiteLoggerConfigurationExtensions).Assembly.Should().BeEqualTo(typeof(WktConverterFactory).Assembly);
    }

    [Test]
    public async Task Spatial_Depends_Only_On_NetTopologySuite_Serilog_And_Dovetail()
    {
        var references = typeof(WktConverterFactory)
                        .Assembly.GetReferencedAssemblies()
                        .Select(x => x.Name!)
                        .Where(x => x != "netstandard" && !x.StartsWith("System", StringComparison.Ordinal))
                        .ToList();

        // Joinery.Dovetail.Serilog is how an application gets Serilog; it brings Serilog and the ISerilogJoint the joint implements.
        await references
           .Where(x => !x.StartsWith("Microsoft.", StringComparison.Ordinal))
           .Order()
           .Should().BeEquivalentTo(["Joinery.Dovetail", "Joinery.Dovetail.Serilog", "NetTopologySuite", "NetTopologySuite.Features", "NetTopologySuite.IO.GeoJSON4STJ", "Serilog"], CollectionOrdering.Matching);
        // The Dovetail types use the Microsoft.Extensions abstractions (which ones depends on the target framework), and nothing more.
        await references
           .Where(x => x.StartsWith("Microsoft.", StringComparison.Ordinal))
           .Should().All(x => x.StartsWith("Microsoft.Extensions.", StringComparison.Ordinal));
    }
}
