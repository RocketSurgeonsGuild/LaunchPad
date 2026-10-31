using Rocket.Surgery.LaunchPad.HotChocolate.Joints;

namespace FluentValidation.HotChocolate.Tests;

public class HotChocolateBoundaryTests
{
    [Test]
    public void HotChocolate_Does_Not_Reference_FluentValidation_Or_FairyBread()
    {
        var references = typeof(GraphqlJoint).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        references.ShouldNotContain("FluentValidation");
        references.ShouldNotContain("FairyBread");
    }
}
