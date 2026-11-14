using Rocket.Surgery.LaunchPad.AspNetCore;

namespace Serilog.AspNetCore.Tests;

public class AspNetCoreBoundaryTests
{
    [Test]
    public async Task AspNetCore_Carries_Serilog_Request_Logging()
    {
        var references = typeof(Rocket.Surgery.LaunchPad.AspNetCore.Joints.AspNetCoreJoint).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        // Serilog is a library every LaunchPad application references, so the request logging ships in the ASP.NET Core package itself.
        await references.Should().Contain("Serilog");
        await references.Should().Contain("Serilog.AspNetCore");
        await typeof(LaunchPadRequestLoggingExtensions).Assembly.Should().BeEqualTo(typeof(Rocket.Surgery.LaunchPad.AspNetCore.Joints.AspNetCoreJoint).Assembly);
    }
}
