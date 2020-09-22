#if CONVENTIONS
using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog.Conventions;
using Serilog;

[assembly: Convention(typeof(SerilogEnrichLoggingConvention))]

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions
{
    /// <summary>
    /// SerilogEnrichLoggingConvention.
    /// Implements the <see cref="ISerilogConvention" />
    /// </summary>
    /// <seealso cref="ISerilogConvention" />
    public class SerilogEnrichLoggingConvention : ISerilogConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="configuration"></param>
        /// <param name="loggerConfiguration"></param>
        public void Register([NotNull] IConventionContext context, IConfiguration configuration, LoggerConfiguration loggerConfiguration)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            loggerConfiguration.AddLaunchPadEnrichLogging();
        }
    }
}
#endif