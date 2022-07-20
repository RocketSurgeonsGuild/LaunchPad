using Microsoft.Extensions.Configuration;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Extension method to apply configuration conventions
/// </summary>
[PublicAPI]
public static class RocketSurgeryOpenTelemetryExtensions
{
    /// <summary>
    ///     Apply configuration conventions
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="conventionContext"></param>
    /// <returns></returns>
    public static MeterProviderBuilder ApplyConventions(this MeterProviderBuilder builder, IConventionContext conventionContext)
    {
        var configuration = conventionContext.Get<IConfiguration>()
                         ?? throw new ArgumentException("Configuration was not found in context", nameof(conventionContext));
        foreach (var item in conventionContext.Conventions.Get<IOpenTelemetryMetricsConvention, OpenTelemetryMetricsConvention>())
        {
            if (item is IOpenTelemetryMetricsConvention convention)
            {
                convention.Register(conventionContext, configuration, builder);
            }
            else if (item is OpenTelemetryMetricsConvention @delegate)
            {
                @delegate(conventionContext, configuration, builder);
            }
        }

        return builder;
    }

    /// <summary>
    ///     Apply configuration conventions
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="conventionContext"></param>
    /// <returns></returns>
    public static TracerProviderBuilder ApplyConventions(this TracerProviderBuilder builder, IConventionContext conventionContext)
    {
        var configuration = conventionContext.Get<IConfiguration>()
                         ?? throw new ArgumentException("Configuration was not found in context", nameof(conventionContext));
        foreach (var item in conventionContext.Conventions.Get<IOpenTelemetryTracingConvention, OpenTelemetryTracingConvention>())
        {
            if (item is IOpenTelemetryTracingConvention convention)
            {
                convention.Register(conventionContext, configuration, builder);
            }
            else if (item is OpenTelemetryTracingConvention @delegate)
            {
                @delegate(conventionContext, configuration, builder);
            }
        }

        return builder;
    }
}
