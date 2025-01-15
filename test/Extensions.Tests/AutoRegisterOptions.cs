using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Extensions.Tests;

public class AutoRegisterOptions(ITestOutputHelper testOutputHelper) : ConventionFakeTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Register_Options()
    {
        await Init(x => x.ConfigureConfiguration(builder => builder.AddInMemoryCollection([new("OptionsA:A", "B"), new("OptionsB:B", "A")])));
        ServiceProvider.GetRequiredService<IOptions<OptionsA>>().Value.A.Should().Be("B");
        ServiceProvider.GetRequiredService<IOptions<OptionsB>>().Value.B.Should().Be("A");
    }

    [RegisterOptionsConfiguration("OptionsA")]
    [PublicAPI]
    private class OptionsA
    {
        public required string A { get; set; }
    }

    [RegisterOptionsConfiguration("OptionsB")]
    [PublicAPI]
    private class OptionsB
    {
        public required string B { get; set; }
    }
}
