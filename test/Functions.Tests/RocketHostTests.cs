using System;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.LaunchPad.Functions;
using Xunit;

namespace Functions.Tests
{
    public class RocketHostTests
    {
        [Fact]
        public void Creates_RocketHost_ForAppDomain()
        {
            new Startup()
               .UseRocketBooster(RocketBooster.For(AppDomain.CurrentDomain))
               .Configure(
                    new WebJobsBuilder(
                        new ServiceCollection()
                           .AddSingleton<IHostEnvironment>(new HostEnvironment())
                           .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
                    )
                );
        }

        [Fact]
        public void Creates_RocketHost_ForAssemblies()
        {
            new Startup()
               .UseRocketBooster(RocketBooster.For(new[] { typeof(RocketHostTests).Assembly }))
               .Configure(
                    new WebJobsBuilder(
                        new ServiceCollection()
                           .AddSingleton<IHostEnvironment>(new HostEnvironment())
                           .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
                    )
                );
        }
    }
}