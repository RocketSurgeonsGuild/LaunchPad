using Rocket.Surgery.LaunchPad.Serilog;

namespace NodaTime.Serilog.Tests;

public class NodaTimeSerilogBoundaryTests
{
    [Test]
    public void Depends_Only_On_NodaTime_Serilog_And_Dovetail()
    {
        var references = typeof(NodaTimeLoggerConfigurationExtensions)
                        .Assembly.GetReferencedAssemblies()
                        .Select(x => x.Name!)
                        .Where(x => x != "netstandard" && !x.StartsWith("System", StringComparison.Ordinal))
                        .ToList();

        // Joinery.Dovetail.Serilog is how an application gets Serilog; it brings Serilog and the ISerilogJoint the joint implements.
        references
           .Where(x => !x.StartsWith("Microsoft.", StringComparison.Ordinal))
           .Order()
           .ShouldBe(["Joinery.Dovetail", "Joinery.Dovetail.Serilog", "NodaTime", "Serilog"]);
        // The Dovetail types use the Microsoft.Extensions abstractions (which ones depends on the target framework), and nothing more.
        references
           .Where(x => x.StartsWith("Microsoft.", StringComparison.Ordinal))
           .ShouldAllBe(x => x.StartsWith("Microsoft.Extensions.", StringComparison.Ordinal) && x.EndsWith(".Abstractions", StringComparison.Ordinal));
    }
}
