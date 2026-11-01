using Rocket.Surgery.LaunchPad.NodaTime;

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

        // The destructuring lives in Rocket.Surgery.LaunchPad.NodaTime itself, next to the System.Text.Json converters. Joinery.Dovetail.Serilog
        // is how an application gets Serilog; it brings Serilog and the ISerilogJoint the joint implements.
        references
           .Where(x => !x.StartsWith("Microsoft.", StringComparison.Ordinal))
           .Order()
           .ShouldBe(["Joinery.Dovetail", "Joinery.Dovetail.Serilog", "NodaTime", "NodaTime.Serialization.SystemTextJson", "Serilog"]);
        // The Dovetail types use the Microsoft.Extensions abstractions (which ones depends on the target framework), and the JSON joint
        // configures the System.Text.Json options through Microsoft.Extensions.Options; nothing more.
        references
           .Where(x => x.StartsWith("Microsoft.", StringComparison.Ordinal))
           .ShouldAllBe(x => x.StartsWith("Microsoft.Extensions.", StringComparison.Ordinal) && ( x.EndsWith(".Abstractions", StringComparison.Ordinal) || x == "Microsoft.Extensions.Options" ));
    }
}
