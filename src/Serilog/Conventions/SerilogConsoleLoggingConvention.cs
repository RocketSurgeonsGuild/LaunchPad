using System.Globalization;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions;

/// <summary>
///     SerilogConsoleLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[ExportConvention]
[AfterConvention<LoggerConvention>]
[ConventionCategory(ConventionCategory.Core)]
public sealed class SerilogConsoleLoggingConvention : ISerilogConvention
{
    private readonly LaunchPadLoggingOptions _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SerilogConsoleLoggingConvention" /> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public SerilogConsoleLoggingConvention(LaunchPadLoggingOptions? options = null)
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

        if (!_options.EnableConsoleLogging) return;

        loggerConfiguration.WriteTo.Async(
            c => c.Console(
                LogEventLevel.Verbose,
                _options.ConsoleMessageTemplate,
                theme: AnsiConsoleTheme.Literate,
                formatProvider: CultureInfo.InvariantCulture
            )
        );
    }
}
