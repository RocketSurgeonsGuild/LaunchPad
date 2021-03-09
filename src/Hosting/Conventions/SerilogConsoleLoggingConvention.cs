using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Hosting.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

[assembly: Convention(typeof(SerilogConsoleLoggingConvention))]

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions
{
    /// <summary>
    /// SerilogConsoleLoggingConvention.
    /// Implements the <see cref="ISerilogConvention" />
    /// </summary>
    /// <seealso cref="ISerilogConvention" />
    public sealed class SerilogConsoleLoggingConvention : ISerilogConvention
    {
        private readonly LaunchPadLoggingOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogConsoleLoggingConvention" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public SerilogConsoleLoggingConvention(LaunchPadLoggingOptions? options = null)
            => _options = options ?? new LaunchPadLoggingOptions();

        /// <inheritdoc />
        public void Register(
            [NotNull] IConventionContext context,
            IServiceProvider services,
            IConfiguration configuration,
            LoggerConfiguration loggerConfiguration
        )
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (!_options.EnableConsoleLogging) return;

            loggerConfiguration.WriteTo.Async(c => c.Console(
                LogEventLevel.Verbose,
                _options.ConsoleMessageTemplate,
                theme: AnsiConsoleTheme.Literate
            ));
        }
    }
}