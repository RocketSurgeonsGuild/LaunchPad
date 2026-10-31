using Rocket.Surgery.LaunchPad.AspNetCore;

namespace FluentValidation.AspNetCore.Tests;

public class AspNetCoreBoundaryTests
{
    [Test]
    public void AspNetCore_Does_Not_Reference_FluentValidation()
    {
        var references = typeof(RestfulApiOptions).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        references.ShouldNotContain("FluentValidation");
        references.ShouldNotContain("FluentValidation.AspNetCore");
        references.ShouldNotContain("Rocket.Surgery.LaunchPad.FluentValidation");
        references.ShouldNotContain("Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi");
    }
}
