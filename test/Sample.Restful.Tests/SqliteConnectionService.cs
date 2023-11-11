using Microsoft.Extensions.Hosting;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;

namespace Sample.Restful.Tests;

internal class SqliteConnectionService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await serviceProvider.WithScoped<RocketDbContext>()
                              .Invoke(z => z.Database.EnsureCreatedAsync(cancellationToken))
                              .ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
