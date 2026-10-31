using Rocket.Surgery.LaunchPad.Foundation.Joints;

namespace NodaTime.NewtonsoftJson.Tests;

public class FoundationNewtonsoftJsonBoundaryTests
{
    [Test]
    public void Foundation_NewtonsoftJson_Does_Not_Reference_NodaTime()
    {
        var references = typeof(NewtonsoftJsonJoint).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToList();

        references.ShouldNotContain("NodaTime");
        references.ShouldNotContain("NodaTime.Serialization.JsonNet");
        references.ShouldNotContain("Rocket.Surgery.LaunchPad.NodaTime");
    }
}
