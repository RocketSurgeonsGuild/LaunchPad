using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation;

internal class CustomHealthCheckService : HealthCheckService
{
    private readonly HealthCheckService _wrappedService;
    private readonly ValidationHealthCheckResults _healthCheckResults;

    public CustomHealthCheckService(HealthCheckService wrappedService, ValidationHealthCheckResults healthCheckResults)
    {
        _wrappedService = wrappedService;
        _healthCheckResults = healthCheckResults;
    }

    public override async Task<HealthReport> CheckHealthAsync(
        Func<HealthCheckRegistration, bool>? predicate, CancellationToken cancellationToken = new CancellationToken()
    )
    {
        var results = await _wrappedService.CheckHealthAsync(predicate, cancellationToken);
        return new HealthReport(
            results.Entries.Concat(_healthCheckResults.Results).ToDictionary(z => z.Key, z => z.Value),
            results.TotalDuration
        );
    }
}
