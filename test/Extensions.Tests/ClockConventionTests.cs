using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Rocket.Surgery.Conventions.TestHost;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests
{
    public class ClockConventionTests : AutoFakeTest
    {
        public ClockConventionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void Clock_Convention_Default()
        {
            var clock = ConventionTestHostBuilder.For(typeof(ClockConventionTests), LoggerFactory)
               .WithLogger(Logger)
               .Create()
               .Build()
               .ServiceProvider.GetRequiredService<IClock>();

            clock.Should().BeOfType<FakeClock>();
            clock.GetCurrentInstant().Should().Be(Instant.FromUnixTimeSeconds(1577836800));
        }
    }
}