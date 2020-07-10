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
    public class AppMetricsConvention : IHostingConvention//, IServiceConvention
    {
        private readonly IConventionScanner _scanner;
        private readonly IAssemblyProvider _assemblyProvider;
        private readonly IAssemblyCandidateFinder _assemblyCandidateFinder;
        private readonly ILogger _diagnosticSource;

        public AppMetricsConvention(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            ILogger diagnosticSource
        )
        {
            _scanner = scanner;
            _assemblyProvider = assemblyProvider;
            _assemblyCandidateFinder = assemblyCandidateFinder;
            _diagnosticSource = diagnosticSource;
        }

        public void Register(IHostingConventionContext context)
        {
            void ConfigureMetrics(HostBuilderContext ctx, IMetricsBuilder metricsBuilder) => new AppMetricsBuilder(
                _scanner,
                _assemblyProvider,
                _assemblyCandidateFinder,
                metricsBuilder,
                ctx.HostingEnvironment,
                ctx.Configuration,
                _diagnosticSource,
                context.Properties
            ).Build();

            IMetricsBuilder metricsBuilder;
            if (context.GetOrAdd(() => new ConventionMetricsOptions()).UseDefaults)
            {
                // throws during unit tests :(
                // context.Builder.ConfigureMetricsWithDefaults(ConfigureMetrics);
                metricsBuilder = global::App.Metrics.AppMetrics.CreateDefaultBuilder();
            }
            else
            {
                // throws during unit tests :(
                // context.Builder.ConfigureMetrics(ConfigureMetrics);
                metricsBuilder = new global::App.Metrics.MetricsBuilder();
            }

            context.Builder.ConfigureServices(
                (ctx, services) =>
                {
                    metricsBuilder.Configuration.ReadFrom(ctx.Configuration);

                    if (metricsBuilder.CanReport())
                    {
                        services.AddMetricsReportingHostedService();
                    }

                    services.AddMetrics(metricsBuilder);
                    ConfigureMetrics(ctx, metricsBuilder);
                }
            );
        }

        //public void Register(IServiceConventionContext context)
        //{
        //    context.Services.AddAppMetricsHealthPublishing();
        //}
    }
}