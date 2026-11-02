using Rocket.Surgery.LaunchPad.AspNetCore;

namespace Serilog.AspNetCore.Tests;

public class AspNetCoreBoundaryTests
{
    [Test]
    public void AspNetCore_Carries_Serilog_Request_Logging()
    {
        var references = typeof(RestfulApiOptions).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        // Serilog is a library every LaunchPad application references, so the request logging ships in the ASP.NET Core package itself.
        references.ShouldContain("Serilog");
        references.ShouldContain("Serilog.AspNetCore");
        typeof(LaunchPadRequestLoggingExtensions).Assembly.ShouldBe(typeof(RestfulApiOptions).Assembly);
    }
}
