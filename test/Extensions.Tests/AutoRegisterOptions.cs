using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Extensions.Tests;

public class AutoRegisterOptions() : JointTest()
{
    [Test]
    public async Task Should_Register_Options()
    {
        await Init(x => x.ConfigureConfiguration((_, builder) => builder.AddInMemoryCollection([new("OptionsA:A", "B"), new("OptionsB:B", "A")])));
        await ServiceProvider.GetRequiredService<IOptions<OptionsA>>().Value.A.Should().BeEqualTo("B");
        await ServiceProvider.GetRequiredService<IOptions<OptionsB>>().Value.B.Should().BeEqualTo("A");
    }

    [DovetailConfiguration("OptionsA")]
    [PublicAPI]
    internal class OptionsA
    {
        public required string A { get; set; }
    }

    [DovetailConfiguration("OptionsB")]
    [PublicAPI]
    internal class OptionsB
    {
        public required string B { get; set; }
    }
}
