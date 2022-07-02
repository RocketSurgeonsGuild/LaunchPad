using App.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.LaunchPad.Functions.Conventions;

/// <summary>
///     SerilogHostingConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
public class SerilogFunctionsConvention : IServiceConvention
{
    private readonly LaunchPadLoggingOptions _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SerilogFunctionsConvention" /> class.
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

        LoggerProviderCollection? loggerProviders = null;
        if (_options.WriteToProviders)
        {
#pragma warning disable CA2000
            loggerProviders = new LoggerProviderCollection();
#pragma warning restore CA2000
        }

        services.AddSingleton<ILoggerFactory>(
            _ =>
            {
                var log = _.GetRequiredService<ILogger>();
                ILogger? registeredLogger = null;
                if (_options.PreserveStaticLogger)
                {
                    registeredLogger = log;
                }
                else
                {
                    // Passing a `null` logger to `SerilogLoggerFactory` results in disposal via
                    // `Log.CloseAndFlush()`, which additionally replaces the static logger with a no-op.
                    Log.Logger = log;
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

                    loggerConfiguration.ApplyConventions(context, _);

                    return loggerConfiguration.CreateLogger();
                }
            );

            if (context.Get<ILoggerFactory>() is { } loggerFactory)
            {
                services.AddSingleton(loggerFactory);
            }
        }
    }
}
