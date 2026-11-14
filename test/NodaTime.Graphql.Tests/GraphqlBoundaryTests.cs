using Rocket.Surgery.LaunchPad.HotChocolate;
using Rocket.Surgery.LaunchPad.StrawberryShake;

namespace NodaTime.Graphql.Tests;

public class GraphqlBoundaryTests
{
    [Test]
    public async Task HotChocolate_Does_Not_Reference_NodaTime()
    {
        var references = typeof(GraphqlExtensions).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        await references.Should().NotContain("NodaTime");
        await references.Should().NotContain("HotChocolate.Types.NodaTime");
        await references.Should().NotContain("Rocket.Surgery.LaunchPad.NodaTime");
    }

    [Test]
    public async Task StrawberryShake_Does_Not_Reference_NodaTime()
    {
        var references = typeof(JsonElementScalarSerializer<>).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        await references.Should().NotContain("NodaTime");
        await references.Should().NotContain("Rocket.Surgery.LaunchPad.NodaTime");
    }
}
