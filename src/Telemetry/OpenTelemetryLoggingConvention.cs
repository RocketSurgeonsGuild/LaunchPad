using Microsoft.Extensions.Configuration;

using OpenTelemetry.Logs;

using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Delegate OpenTelemetryLoggingConvention
/// </summary>
/// <param name="context"></param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
[PublicAPI]
public delegate void OpenTelemetryLoggingConvention(IConventionContext context, IConfiguration configuration, LoggerProviderBuilder builder);
