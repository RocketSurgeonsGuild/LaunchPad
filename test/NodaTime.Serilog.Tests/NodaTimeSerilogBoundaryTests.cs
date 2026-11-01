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
                        .Order()
                        .ToList();

        // Joinery.Dovetail.Serilog is how an application gets Serilog; it brings Serilog and the ISerilogJoint the joint implements, whose
        // signature takes the IServiceCollection of the DependencyInjection abstractions.
        references.ShouldBe(["Joinery.Dovetail", "Joinery.Dovetail.Serilog", "Microsoft.Extensions.DependencyInjection.Abstractions", "NodaTime", "Serilog"]);
    }
}
