using System.Globalization;

using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Rocket.Surgery.LaunchPad.Serilog.Conventions;

using Serilog;

namespace Rocket.Surgery.LaunchPad.WebAssembly.Hosting.Conventions;

/// <summary>
///     SerilogConsoleLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
/// <remarks>
///     Initializes a new instance of the <see cref="SerilogConsoleLoggingConvention" /> class.
/// </remarks>
/// <param name="options">The options.</param>
[PublicAPI]
[ExportConvention]
[AfterConvention<LoggerConvention>]
[ConventionCategory(ConventionCategory.Core)]
public sealed class SerilogConsoleLoggingConvention(LaunchPadLoggingOptions? options = null) : ISerilogConvention
{
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

        loggerConfiguration.WriteTo.BrowserConsole(formatProvider: CultureInfo.InvariantCulture);
    }

    private readonly LaunchPadLoggingOptions _options = options ?? new LaunchPadLoggingOptions();
}
