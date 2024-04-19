using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Conventions;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions;

/// <summary>
///     ConfigureOptionsLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[ExportConvention]
public class ConfigureOptionsLoggingConvention : ISerilogConvention
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
        IServiceProvider services,
        IConfiguration configuration,
        LoggerConfiguration loggerConfiguration
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        foreach (var setup in services.GetServices<IConfigureOptions<LoggerConfiguration>>())
        {
            if (setup is IConfigureNamedOptions<LoggerConfiguration> namedSetup)
                namedSetup.Configure(Options.DefaultName, loggerConfiguration);
            else
                setup.Configure(loggerConfiguration);
        }

        foreach (var post in services.GetServices<IPostConfigureOptions<LoggerConfiguration>>())
        {
            post.PostConfigure(Options.DefaultName, loggerConfiguration);
        }
    }
}
