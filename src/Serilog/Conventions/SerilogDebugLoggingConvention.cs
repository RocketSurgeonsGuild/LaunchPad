using System.Globalization;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Serilog;
using Serilog.Events;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions;

/// <summary>
///     SerilogDebugLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[ExportConvention]
[AfterConvention<LoggerConvention>]
[ConventionCategory(ConventionCategory.Core)]
public sealed class SerilogDebugLoggingConvention : ISerilogConvention
{
    private readonly LaunchPadLoggingOptions _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SerilogDebugLoggingConvention" /> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public SerilogDebugLoggingConvention(LaunchPadLoggingOptions? options = null)
    {
        _options = options ?? new LaunchPadLoggingOptions();
    }

    /// <inheritdoc />
    public void Register(
        IConventionContext context,
        IConfiguration configuration,
        IServiceProvider services,
        LoggerConfiguration loggerConfiguration
    )
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (!_options.EnableDebugLogging) return;

        loggerConfiguration.WriteTo.Async(
            c => c.Debug(
                LogEventLevel.Verbose,
                _options.DebugMessageTemplate,
                CultureInfo.InvariantCulture
            )
        );
    }
}
