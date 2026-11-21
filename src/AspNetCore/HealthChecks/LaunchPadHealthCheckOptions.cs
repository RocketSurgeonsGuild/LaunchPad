using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Optionary;

namespace Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

/// <summary>
///     Where LaunchPad maps the health check endpoints (<c>MapLaunchPadHealthChecks</c>), and on which port.
/// </summary>
/// <remarks>
///     Optionary binds these options from the <see cref="SectionName" /> configuration section without reflection, for example
///     <c>LaunchPad__HealthChecks__Port=8081</c>, and describes them in the application's <c>appsettings.schema.json</c>. The
///     <see cref="Joints.HealthChecksJoint" /> registers them, and checks them when the host starts. Code can change them with
///     <c>services.Configure&lt;LaunchPadHealthCheckOptions&gt;</c>.
/// </remarks>
[PublicAPI]
[Optionary(SectionName, ValidateOnStart = true)]
public sealed class LaunchPadHealthCheckOptions
{
    /// <summary>The configuration section the options are bound from.</summary>
    public const string SectionName = "LaunchPad:HealthChecks";

    /// <summary>
    ///     The port the health check endpoints are served on, alone. When it is <see langword="null" /> (the default) they are served on the
    ///     application's own addresses.
    /// </summary>
    /// <remarks>
    ///     When a port is set, Kestrel also listens on <c>http://*:{Port}</c>, next to the application's own addresses. A request on that port
    ///     for anything but the health check endpoints gets a 404, and so does a request for the health check endpoints on any other port. When
    ///     the server doesn't listen on the port after all (a <c>Listen</c> in code, IIS, HTTP.sys or the in-memory test server), the health
    ///     check endpoints answer on the application's own addresses, as they do with no port.
    /// </remarks>
    [Range(1, 65535)]
    public int? Port { get; set; }

    /// <summary>The path of the health report, which the liveness and readiness paths sit under. The default is <c>/health</c>.</summary>
    public string Path { get; set; } = "/health";

    /// <summary>The liveness path, under <see cref="Path" />. The default is <c>live</c>.</summary>
    public string LivePath { get; set; } = "live";

    /// <summary>The readiness path, under <see cref="Path" />. The default is <c>ready</c>.</summary>
    public string ReadyPath { get; set; } = "ready";

    /// <summary>
    ///     The status code each endpoint answers for each <see cref="HealthStatus" />. The defaults are 200 for <see cref="HealthStatus.Healthy" />
    ///     and <see cref="HealthStatus.Degraded" />, and 503 for <see cref="HealthStatus.Unhealthy" />.
    /// </summary>
    public IDictionary<HealthStatus, int> ResultStatusCodes { get; } = new Dictionary<HealthStatus, int>
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    };
}
