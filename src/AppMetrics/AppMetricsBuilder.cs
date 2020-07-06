using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using IMetricsBuilder = App.Metrics.IMetricsBuilder;

namespace Rocket.Surgery.LaunchPad.AppMetrics
{
    /// <summary>
    /// Logging Builder
    /// Implements the <see cref="ConventionBuilder{MetricsBuilder,TConvention,TDelegate}" />
    /// Implements the <see cref="IMetricsConvention" />
    /// Implements the <see cref="IMetricsConventionContext" />
    /// </summary>
    /// <seealso cref="ConventionBuilder{MetricsBuilder, IMetricsConvention, MetricsConventionDelegate}" />
    /// <seealso cref="IMetricsConvention" />
    /// <seealso cref="IMetricsConventionContext" />
    [PublicAPI]
    public class AppMetricsBuilder : ConventionBuilder<AppMetricsBuilder, IMetricsConvention, MetricsConventionDelegate>,
                                  IMetricsConventionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppMetrics.AppMetricsBuilder" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="appMetricsBuilder">The metrics builder.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">
        /// environment
        /// or
        /// metricsBuilder
        /// or
        /// configuration
        /// or
        /// diagnosticSource
        /// </exception>
        public AppMetricsBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IMetricsBuilder appMetricsBuilder,
            IHostEnvironment environment,
            IConfiguration configuration,
            ILogger diagnosticSource,
            IDictionary<object, object?> properties
        ) : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            MetricsBuilder = appMetricsBuilder ?? throw new ArgumentNullException(nameof(appMetricsBuilder));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public void Build() => Composer.Register(
            Scanner,
            this,
            typeof(IMetricsConvention),
            typeof(MetricsConventionDelegate)
        );

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the metrics builder.
        /// </summary>
        /// <value>The metrics builder.</value>
        public IMetricsBuilder MetricsBuilder { get; }

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        /// <summary>
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        public IHostEnvironment Environment { get; }
    }
}