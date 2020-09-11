using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Hosting.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;
using Serilog.Events;

[assembly: Convention(typeof(SerilogDebugLoggingConvention))]

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions
{
    /// <summary>
    /// SerilogDebugLoggingConvention.
    /// Implements the <see cref="ISerilogConvention" />
    /// </summary>
    /// <seealso cref="ISerilogConvention" />
    public sealed class SerilogDebugLoggingConvention : ISerilogConvention
    {
        private readonly LaunchPadLoggingOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogDebugLoggingConvention" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public SerilogDebugLoggingConvention(LaunchPadLoggingOptions? options = null)
            => _options = options ?? new LaunchPadLoggingOptions();

        /// <inheritdoc />
        public void Register([NotNull] IConventionContext context, IConfiguration configuration, LoggerConfiguration loggerConfiguration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (!_options.EnableDebugLogging) return;

            loggerConfiguration.WriteTo.Async(c => c.Debug(
                LogEventLevel.Verbose,
                _options.DebugMessageTemplate
            ));
        }
    }
}