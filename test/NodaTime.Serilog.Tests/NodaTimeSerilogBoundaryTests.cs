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
                        .Where(x => x != "netstandard" && !x.StartsWith("System", StringComparison.Ordinal) && !x.StartsWith("Microsoft", StringComparison.Ordinal))
                        .Order()
                        .ToList();

        // Joinery.Dovetail.Serilog is how an application gets Serilog; it brings Serilog and the ISerilogJoint the joint implements.
        references.ShouldBe(["Joinery.Dovetail", "Joinery.Dovetail.Serilog", "NodaTime", "Serilog"]);
    }
}
