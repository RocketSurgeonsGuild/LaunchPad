using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;

// ReSharper disable once CheckNamespace
namespace App.Metrics;

/// <summary>
///     Extension method to apply configuration conventions
/// </summary>
public static class RocketSurgerySerilogExtensions
{
    /// <summary>
    ///     Apply configuration conventions
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    public static LoggerConfiguration ApplyConventions(
        this LoggerConfiguration configurationBuilder, IConventionContext conventionContext, IServiceProvider services
    )
    {
        var configuration = conventionContext.Get<IConfiguration>()
                         ?? throw new ArgumentException("Configuration was not found in context", nameof(conventionContext));
        foreach (var item in conventionContext.Conventions.Get<ISerilogConvention, SerilogConvention>())
        {
            if (item is ISerilogConvention convention)
            {
                convention.Register(conventionContext, services, configuration, configurationBuilder);
            }
            else if (item is SerilogConvention @delegate)
            {
                @delegate(conventionContext, services, configuration, configurationBuilder);
            }
        }

        return configurationBuilder;
    }
}
