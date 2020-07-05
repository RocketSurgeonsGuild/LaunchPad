using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Models;
using System.Net.Http.Json;

namespace Sample.Restful.Tests
{
    class SqliteConnectionService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SqliteConnectionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _serviceProvider.WithScoped<RocketDbContext>()
               .Invoke(z => z.Database.EnsureCreatedAsync(cancellationToken))
               .ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}