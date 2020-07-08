using System;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog.Conventions;

[assembly: Convention(typeof(EnvironmentLoggingConvention))]

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions
{
    /// <summary>
    /// EnvironmentLoggingConvention.
    /// Implements the <see cref="ISerilogConvention" />
    /// </summary>
    /// <seealso cref="ISerilogConvention" />
    public class EnvironmentLoggingConvention : ISerilogConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register([NotNull] ISerilogConventionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var environment = context.Environment;
            context.LoggerConfiguration.Enrich.WithProperty(
                nameof(environment.EnvironmentName),
                environment.EnvironmentName
            );
            context.LoggerConfiguration.Enrich.WithProperty(
                nameof(environment.ApplicationName),
                environment.ApplicationName
            );
        }
    }
}