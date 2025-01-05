using Microsoft.Extensions.Configuration;

using OpenTelemetry.Resources;

using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Delegate OpenTelemetryResourceConvention
/// </summary>
/// <param name="context"></param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
[PublicAPI]
public delegate void OpenTelemetryResourceConvention(IConventionContext context, IConfiguration configuration, ResourceBuilder builder);
