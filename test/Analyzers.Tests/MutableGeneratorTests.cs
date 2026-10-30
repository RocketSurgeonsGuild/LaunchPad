using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Analyzers.Tests;

public class MutableGeneratorTests() : GeneratorTest()
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<MutableGenerator>()
                 .AddReferences(
                      typeof(MutableAttribute)
                  );
    }
}
