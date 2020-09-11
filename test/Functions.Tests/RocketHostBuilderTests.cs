using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Functions;
using Xunit;
using Xunit.Abstractions;

namespace Functions.Tests
{
    internal class Startup : LaunchPadFunctionStartup, IServiceConvention
    {
        public Startup() : base() { }
        public Startup([NotNull] Func<LaunchPadFunctionStartup, ConventionContextBuilder> configure) : base(configure) { }
        public override void Setup(IFunctionsHostBuilder builder, ConventionContextBuilder contextBuilder) { }

        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services) => services.AddSingleton(new object());
    }

    internal class HostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Test";
        public string ApplicationName { get; set; } = "Test";
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }

    class WebJobsBuilder : IWebJobsBuilder
    {
        public WebJobsBuilder(IServiceCollection services) => Services = services;
        public IServiceCollection Services { get; }
    }

    public class RocketHostBuilderTests : AutoFakeTest
    {
        [Fact]
        public void Should_UseAssemblies()
        {
            var startup = new Startup(RocketBooster.ForAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            var services = new ServiceCollection()
               .AddSingleton(
                    new HostBuilderContext(new Dictionary<object, object>())
                    {
                        Configuration = new ConfigurationBuilder().Build(),
                        HostingEnvironment = new HostEnvironment()
                    }
                );
            startup.Configure(new WebJobsBuilder(services));

            var sp = services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseRocketBooster()
        {
            var startup = new Startup(RocketBooster.ForAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            var services = new ServiceCollection()
               .AddSingleton(
                    new HostBuilderContext(new Dictionary<object, object>())
                    {
                        Configuration = new ConfigurationBuilder().Build(),
                        HostingEnvironment = new HostEnvironment()
                    }
                );
            startup.Configure(new WebJobsBuilder(services));

            var sp = services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        [Fact]
        public void Should_Build_The_Host_Correctly()
        {
            var startup = new Startup(RocketBooster.ForAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            var services = new ServiceCollection()
               .AddSingleton(
                    new HostBuilderContext(new Dictionary<object, object>())
                    {
                        Configuration = new ConfigurationBuilder().Build(),
                        HostingEnvironment = new HostEnvironment()
                    }
                );
            startup.Configure(new WebJobsBuilder(services));

            var sp = services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        public RocketHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}