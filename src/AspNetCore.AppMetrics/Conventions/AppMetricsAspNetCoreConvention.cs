using App.Metrics.AspNetCore.Endpoints;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore.AppMetrics.Conventions;

[assembly: Convention(typeof(AppMetricsAspNetCoreConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.AppMetrics.Conventions
{
    public class AppMetricsAspNetCoreConvention : IServiceConvention, IHostingConvention
    {
        public void Register(IHostingConventionContext context)
        {
            context.Builder.UseMetricsWebTracking();
        }

        public void Register(IServiceConventionContext context)
        {
            context.Services
               .AddMetricsEndpoints(context.Configuration.GetSection("Metrics:Endpoints"))
               .AddMetricsTrackingMiddleware(context.Configuration.GetSection("Metrics:Tracking"))
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
}