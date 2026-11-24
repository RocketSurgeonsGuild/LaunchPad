using Joinery.Dovetail;
using Joinery.Dovetail.Joints;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Rocket.Surgery.LaunchPad.Primitives.Generated;

using Sample.Core.Domain;

namespace Sample.Core.Tests;

public class DataGenerationJointTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Should_Seed_The_Database_When_The_Live_Host_Starts()
    {
        await using var provider = await CreateLiveProvider();

        await StartHostedServices(provider);

        await using var scope = provider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RocketDbContext>();
        await ( await dbContext.Rockets.CountAsync(TestContext.CancellationToken) ).Should().BeGreaterThanOrEqualTo(10);
        await ( await dbContext.LaunchRecords.CountAsync(TestContext.CancellationToken) ).Should().BeGreaterThanOrEqualTo(100);
    }

    [Test]
    public async Task Should_Not_Seed_The_Database_In_Unit_Tests()
    {
        var context = await DovetailContext.FromAsync(Imports.Joints().Set(DovetailHostType.UnitTest), TestContext.CancellationToken);

        var services = await ApplyServicesAsync(context);

        await services.Should().All(x => x.ServiceType != typeof(RocketDbContext));
        await services.Should().All(x => !( x.ServiceType == typeof(IHostedService) && x.ImplementationType != null && x.ImplementationType.DeclaringType == typeof(DataGenerationJoint) ));
    }

    private async Task<ServiceProvider> CreateLiveProvider()
    {
        var context = await DovetailContext.FromAsync(Imports.Joints().Set(DovetailHostType.Live), TestContext.CancellationToken);
        var services = await ApplyServicesAsync(context);
        // An application stores the strong IDs through the Entity Framework joint of LaunchPad.Primitives. Sample.Core.Tests.csproj writes that
        // joint into this test project, and Dovetail exports a test project's own joints for the unit test host only, so it doesn't run for the
        // live host: the live host here configures the database the same way itself.
        LaunchPadPrimitives.ConfigureDbContexts(services);
        return services.BuildServiceProvider();
    }

    private async Task<ServiceCollection> ApplyServicesAsync(IDovetailContext context)
    {
        var configuration = new ConfigurationManager();
        await configuration.ApplyConfiguration(context, TestContext.CancellationToken);
        context.Set<IConfiguration>(configuration);

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        await services.ApplyService(context, TestContext.CancellationToken);
        return services;
    }

    private async Task StartHostedServices(IServiceProvider provider)
    {
        foreach (var hostedService in provider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(TestContext.CancellationToken);
        }
    }
}
