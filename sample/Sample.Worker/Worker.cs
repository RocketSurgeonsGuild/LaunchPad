using MediatR;

using Sample.Core.Operations.Rockets;

namespace Sample.Worker;

public class BackgroundWorker(ILogger<BackgroundWorker> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using (var scope = scopeFactory.CreateAsyncScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var items = await mediator.CreateStream(new ListRockets.Request(null), stoppingToken).ToListAsync(stoppingToken);
                logger.LogInformation("Items: {@Items}", items);
            }

            await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
        }
    }
}
