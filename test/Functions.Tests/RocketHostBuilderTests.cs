using FakeItEasy;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Functions;

namespace Functions.Tests;

internal sealed  class Startup : LaunchPadFunctionStartup, IServiceConvention
{
    public Startup()
    {
    }

    public Startup(Func<LaunchPadFunctionStartup, ConventionContextBuilder> configure) : base(configure)
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

public class RocketHostBuilderTests(ITestOutputHelper outputHelper) : AutoFakeTest(outputHelper)
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
