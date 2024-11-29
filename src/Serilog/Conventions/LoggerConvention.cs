using App.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions;

/// <summary>
///     SerilogHostingConvention.
///     Implements the <see cref="IHostCreatedConvention{THost}" />
/// </summary>
/// <seealso cref="IHostCreatedConvention&lt;IHost&gt;" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class LoggerConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        // removes default console loggers and such
        foreach (var item in services
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
            services.Remove(item);
        }

        #pragma warning disable CA2000
        var loggerProviderCollection = new LoggerProviderCollection();
        #pragma warning restore CA2000
        var loggerConfiguration = new LoggerConfiguration();

        if (context.Get<ILogger>() is { } logger) loggerConfiguration.WriteTo.Logger(logger);
        services.AddSingleton(
            serviceProvider =>
            {
                loggerConfiguration.ApplyConventions(context, configuration, serviceProvider);
                return loggerConfiguration.CreateLogger();
            }
        );
        services.AddSingleton<ILogger>(serviceProvider => serviceProvider.GetRequiredService<Logger>());
        services.AddSingleton<ILoggerFactory>(
            serviceProvider => new SerilogLoggerFactory(
                serviceProvider.GetRequiredService<Logger>(),
                true,
                loggerProviderCollection
            )
        );

        services.AddSingleton<LoggerProviderCollection>(
            serviceProvider =>
            {
                var loggerProviders = serviceProvider.GetServices<ILoggerProvider>();
                return loggerProviders.Aggregate(
                    loggerProviderCollection,
                    (factory, loggerProvider) =>
                    {
                        factory.AddProvider(loggerProvider);
                        return factory;
                    }
                );
            }
        );
    }
}