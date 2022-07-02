using App.Metrics;
using App.Metrics.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.LaunchPad.AppMetrics.Conventions;

/// <summary>
///     Convention for activating app metrics
/// </summary>
[PublicAPI]
[ExportConvention]
public class AppMetricsConvention : IHostingConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IHostBuilder builder)
    {
        IMetricsBuilder metricsBuilder;
        metricsBuilder = context.GetOrAdd(() => new ConventionMetricsOptions()).UseDefaults
            ? App.Metrics.AppMetrics.CreateDefaultBuilder()
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
