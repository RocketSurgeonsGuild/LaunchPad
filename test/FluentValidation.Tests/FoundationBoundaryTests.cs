using Rocket.Surgery.LaunchPad.Foundation;

namespace FluentValidation.Tests;

public class FoundationBoundaryTests
{
    [Test]
    public void Foundation_Does_Not_Reference_FluentValidation()
    {
        var references = typeof(FoundationOptions).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        references.ShouldNotContain("FluentValidation");
        references.ShouldNotContain("FluentValidation.DependencyInjectionExtensions");
    }
}
