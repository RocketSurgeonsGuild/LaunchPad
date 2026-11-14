using Microsoft.Extensions.DependencyInjection;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using ZeroAlloc.Mediator;

namespace Sample.Worker.Tests;

public class BackgroundWorkerTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Should_List_The_Rockets_With_A_Mediator_From_A_Scope_Of_Its_Own()
    {
        var mediator = IMediator.Mock();
        mediator.CreateStream(Any<ListRockets.Request>(), Any()).Returns(AsyncEnumerable.Empty<RocketModel>());
        var scopes = 0;
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped(
            _ =>
            {
                Interlocked.Increment(ref scopes);
                return mediator.Object;
            }
        );
        // a provider that validates scopes: resolving the scoped mediator from the root provider would throw
        await using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true, });
        var worker = ActivatorUtilities.CreateInstance<BackgroundWorker>(provider);

        await worker.StartAsync(TestContext.CancellationToken);
        await WaitUntil(() => Volatile.Read(ref scopes) >= 1);
        await worker.StopAsync(TestContext.CancellationToken);

        mediator.CreateStream(Any<ListRockets.Request>(), Any()).WasCalled();
    }

    private static async Task WaitUntil(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (!condition())
        {
            await Task.Delay(10, timeout.Token);
        }
    }
}
