using App.Metrics;
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
    public class AppMetricsConvention : IHostingConvention, IServiceConvention
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

            if (context.GetOrAdd(() => new ConventionMetricsOptions()).UseDefaults)
            {
                context.Builder.ConfigureMetricsWithDefaults(ConfigureMetrics);
            }
            else
            {
                context.Builder.ConfigureMetrics(ConfigureMetrics);
            }
        }

        public void Register(IServiceConventionContext context)
        {
            context.Services.AddAppMetricsHealthPublishing();
        }
    }
}