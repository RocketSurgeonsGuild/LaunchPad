using Rocket.Surgery.LaunchPad.Foundation;

namespace FluentValidation.Tests;

public class FoundationBoundaryTests
{
    [Test]
    public async Task Foundation_Does_Not_Reference_FluentValidation()
    {
        var references = typeof(FoundationOptions).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        await references.Should().NotContain("FluentValidation");
        await references.Should().NotContain("FluentValidation.DependencyInjectionExtensions");
    }
}
