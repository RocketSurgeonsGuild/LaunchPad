using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Joints;

namespace NewtonsoftJson.Serilog.Tests;

public class NewtonsoftJsonBoundaryTests
{
    [Test]
    public void Foundation_NewtonsoftJson_Does_Not_Reference_Serilog()
    {
        var references = typeof(NewtonsoftJsonJoint).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        references.ShouldNotContain("Serilog");
        references.ShouldNotContain("Joinery.Dovetail.Serilog");
    }

    [Test]
    public void NewtonsoftJson_Serilog_Depends_Only_On_Newtonsoft_Json_Serilog_And_Dovetail()
    {
        var references = typeof(NewtonsoftJsonLoggerConfigurationExtensions)
                        .Assembly.GetReferencedAssemblies()
                        .Select(x => x.Name!)
                        .Where(x => x != "netstandard" && !x.StartsWith("System", StringComparison.Ordinal))
                        .ToList();

        // Joinery.Dovetail.Serilog is how an application gets Serilog; it brings Serilog and the ISerilogJoint the joint implements.
        references
           .Where(x => !x.StartsWith("Microsoft.", StringComparison.Ordinal))
           .Order()
           .ShouldBe(["Joinery.Dovetail", "Joinery.Dovetail.Serilog", "Newtonsoft.Json", "Serilog"]);
        // The Dovetail types use the Microsoft.Extensions abstractions (which ones depends on the target framework), and nothing more.
        references
           .Where(x => x.StartsWith("Microsoft.", StringComparison.Ordinal))
           .ShouldAllBe(x => x.StartsWith("Microsoft.Extensions.", StringComparison.Ordinal) && x.EndsWith(".Abstractions", StringComparison.Ordinal));
    }
}
