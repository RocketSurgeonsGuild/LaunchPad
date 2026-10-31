using Rocket.Surgery.LaunchPad.HotChocolate;
using Rocket.Surgery.LaunchPad.StrawberryShake;

namespace NodaTime.Graphql.Tests;

public class GraphqlBoundaryTests
{
    [Test]
    public void HotChocolate_Does_Not_Reference_NodaTime()
    {
        var references = typeof(GraphqlExtensions).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        references.ShouldNotContain("NodaTime");
        references.ShouldNotContain("HotChocolate.Types.NodaTime");
        references.ShouldNotContain("Rocket.Surgery.LaunchPad.NodaTime");
    }

    [Test]
    public void StrawberryShake_Does_Not_Reference_NodaTime()
    {
        var references = typeof(JsonElementScalarSerializer<>).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        references.ShouldNotContain("NodaTime");
        references.ShouldNotContain("Rocket.Surgery.LaunchPad.NodaTime");
    }
}
