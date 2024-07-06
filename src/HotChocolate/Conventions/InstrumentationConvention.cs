using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Telemetry;
using IConventionContext = Rocket.Surgery.Conventions.IConventionContext;

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
    public void Register(IConventionContext context, IConfiguration configuration, IOpenTelemetryBuilder builder)
    {
        builder.WithTracing(b => b.AddHotChocolateInstrumentation());
    }
}
