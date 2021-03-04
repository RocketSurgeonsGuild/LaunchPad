using App.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Functions.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Linq;
using ILogger = Serilog.ILogger;

[assembly: Convention(typeof(SerilogFunctionsConvention))]

namespace Rocket.Surgery.LaunchPad.Functions.Conventions
{
    /// <summary>
    /// SerilogHostingConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    public class SerilogFunctionsConvention : IServiceConvention
    {
        private readonly LaunchPadLoggingOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogFunctionsConvention" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public SerilogFunctionsConvention(LaunchPadLoggingOptions? options = null)
        {
            _options = options ?? new LaunchPadLoggingOptions();
        }

        /// <inheritdoc />
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // removes default console loggers and such
            foreach (var item in services
               .Where(
                    x => x.ImplementationType?.FullName?.StartsWith("Microsoft.Extensions.Logging", StringComparison.Ordinal) == true
                     && x.ImplementationType?.FullName?.EndsWith("Provider", StringComparison.Ordinal) == true
                )
               .ToArray()
            )
            {
                services.Remove(item);
            }

            LoggerProviderCollection loggerProviders = null;
            if (_options.WriteToProviders)
            {
                loggerProviders = new LoggerProviderCollection();
            }

            services.AddSingleton<ILoggerFactory>(
                _ =>
                {
                    var logger = _.GetRequiredService<ILogger>();
                    ILogger registeredLogger = null;
                    if (_options.PreserveStaticLogger)
                    {
                        registeredLogger = logger;
                    }
                    else
                    {
                        // Passing a `null` logger to `SerilogLoggerFactory` results in disposal via
                        // `Log.CloseAndFlush()`, which additionally replaces the static logger with a no-op.
                        Log.Logger = logger;
                    }

                    var factory = new SerilogLoggerFactory(registeredLogger, true, loggerProviders);

                    if (_options.WriteToProviders)
                    {
                        foreach (var provider in _.GetServices<ILoggerProvider>())
                            factory.AddProvider(provider);
                    }

                    return factory;
                }
            );

            if (context.Get<ILogger>() is { } logger)
            {
                services.AddSingleton(logger);
            }
            else
            {
                services.AddSingleton(
                    _ =>
                    {
                        var loggerConfiguration = new LoggerConfiguration();

                        if (loggerProviders != null)
                            loggerConfiguration.WriteTo.Providers(loggerProviders);

                        loggerConfiguration.ApplyConventions(context);

                        return loggerConfiguration.CreateLogger();
                    }
                );

                if (context.Get<ILoggerFactory>() != null)
                {
                    services.AddSingleton(context.Get<ILoggerFactory>());
                }
            }
        }
    }
}