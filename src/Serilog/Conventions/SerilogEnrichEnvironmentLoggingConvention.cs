using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions;

/// <summary>
///     SerilogEnrichLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class SerilogEnrichEnvironmentLoggingConvention : ISerilogConvention
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

        loggerConfiguration
           .Enrich.WithEnvironmentUserName()
           .Enrich.WithMachineName()
           .Enrich.WithProcessId()
           .Enrich.WithProcessName()
           .Enrich.WithThreadId()
           .Enrich.WithAssemblyInformationalVersion()
           .Enrich.WithAssemblyName()
           .Enrich.WithAssemblyVersion()
            ;
    }
}
