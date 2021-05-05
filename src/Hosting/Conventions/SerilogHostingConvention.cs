using App.Metrics;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.Hosting.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;
using System;
using System.Linq;
using ILogger = Serilog.ILogger;

[assembly: Convention(typeof(SerilogHostingConvention))]

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions
{
    /// <summary>
    /// SerilogHostingConvention.
    /// Implements the <see cref="IHostingConvention" />
    /// </summary>
    /// <seealso cref="IHostingConvention" />
    public class SerilogHostingConvention : IHostingConvention
    {
        private readonly LaunchPadLoggingOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogHostingConvention" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public SerilogHostingConvention(LaunchPadLoggingOptions? options = null)
        {
            _options = options ?? new LaunchPadLoggingOptions();
        }

        /// <inheritdoc />
        public void Register([NotNull] IConventionContext context, IHostBuilder builder)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            builder.ConfigureServices(
                (context, services) =>
                {
                    // removes default console loggers and such
                    foreach (var item in services
                       .Where(
                            x => x.ImplementationType?.FullName.StartsWith("Microsoft.Extensions.Logging", StringComparison.Ordinal) == true
                             && x.ImplementationType?.FullName.EndsWith("Provider", StringComparison.Ordinal) == true
                        )
                       .ToArray()
                    )
                    {
                        services.Remove(item);
                    }
                }
            );

            if (context.Get<ILogger>() is { } logger)
            {
                builder.UseSerilog(logger);
            }
            else
            {
                builder.UseSerilog(
                    (ctx, services, loggerConfiguration) => loggerConfiguration.ApplyConventions(context, services),
                    _options.PreserveStaticLogger,
                    _options.WriteToProviders
                );

                if (context.Get<ILoggerFactory>() != null)
                {
                    builder.ConfigureServices((_, services) => services.AddSingleton(context.Get<ILoggerFactory>()));
                }
            }
        }
    }
}