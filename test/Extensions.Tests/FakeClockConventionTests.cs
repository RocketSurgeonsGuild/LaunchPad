using DryIoc;
using FluentAssertions;
using FluentValidation.Validators;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.Extensions.Validation;
using Rocket.Surgery.LaunchPad.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests
{
    public class FakeClockConventionTests : ConventionFakeTest
    {
        public FakeClockConventionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void Clock_Convention_Default()
        {
            Init(
                x =>
                {
                    x.Set(HostType.UnitTest);
                });
            var clock = Container.GetRequiredService<IClock>();
            clock.Should().BeOfType<FakeClock>();
            clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(1577836800));
            clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(1577836800) + Duration.FromSeconds(1));
        }

        [Fact]
        public void Clock_Convention_Override()
        {
            Init(
                x =>
                {
                    x.AppendConvention(new FakeClockConvention(0, Duration.FromMinutes(1)));
                }
            );

            var clock = ServiceProvider.GetRequiredService<IClock>();
            clock.Should().BeOfType<FakeClock>();
            clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(0));
            clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(0) + Duration.FromMinutes(1));
        }
    }
}