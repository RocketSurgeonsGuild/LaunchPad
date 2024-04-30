using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.Serilog;
using Rocket.Surgery.LaunchPad.Telemetry;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

[ExportConvention]
class HostingConvention : IServiceConvention, IHostApplicationConvention, IOpenTelemetryConvention
{

    public void Register(IConventionContext context, IHostApplicationBuilder builder)
    {
        if (context.GetOrAdd(() => new LaunchPadLoggingOptions()).WriteToProviders != true)
        {
            var providers = builder.Services.Where(z => z.ServiceType == typeof(ILoggerProvider)).ToArray();
            builder.Logging.ClearProviders();
            builder.OnHostStarting(
                provider => providers.Aggregate(
                    provider.GetRequiredService<ILoggerFactory>(),
                     (factory, descriptor) =>
                    {
                        switch (descriptor)
                        {
                            case { ImplementationFactory: { } method } when method(provider) is ILoggerProvider p:
                                factory.AddProvider(p);
                                break;
                            case { ImplementationInstance: ILoggerProvider instance }:
                                factory.AddProvider(instance);
                                break;
                            case { ImplementationType: { } type } when ActivatorUtilities.CreateInstance(provider, type) is ILoggerProvider instance:
                                factory.AddProvider(instance);
                                break;
                        }

                        return factory;
                    }
                )
            );
        }
    }

    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddHostedService<ApplicationLifecycleService>();
    }

    public void Register(IConventionContext context, IConfiguration configuration, IOpenTelemetryBuilder builder)
    {
        builder.ConfigureResource(
            z => z
                .AddTelemetrySdk()
                .AddService(
                     configuration["OTEL_SERVICE_NAME"] ?? context.Get<IHostEnvironment>()?.ApplicationName ?? "unknown",
                     "Syndicates",
                     Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
                     serviceInstanceId: configuration["WEBSITE_SITE_NAME"]
                 )
        );
    }
}
