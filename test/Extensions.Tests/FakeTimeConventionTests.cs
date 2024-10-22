using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Testing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Testing;

namespace Extensions.Tests;

public class FakeTimeConventionTests(ITestOutputHelper testOutputHelper) : ConventionFakeTest(testOutputHelper)
{
    [Fact]
    public async Task Clock_Convention_Default()
    {
        await Init(x => { x.Set(HostType.UnitTest); });
        var clock = Container.GetRequiredService<IClock>();
        clock.Should().BeAssignableTo<IClock>();
        clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(1577836800));
        clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(1577836800) + Duration.FromSeconds(1));
    }

    [Fact]
    public async Task Clock_Convention_Override()
    {
        await Init(x => { x.AppendConvention(new FakeTimeConvention(0, Duration.FromMinutes(1))); });

        var clock = ServiceProvider.GetRequiredService<IClock>();
        clock.Should().BeAssignableTo<IClock>();
        clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(0));
        clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(0) + Duration.FromMinutes(1));
    }
}

public class AutoRegisterOptions(ITestOutputHelper testOutputHelper) : ConventionFakeTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Register_Options()
    {
        await Init(x => x.ConfigureConfiguration(builder => builder.AddInMemoryCollection([new("OptionsA:A", "B"), new("OptionsB:B", "A"),])));
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
