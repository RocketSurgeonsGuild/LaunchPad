using Bogus;

using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sample.Core.Domain;

namespace Sample.Core;

/// <summary>
///     Fills the database with generated rockets and launch records when the application starts.
/// </summary>
[LiveJoint]
[DovetailExport]
public class DataGenerationJoint : IServiceJoint
{
    public void Register(IDovetailContext context, IServiceCollection services) =>
        services.Insert(0, ServiceDescriptor.Singleton<IHostedService, HostedService>());

    private class HostedService(IServiceProvider serviceProvider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RocketDbContext>();

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

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
