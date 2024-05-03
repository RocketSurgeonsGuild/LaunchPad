using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

/// <summary>
///     EnvironmentLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[ExportConvention]
public class EnvironmentLoggingConvention : ISerilogConvention
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="loggerConfiguration"></param>
    public void Register(
        IConventionContext context,
        IConfiguration configuration,
        IServiceProvider services,
        LoggerConfiguration loggerConfiguration
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        if (context.Get<IHostEnvironment>() is not { } environment) return;
        loggerConfiguration.Enrich.WithProperty(nameof(environment.EnvironmentName), environment.EnvironmentName);
        loggerConfiguration.Enrich.WithProperty(nameof(environment.ApplicationName), environment.ApplicationName);
    }
}