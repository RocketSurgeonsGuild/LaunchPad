namespace FluentValidation.AspNetCore.Tests;

public class AspNetCoreBoundaryTests
{
    [Test]
    public async Task AspNetCore_Does_Not_Reference_FluentValidation()
    {
        var references = typeof(Rocket.Surgery.LaunchPad.AspNetCore.Joints.AspNetCoreJoint).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        await references.Should().NotContain("FluentValidation");
        await references.Should().NotContain("FluentValidation.AspNetCore");
        await references.Should().NotContain("Rocket.Surgery.LaunchPad.FluentValidation");
        await references.Should().NotContain("Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi");
    }
}
