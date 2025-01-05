using System.Globalization;

using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Rocket.Surgery.LaunchPad.Serilog.Conventions;

using Serilog;
using Serilog.Events;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

/// <summary>
///     SerilogDebugLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
/// <remarks>
///     Initializes a new instance of the <see cref="SerilogDebugLoggingConvention" /> class.
/// </remarks>
/// <param name="options">The options.</param>
[PublicAPI]
[ExportConvention]
[AfterConvention<LoggerConvention>]
[ConventionCategory(ConventionCategory.Core)]
public sealed class SerilogDebugLoggingConvention(LaunchPadLoggingOptions? options = null) : ISerilogConvention
{
    private readonly LaunchPadLoggingOptions _options = options ?? new LaunchPadLoggingOptions();

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
