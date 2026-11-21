using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     Maps LaunchPad's health check endpoints.
/// </summary>
[PublicAPI]
public static class LaunchPadHealthCheckExtensions
{
    /// <summary>
    ///     Maps the health report, liveness and readiness endpoints where <see cref="LaunchPadHealthCheckOptions" /> says.
    /// </summary>
    /// <remarks>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Path (by default)</term>
    ///             <description>What it answers</description>
    ///         </listheader>
    ///         <item>
    ///             <term><c>/health</c></term>
    ///             <description>
    ///                 Runs every health check and writes the report as JSON for the HealthChecks.UI dashboard
    ///                 (<see cref="LaunchPadHealthCheckResponseWriter" />).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term><c>/health/live</c></term>
    ///             <description>Runs no health check: it answers <c>Healthy</c> as long as the application is up. Use it for a liveness probe.</description>
    ///         </item>
    ///         <item>
    ///             <term><c>/health/ready</c></term>
    ///             <description>Runs every health check and answers their status as plain text. Use it for a readiness probe.</description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Every endpoint allows anonymous requests and answers with <see cref="LaunchPadHealthCheckOptions.ResultStatusCodes" />. The
    ///         returned builder applies its conventions to all three, so <c>RequireAuthorization</c> on it secures them all. The health check
    ///         services must be registered (<c>AddHealthChecks</c>), which LaunchPad's <c>HealthChecksJoint</c> does.
    ///     </para>
    /// </remarks>
    /// <param name="endpoints">The endpoints to map them on.</param>
    /// <returns>A builder for the three endpoints.</returns>
    public static IEndpointConventionBuilder MapLaunchPadHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var options = endpoints.ServiceProvider.GetService<IOptions<LaunchPadHealthCheckOptions>>()?.Value ?? new LaunchPadHealthCheckOptions();
        var group = endpoints.MapGroup(options.Path);

        group.MapHealthChecks("", Create(options, LaunchPadHealthCheckResponseWriter.WriteAsync));
        group.MapHealthChecks(options.LivePath, Create(options, predicate: static _ => false));
        group.MapHealthChecks(options.ReadyPath, Create(options));

        return group.AllowAnonymous();
    }

    private static HealthCheckOptions Create(
        LaunchPadHealthCheckOptions options,
        Func<HttpContext, HealthReport, Task>? responseWriter = null,
        Func<HealthCheckRegistration, bool>? predicate = null
    )
    {
        var healthCheckOptions = new HealthCheckOptions
        {
            Predicate = predicate,
            ResultStatusCodes = new Dictionary<HealthStatus, int>(options.ResultStatusCodes),
        };
        if (responseWriter is not null) healthCheckOptions.ResponseWriter = responseWriter;
        return healthCheckOptions;
    }
}
