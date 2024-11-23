using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Logging;

namespace Rocket.Surgery.LaunchPad.Telemetry.Conventions;

/// <summary>
/// Defines default telemetry convention
/// </summary>
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class TelemetryConvention : ILoggingConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, ILoggingBuilder builder)
    {
        builder.AddOpenTelemetry();
    }
}