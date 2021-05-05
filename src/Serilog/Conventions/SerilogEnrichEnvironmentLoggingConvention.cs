using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog.Conventions;
using Serilog;
using System;

[assembly: Convention(typeof(SerilogEnrichLoggingConvention))]

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions
{
    /// <summary>
    /// SerilogEnrichLoggingConvention.
    /// Implements the <see cref="ISerilogConvention" />
    /// </summary>
    /// <seealso cref="ISerilogConvention" />
    public class SerilogEnrichEnvironmentLoggingConvention : ISerilogConvention
    {
        /// <summary>
        /// Registers the specified context.
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
               .Enrich.WithEnvironmentUserName()
               .Enrich.WithMachineName()
               .Enrich.WithProcessId()
               .Enrich.WithProcessName()
               .Enrich.WithThreadId();
        }
    }
}