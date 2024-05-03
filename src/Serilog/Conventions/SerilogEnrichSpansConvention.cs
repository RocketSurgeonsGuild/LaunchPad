using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Serilog;
using Serilog.Enrichers.Span;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions;

/// <summary>
///     SerilogEnrichLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[ExportConvention]
public class SerilogEnrichSpansConvention : ISerilogConvention
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

        loggerConfiguration.Enrich.WithSpan(
            new()
            {
                IncludeBaggage = true,
                IncludeTags = true,
                IncludeOperationName = true,
                IncludeTraceFlags = true,
            }
        );
    }
}
