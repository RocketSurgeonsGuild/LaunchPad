using Rocket.Surgery.LaunchPad.AspNetCore;

namespace Serilog.AspNetCore.Tests;

public class AspNetCoreBoundaryTests
{
    [Test]
    public void AspNetCore_Does_Not_Reference_Serilog()
    {
        var references = typeof(RestfulApiOptions).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        references.ShouldNotContain("Serilog");
        references.ShouldNotContain("Serilog.AspNetCore");
        references.ShouldNotContain("Serilog.Extensions.Hosting");
    }
}
