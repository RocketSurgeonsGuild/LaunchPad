using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Testing;

namespace Extensions.Tests;

public class FakeClockConventionTests(ITestOutputHelper testOutputHelper) : ConventionFakeTest(testOutputHelper)
{
    [Fact]
    public async Task Clock_Convention_Default()
    {
        await Init(x => { x.Set(HostType.UnitTest); });
        var clock = Container.GetRequiredService<IClock>();
        clock.Should().BeOfType<FakeClock>();
        clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(1577836800));
        clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(1577836800) + Duration.FromSeconds(1));
    }

    [Fact]
    public async Task Clock_Convention_Override()
    {
        await Init(x => { x.AppendConvention(new FakeClockConvention(0, Duration.FromMinutes(1))); });

        var clock = ServiceProvider.GetRequiredService<IClock>();
        clock.Should().BeOfType<FakeClock>();
        clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(0));
        clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(0) + Duration.FromMinutes(1));
    }
}