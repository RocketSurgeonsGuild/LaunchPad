using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    public static LoggerConfiguration ApplyConventions(
        this LoggerConfiguration configurationBuilder,
        IConventionContext context,
        IConfiguration configuration,
        IServiceProvider services
    )
    {
        foreach (var item in context.Conventions.Get<ISerilogConvention, SerilogConvention>())
        {
            if (item is ISerilogConvention convention)
                convention.Register(context, configuration, services, configurationBuilder);
            else if (item is SerilogConvention @delegate) @delegate(context, configuration, services, configurationBuilder);
        }

        return configurationBuilder;
    }
}
