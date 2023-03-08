using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Delegate OpenTelemetryTracingConvention
/// </summary>
/// <param name="conventionContext"></param>
/// <param name="configuration"></param>
/// <param name="builder"></param>
public delegate void OpenTelemetryTracingConvention(IConventionContext conventionContext, IConfiguration configuration, TracerProviderBuilder builder);
