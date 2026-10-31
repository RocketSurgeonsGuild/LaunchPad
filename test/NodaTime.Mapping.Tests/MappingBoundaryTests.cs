using Rocket.Surgery.LaunchPad.Mapping;

namespace NodaTime.Mapping.Tests;

public class MappingBoundaryTests
{
    [Test]
    public void Mapping_Does_Not_Reference_NodaTime()
    {
        var references = typeof(DateTimeMapper).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        references.ShouldNotContain("NodaTime");
        references.ShouldNotContain("Rocket.Surgery.LaunchPad.NodaTime");
    }
}
