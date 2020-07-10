using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Operations.Rockets;

namespace Sample.Worker
{
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
                var items = await _mediator.Invoke(m => m.Send(new ListRockets.Request(), stoppingToken));
                _logger.LogInformation("Items: {@Items}", items);
                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
