using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.AppMetrics;
using System;

// ReSharper disable once CheckNamespace
namespace App.Metrics
{
    /// <summary>
    /// Extension method to apply configuration conventions
    /// </summary>
    public static class RocketSurgeryAppMetricsExtensions
    {
        /// <summary>
        /// Apply configuration conventions
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <param name="conventionContext"></param>
        /// <returns></returns>
        public static IMetricsBuilder ApplyConventions(this IMetricsBuilder configurationBuilder, IConventionContext conventionContext)
        {
            var configuration = conventionContext.Get<IConfiguration>() ?? throw new ArgumentException("Configuration was not found in context", nameof(conventionContext));
            foreach (var item in conventionContext.Conventions.Get<IMetricsConvention, MetricsConvention>())
            {
                if (item is IMetricsConvention convention)
                {
                    convention.Register(conventionContext, configuration, configurationBuilder);
                }
                else if (item is MetricsConvention @delegate)
                {
                    @delegate(conventionContext, configuration, configurationBuilder);
                }
            }

            return configurationBuilder;
        }
    }
}