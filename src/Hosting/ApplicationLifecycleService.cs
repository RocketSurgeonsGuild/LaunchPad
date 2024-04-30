using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.LaunchPad.Hosting;

internal class ApplicationLifecycleService
    (IServiceProvider serviceProvider, IEnumerable<ApplicationLifecycleRegistration> registrations) : IHostedLifecycleService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(registrations.Where(z => z.Method == nameof(StartingAsync)).Select(z => z.Action(serviceProvider, cancellationToken)));
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(registrations.Where(z => z.Method == nameof(StartedAsync)).Select(z => z.Action(serviceProvider, cancellationToken)));
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(registrations.Where(z => z.Method == nameof(StoppingAsync)).Select(z => z.Action(serviceProvider, cancellationToken)));
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(registrations.Where(z => z.Method == nameof(StoppedAsync)).Select(z => z.Action(serviceProvider, cancellationToken)));
    }
}