using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Serilog;
using Serilog.Exceptions;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions;

/// <summary>
///     SerilogEnrichLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[ExportConvention]
public class SerilogEnrichLoggingConvention : ISerilogConvention
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
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        loggerConfiguration
           .Enrich.FromLogContext()
           .Enrich.WithExceptionDetails();
    }
}
