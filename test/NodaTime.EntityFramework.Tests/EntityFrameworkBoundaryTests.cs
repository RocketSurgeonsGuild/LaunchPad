using Rocket.Surgery.LaunchPad.EntityFramework;

namespace NodaTime.EntityFramework.Tests;

public class EntityFrameworkBoundaryTests
{
    [Test]
    public void EntityFramework_Does_Not_Reference_NodaTime()
    {
        var references = typeof(LpContext<>).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        references.ShouldNotContain("NodaTime");
        references.ShouldNotContain("Rocket.Surgery.LaunchPad.NodaTime");
    }
}
