using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

[ExportConvention, AfterConvention<SerilogHostingConvention>]
internal class HostingConvention : IServiceConvention, IOpenTelemetryConvention
{
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

    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddHostedService<ApplicationLifecycleService>();
    }
}
