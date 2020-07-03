using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;

[assembly: Convention(typeof(DataGenerationConvention))]
[assembly: Convention(typeof(DataConvention))]

namespace Sample.Core
{
    public class DataGenerationConvention : IServiceConvention
    {

        public void Register(IServiceConventionContext context)
        {
            context.Services.AddHostedService<HostedService>();
        }

        class HostedService : IHostedService
        {
            private readonly IServiceProvider _serviceProvider;

            public HostedService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

            public async Task StartAsync(CancellationToken cancellationToken)
            {
                await _serviceProvider.WithScoped<RocketDbContext>().Invoke(
                    async dbContext =>
                    {
                        await dbContext.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
                        // await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
                    }
                ).ConfigureAwait(false);
                await _serviceProvider.WithScoped<RocketDbContext>().Invoke(
                    async dbContext =>
                    {
                        var rocketFaker = new RocketFaker();
                        var rockets = rocketFaker.GenerateBetween(10, 100);
                        var launchFaker = new LaunchRecordFaker(rockets);

                        var launches = launchFaker.GenerateBetween(100, 1000);

                        await dbContext.Rockets.AddRangeAsync(rockets, cancellationToken).ConfigureAwait(false);
                        await dbContext.LaunchRecords.AddRangeAsync(launches, cancellationToken).ConfigureAwait(false);
                        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    }
                ).ConfigureAwait(false);
            }

            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        }
    }

    class DataConvention : IServiceConvention
    {
        public void Register(IServiceConventionContext context)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            context.Services
               .AddDbContext<RocketDbContext>(x => x
                   .EnableDetailedErrors()
                   .EnableSensitiveDataLogging()
                   .EnableServiceProviderCaching()
                   .UseSqlite(connection)
                );
        }
    }
}