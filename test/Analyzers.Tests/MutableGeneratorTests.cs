using Analyzers.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Analyzers.Tests;

public class MutableGeneratorTests : GeneratorTest
{
    public MutableGeneratorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, LogLevel.Trace)
    {
        WithGenerator<MutableGenerator>();
        AddReferences(typeof(MutableAttribute));
    }
}
