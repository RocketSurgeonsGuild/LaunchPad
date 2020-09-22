using Microsoft.Extensions.Configuration;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Serilog
{
    /// <summary>
    /// Extensions used to pull in the default services for launchpad
    /// </summary>
#if CONVENTIONS
    internal
#else
    public
#endif
        static class AddLaunchPadSerilogExtension
    {
        /// <summary>
        /// Adds the launchpad logging
        /// </summary>
        /// <param name="loggerConfiguration"></param>
        /// <returns></returns>
        public static LoggerConfiguration AddLaunchPadLogging(this LoggerConfiguration loggerConfiguration) => loggerConfiguration
           .Enrich.WithEnvironmentUserName()
           .Enrich.WithMachineName()
           .Enrich.WithProcessId()
           .Enrich.WithProcessName()
           .Enrich.WithThreadId();

        /// <summary>
        /// Enriches the launchpad logging
        /// </summary>
        /// <param name="loggerConfiguration"></param>
        /// <returns></returns>
        public static LoggerConfiguration AddLaunchPadEnrichLogging(this LoggerConfiguration loggerConfiguration) => loggerConfiguration
           .Enrich.FromLogContext()
           .Enrich.WithDemystifiedStackTraces();

        /// <summary>
        /// Enriches the launchpad logging
        /// </summary>
        /// <param name="loggerConfiguration"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration AddLaunchPadLoggingConfiguration(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
            => loggerConfiguration.ReadFrom.Configuration(configuration);
    }
}