using App.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Hosting;
using Rocket.Surgery.Conventions.Logging;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

/// <summary>
///     SerilogHostingConvention.
///     Implements the <see cref="IHostApplicationConvention" />
/// </summary>
/// <seealso cref="IHostApplicationConvention" />
[PublicAPI]
[ExportConvention]
public class SerilogHostingConvention : IHostApplicationConvention, IHostCreatedConvention<IHost>
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(context);

        // removes default console loggers and such
        foreach (var item in builder
                            .Services
                            .Where(
                                 x =>
                                 {
                                     var type = x.IsKeyedService ? x.KeyedImplementationType : x.ImplementationType;
                                     return type?.FullName?.StartsWith("Microsoft.Extensions.Logging", StringComparison.Ordinal) == true
                                      && type.FullName.EndsWith("Provider", StringComparison.Ordinal);
                                 }
                             )
                            .ToArray()
                )
        {
            builder.Services.Remove(item);
        }

        if (context.Get<ILogger>() is { } logger)
        {
            builder.Services.AddSerilog(logger);
        }
        else
        {
            CustomAddSerilog(
                builder.Services,
                (services, loggerConfiguration) => loggerConfiguration.ApplyConventions(context, builder.Configuration, services)
            );
        }

        if (context.Get<ILoggerFactory>() != null)
            // ReSharper disable once NullableWarningSuppressionIsUsed
            builder.Services.AddSingleton(context.Get<ILoggerFactory>()!);
    }

    private void CustomAddSerilog(
        IServiceCollection collection,
        Action<IServiceProvider, LoggerConfiguration> configureLogger
    )
    {
        collection.AddSingleton(new LoggerProviderCollection());
        collection.AddSingleton(
            services =>
            {
                var loggerConfiguration = new LoggerConfiguration();
                loggerConfiguration.WriteTo.Providers(services.GetRequiredService<LoggerProviderCollection>());
                configureLogger(services, loggerConfiguration);
                return loggerConfiguration.CreateLogger();
            }
        );
        collection.AddSingleton<ILogger>(services => services.GetRequiredService<Logger>().ForContext(new NullEnricher()));
        collection.AddSingleton<ILoggerFactory>(
            services => new SerilogLoggerFactory(
                services.GetRequiredService<Logger>(),
                true,
                services.GetRequiredService<LoggerProviderCollection>()
            )
        );
        collection.AddSingleton(services => new DiagnosticContext(services.GetRequiredService<Logger>()));
        collection.AddSingleton<IDiagnosticContext>(services => services.GetRequiredService<DiagnosticContext>());
    }

    private class NullEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) { }
    }

    /// <inheritdoc />
    public void Register(IConventionContext context, IHost host) => host
                                                                   .Services
                                                                   .GetServices<ILoggerProvider>()
                                                                   .Aggregate(
                                                                        host.Services.GetRequiredService<LoggerProviderCollection>(),
                                                                        (factory, loggerProvider) =>
                                                                        {
                                                                            factory.AddProvider(loggerProvider);
                                                                            return factory;
                                                                        }
                                                                    );
}
