using MediatR;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Operations.Rockets;

namespace Sample.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IExecuteScoped<IMediator> _mediator;

    public Worker(ILogger<Worker> logger, IExecuteScoped<IMediator> mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var items = await _mediator.Invoke(m => m.CreateStream(new ListRockets.Request(null), stoppingToken))
                                       .ToListAsync(stoppingToken);
            _logger.LogInformation("Items: {@Items}", items);
            await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
        }
    }
}
