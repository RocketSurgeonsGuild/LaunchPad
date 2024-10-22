using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Resources;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Telemetry;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

[ExportConvention]
[AfterConvention<SerilogHostingConvention>]
[ConventionCategory(ConventionCategory.Core)]
internal class DefaultTelemetryConvention : IOpenTelemetryConvention
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
}