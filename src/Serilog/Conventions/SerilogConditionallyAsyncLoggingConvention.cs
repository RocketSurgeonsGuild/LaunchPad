using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Serilog;
using Serilog.Configuration;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions
{
    /// <summary>
    /// SerilogConditionallyAsyncLoggingConvention.
    /// Implements the <see cref="ISerilogConvention" />
    /// </summary>
    /// <seealso cref="ISerilogConvention" />
    public abstract class SerilogConditionallyAsyncLoggingConvention : ISerilogConvention
    {
        /// <summary>
        /// Registers the sink synchronously or asynchronously
        /// </summary>
        /// <param name="configuration">The sink configuration.</param>
        protected abstract void Register(LoggerSinkConfiguration configuration);

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

            loggerConfiguration.WriteToAsyncConditionally(configuration, Register);
        }
    }
}