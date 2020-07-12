using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Functions;
using Xunit;
using Xunit.Abstractions;

namespace Functions.Tests
{
    internal class Startup : LaunchPadFunctionStartup, IServiceConvention
    {
        public override void Setup(IFunctionsHostBuilder builder) { }

        public void Register(IServiceConventionContext context) => context.Services.AddSingleton(new object());
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
        public void Should_Call_Through_To_Delegate_Methods()
        {
            var startup = new Startup();
            var services = new ServiceCollection()
               .AddSingleton<IHostEnvironment>(new HostEnvironment())
               .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            startup
               .UseScanner(AutoFake.Resolve<IConventionScanner>())
               .PrependDelegate(new Action(() => { }))
               .AppendDelegate(new Action(() => { }))
               .Configure(new WebJobsBuilder(services));

            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependDelegate(A<Delegate>._))
               .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().AppendDelegate(A<Delegate>._))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void Should_Call_Through_To_Convention_Methods()
        {
            var convention = AutoFake.Resolve<IConvention>();

            var startup = new Startup();
            var services = new ServiceCollection()
               .AddSingleton<IHostEnvironment>(new HostEnvironment())
               .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            startup
               .UseScanner(AutoFake.Resolve<IConventionScanner>())
               .PrependConvention(convention)
               .AppendConvention(convention)
               .Configure(new WebJobsBuilder(services));

            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependConvention(A<IConvention>._))
               .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().AppendConvention(A<IConvention>._))
               .MustHaveHappened(2, Times.Exactly);
        }

        [Fact]
        public void Should_UseAssemblies()
        {
            var startup = new Startup();
            var services = new ServiceCollection()
               .AddSingleton<IHostEnvironment>(new HostEnvironment())
               .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            startup
               .ConfigureForAssemblies(AppDomain.CurrentDomain.GetAssemblies())
               .Configure(new WebJobsBuilder(services));

            var sp = services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseRocketBooster()
        {
            var startup = new Startup();
            var services = new ServiceCollection()
               .AddSingleton<IHostEnvironment>(new HostEnvironment())
               .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            startup
               .UseRocketBooster(RocketBooster.For(AppDomain.CurrentDomain))
               .Configure(new WebJobsBuilder(services));

            var sp = services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseDependencyContext()
        {
            var startup = new Startup();
            var services = new ServiceCollection()
               .AddSingleton<IHostEnvironment>(new HostEnvironment())
               .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            startup
               .ConfigureForDependencyContext(DependencyContext.Default)
               .Configure(new WebJobsBuilder(services));

            var sp = services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        [Fact]
        public void Should_Build_The_Host_Correctly()
        {
            var serviceConventionFake = A.Fake<IServiceConvention>();
            var configurationConventionFake = A.Fake<IConfigConvention>();

            var startup = new Startup();
            var services = new ServiceCollection()
               .AddSingleton<IHostEnvironment>(new HostEnvironment())
               .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            startup
               .UseScanner(
                    new BasicConventionScanner(
                        A.Fake<IServiceProviderDictionary>(),
                        serviceConventionFake,
                        configurationConventionFake
                    )
                )
               .UseAssemblyCandidateFinder(new DefaultAssemblyCandidateFinder(new[] { typeof(RocketHostBuilderTests).Assembly }))
               .UseAssemblyProvider(new DefaultAssemblyProvider(new[] { typeof(RocketHostBuilderTests).Assembly }))
               .Configure(new WebJobsBuilder(services));

            var sp = services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        public RocketHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}