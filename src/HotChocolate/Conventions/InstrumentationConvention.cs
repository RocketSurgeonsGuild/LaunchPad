using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Telemetry;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Conventions;

/// <summary>
///     InstrumentationConvention.
///     Implements <see cref="IOpenTelemetryConvention" /> and <see cref="IOpenTelemetryConvention" />
/// </summary>
/// <seealso cref="IOpenTelemetryConvention" />
[PublicAPI]
[ExportConvention]
public class InstrumentationConvention : IOpenTelemetryConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext conventionContext, IConfiguration configuration, IOpenTelemetryBuilder builder)
    {
        builder.WithTracing(b => b.AddHotChocolateInstrumentation());
    }
}
