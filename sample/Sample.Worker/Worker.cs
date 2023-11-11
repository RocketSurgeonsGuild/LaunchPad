using MediatR;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Operations.Rockets;

namespace Sample.Worker;

public class BackgroundWorker(ILogger<BackgroundWorker> logger, IExecuteScoped<IMediator> mediator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var items = await mediator.Invoke(m => m.CreateStream(new ListRockets.Request(null), stoppingToken))
                                       .ToListAsync(stoppingToken);
            logger.LogInformation("Items: {@Items}", items);
            await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
        }
    }
}
