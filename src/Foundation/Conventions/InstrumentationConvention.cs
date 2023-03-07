using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Telemetry;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     InstrumentationConvention.
///     Implements <see cref="IOpenTelemetryConvention" /> and <see cref="IOpenTelemetryConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
public class InstrumentationConvention : IOpenTelemetryConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext conventionContext, IConfiguration configuration, OpenTelemetryBuilder builder)
    {
        builder
           .WithMetrics(z => z.AddHttpClientInstrumentation())
           .WithTracing(z => z.AddHttpClientInstrumentation(x => x.RecordException = true));
    }
}
