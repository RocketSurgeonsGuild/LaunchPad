using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.LaunchPad.Serilog;
using Rocket.Surgery.LaunchPad.Serilog.Conventions;
using Serilog;
using Serilog.Configuration;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// SerilogHostBuilderExtensions.
    /// </summary>
    [PublicAPI]
    public static class SerilogHostBuilderExtensions
    {
        /// <summary>
        /// Uses the serilog.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="options">The options.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static ConventionContextBuilder UseSerilog([NotNull] this ConventionContextBuilder container, LaunchPadLoggingOptions? options = null)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Properties.Set(options ?? new LaunchPadLoggingOptions());
            container.PrependConvention<SerilogReadFromConfigurationConvention>();
            container.PrependConvention<SerilogEnrichLoggingConvention>();
            container.PrependConvention<SerilogConsoleLoggingConvention>();
            container.PrependConvention<SerilogDebugLoggingConvention>();
            return container;
        }

        /// <summary>
        /// Write to the log an async sink when running the default command (or web server / hosted process).
        /// Write to a sync sink when not running the default command.
        /// </summary>
        /// <param name="loggerConfiguration">The context.</param>
        /// <param name="configuration"></param>
        /// <param name="register">The action to register the sink.</param>
        public static LoggerConfiguration WriteToAsyncConditionally(
            [NotNull] this LoggerConfiguration loggerConfiguration,
            [NotNull] IConfiguration configuration,
            [NotNull] Action<LoggerSinkConfiguration> register
        )
        {
            if (register == null)
            {
                throw new ArgumentNullException(nameof(register));
            }

            if (ConfigurationAsyncHelper.IsAsync(configuration))
            {
                loggerConfiguration.WriteTo.Async(register);
            }
            else
            {
                register(loggerConfiguration.WriteTo);
            }

            return loggerConfiguration;
        }
    }
}