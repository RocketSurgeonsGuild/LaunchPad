using Rocket.Surgery.LaunchPad.EntityFramework;

namespace EntityFramework.Tests;

public class EntityFrameworkBoundaryTests
{
    [Test]
    public async Task EntityFramework_Does_Not_Reference_NodaTime()
    {
        var references = typeof(LpContext<>).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        await references.Should().NotContain("NodaTime");
        await references.Should().NotContain("Rocket.Surgery.LaunchPad.NodaTime");
    }
}
