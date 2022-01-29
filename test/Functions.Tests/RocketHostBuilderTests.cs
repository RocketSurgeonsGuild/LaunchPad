using System;
using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Functions;
using Xunit;
using Xunit.Abstractions;

namespace Functions.Tests;

internal class Startup : LaunchPadFunctionStartup, IServiceConvention
{
    public Startup()
    {
    }

    public Startup([NotNull] Func<LaunchPadFunctionStartup, ConventionContextBuilder> configure) : base(configure)
    {
    }

    public override void Configure(IFunctionsHostBuilder builder, IConventionContext context)
    {
    }

    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton(new object());
    }
}

public class RocketHostBuilderTests : AutoFakeTest
{
    [Fact]
    public void Should_UseAssemblies()
    {
        var startup = new Startup(RocketBooster.ForAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        var configBuilder = new ConfigurationBuilder();
        var functionsConfigurationBuilder = ConfigureConfiguration(configBuilder);
        var functionsHostBuilder = ConfigureHost();
        startup.ConfigureAppConfiguration(functionsConfigurationBuilder);
        var services = new ServiceCollection();
        startup.Configure(functionsHostBuilder);

        var sp = services.BuildServiceProvider();
        sp.Should().NotBeNull();
    }

    [Fact]
    public void Should_UseRocketBooster()
    {
        var startup = new Startup(RocketBooster.ForAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        var configBuilder = new ConfigurationBuilder();
        var functionsConfigurationBuilder = ConfigureConfiguration(configBuilder);
        var functionsHostBuilder = ConfigureHost();
        startup.ConfigureAppConfiguration(functionsConfigurationBuilder);
        var services = new ServiceCollection();
        startup.Configure(functionsHostBuilder);

        var sp = services.BuildServiceProvider();
        sp.Should().NotBeNull();
    }

    [Fact]
    public void Should_Build_The_Host_Correctly()
    {
        var startup = new Startup(RocketBooster.ForAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        var configBuilder = new ConfigurationBuilder();
        var functionsConfigurationBuilder = ConfigureConfiguration(configBuilder);
        var functionsHostBuilder = ConfigureHost();
        startup.ConfigureAppConfiguration(functionsConfigurationBuilder);
        var services = new ServiceCollection();
        startup.Configure(functionsHostBuilder);

        var sp = services.BuildServiceProvider();
        sp.Should().NotBeNull();
    }

    public RocketHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    private static IFunctionsConfigurationBuilder ConfigureConfiguration(IConfigurationBuilder configurationBuilder)
    {
        var functionsConfigurationBuilder = A.Fake<IFunctionsConfigurationBuilder>();
        A.CallTo(() => functionsConfigurationBuilder.ConfigurationBuilder).Returns(configurationBuilder);
        return functionsConfigurationBuilder;
    }

    private static IFunctionsHostBuilder ConfigureHost()
    {
        var functionsHostBuilder = A.Fake<IFunctionsHostBuilder>();
        return functionsHostBuilder;
    }
}
