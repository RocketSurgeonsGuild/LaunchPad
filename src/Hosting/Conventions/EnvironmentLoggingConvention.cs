using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Hosting.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;
using System;

[assembly: Convention(typeof(EnvironmentLoggingConvention))]

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions
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

            var environment = context.Get<IHostEnvironment>()!;
            loggerConfiguration.Enrich.WithProperty(nameof(environment.EnvironmentName), environment.EnvironmentName);
            loggerConfiguration.Enrich.WithProperty(nameof(environment.ApplicationName), environment.ApplicationName);
        }
    }
}