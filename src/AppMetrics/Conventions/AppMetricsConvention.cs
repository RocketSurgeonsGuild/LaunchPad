using App.Metrics;
using App.Metrics.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AppMetrics.Conventions;

[assembly: Convention(typeof(AppMetricsConvention))]

namespace Rocket.Surgery.LaunchPad.AppMetrics.Conventions
{
    public class AppMetricsConvention : IHostingConvention
    {
        public void Register(IConventionContext context, IHostBuilder builder)
        {
            IMetricsBuilder metricsBuilder;
            metricsBuilder = context.GetOrAdd(() => new ConventionMetricsOptions()).UseDefaults
                ? global::App.Metrics.AppMetrics.CreateDefaultBuilder()
                : new MetricsBuilder();

            builder.ConfigureServices(
                (ctx, services) =>
                {
                    metricsBuilder.Configuration.ReadFrom(ctx.Configuration);

                    if (metricsBuilder.CanReport())
                    {
                        services.AddMetricsReportingHostedService();
                    }

                    services.AddMetrics(metricsBuilder);
                    metricsBuilder.ApplyConventions(context);
                }
            );
        }
    }
}