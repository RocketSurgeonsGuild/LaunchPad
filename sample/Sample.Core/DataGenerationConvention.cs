﻿using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;

namespace Sample.Core;

[LiveConvention]
[ExportConvention]
public class DataGenerationConvention : IServiceConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.Insert(0, ServiceDescriptor.Singleton<IHostedService, HostedService>());
    }

    private class HostedService(IServiceProvider serviceProvider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await serviceProvider.WithScoped<RocketDbContext>().Invoke(
                async dbContext =>
                {
                    await dbContext.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
                    var rocketFaker = new RocketFaker();
                    rocketFaker.UseSeed(0);
                    var rockets = rocketFaker.GenerateBetween(10, 100);
                    var launchFaker = new LaunchRecordFaker(rockets);
                    launchFaker.UseSeed(1);
                    var launches = launchFaker.GenerateBetween(100, 1000);

                    await dbContext.Rockets.AddRangeAsync(rockets, cancellationToken).ConfigureAwait(false);
                    await dbContext.LaunchRecords.AddRangeAsync(launches, cancellationToken).ConfigureAwait(false);
                    await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
            ).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
