using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Telemetry;

namespace Rocket.Surgery.LaunchPad.Hosting.Telemetry;

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
    public static OpenTelemetryBuilder ApplyConventions(this OpenTelemetryBuilder builder, IConventionContext conventionContext)
    {
        var configuration = conventionContext.Get<IConfiguration>()
                         ?? throw new ArgumentException("Configuration was not found in context", nameof(conventionContext));

        builder
           .WithMetrics(z => z.ApplyConventions(conventionContext))
           .WithTracing(z => z.ApplyConventions(conventionContext));

        foreach (var item in conventionContext.Conventions.Get<IOpenTelemetryConvention, OpenTelemetryConvention>())
        {
            if (item is IOpenTelemetryConvention convention)
            {
                convention.Register(conventionContext, configuration, builder);
            }
            else if (item is OpenTelemetryConvention @delegate)
            {
                @delegate(conventionContext, configuration, builder);
            }
        }

        return builder;
    }
}
