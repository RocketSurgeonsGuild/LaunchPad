using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;

using Serilog;

namespace Rocket.Surgery.LaunchPad.WebAssembly.Hosting.Conventions;

/// <summary>
///     EnvironmentLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class EnvironmentLoggingConvention : ISerilogConvention
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// <param name="loggerConfiguration"></param>
    public void Register(
        IConventionContext context,
        IConfiguration configuration,
        IServiceProvider services,
        LoggerConfiguration loggerConfiguration
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        if (context.Get<IWebAssemblyHostEnvironment>() is not { } environment) return;
        loggerConfiguration.Enrich.WithProperty(nameof(environment.Environment), environment.Environment);
        loggerConfiguration.Enrich.WithProperty(nameof(environment.BaseAddress), environment.BaseAddress);
    }
}
