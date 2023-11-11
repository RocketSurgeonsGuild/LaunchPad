using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

internal class CustomHealthCheckService(HealthCheckService wrappedService, ValidationHealthCheckResults healthCheckResults)
    : HealthCheckService
{
    public override async Task<HealthReport> CheckHealthAsync(
        Func<HealthCheckRegistration, bool>? predicate, CancellationToken cancellationToken = new CancellationToken()
    )
    {
        var results = await wrappedService.CheckHealthAsync(predicate, cancellationToken);
        return new HealthReport(
            results.Entries.Concat(healthCheckResults.Results).ToDictionary(z => z.Key, z => z.Value),
            results.TotalDuration
        );
    }
}
