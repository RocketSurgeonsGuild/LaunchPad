using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Analyzers.Tests;

public class MutableGeneratorTests(ITestContextAccessor testContext) : GeneratorTest(testContext)
{
    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<MutableGenerator>()
                 .AddReferences(
                      typeof(MutableAttribute)
                  );
    }
}
