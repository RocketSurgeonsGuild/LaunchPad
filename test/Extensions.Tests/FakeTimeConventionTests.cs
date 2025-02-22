﻿using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Testing;

namespace Extensions.Tests;

public class FakeTimeConventionTests(ITestOutputHelper testOutputHelper) : ConventionFakeTest(testOutputHelper)
{
    [Fact]
    public async Task Clock_Convention_Default()
    {
        await Init(x => { x.Set(HostType.UnitTest); });
        var clock = Container.GetRequiredService<IClock>();
        clock.ShouldBeAssignableTo<IClock>();
        clock.GetCurrentInstant().ShouldBe(Instant.FromUnixTimeSeconds(1577836800));
        clock.GetCurrentInstant().ShouldBe(Instant.FromUnixTimeSeconds(1577836800) + Duration.FromSeconds(1));
    }

    [Fact]
    public async Task Clock_Convention_Override()
    {
        await Init(x => { x.AppendConvention(new FakeTimeConvention(0, Duration.FromMinutes(1))); });

        var clock = ServiceProvider.GetRequiredService<IClock>();
        clock.ShouldBeAssignableTo<IClock>();
        clock.GetCurrentInstant().ShouldBe(Instant.FromUnixTimeSeconds(0));
        clock.GetCurrentInstant().ShouldBe(Instant.FromUnixTimeSeconds(0) + Duration.FromMinutes(1));
    }
}
