using App.Metrics.AspNetCore.Endpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.AppMetrics.Conventions;

/// <summary>
///     Convention for activating App Metrics with Asp.Net Core
/// </summary>
[PublicAPI]
[ExportConvention]
public class AppMetricsAspNetCoreConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services
           .AddMetricsEndpoints(configuration.GetSection("Metrics:Endpoints"))
           .AddMetricsTrackingMiddleware(configuration.GetSection("Metrics:Tracking"))
           .AddMetricsReportingHostedService()
           .Configure<MetricsEndpointsHostingOptions>(
                options =>
                {
                    options.MetricsEndpoint = "/";
                    options.MetricsTextEndpoint = "/text";
                    options.EnvironmentInfoEndpoint = "/info";
                }
            );
    }
}
