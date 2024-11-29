using Microsoft.Extensions.Options;

internal class CustomHostedService(IOptions<CustomHostedServiceOptions> options) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ReSharper disable once UnusedVariable
        var v = options.Value.A;
        return Task.CompletedTask;
    }
}