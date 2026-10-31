using FakeItEasy;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Worker.Tests;

public class BackgroundWorkerTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Should_List_The_Rockets_With_A_Mediator_From_A_Scope_Of_Its_Own()
    {
        var mediator = A.Fake<IMediator>();
        A
           .CallTo(() => mediator.CreateStream(A<IStreamRequest<RocketModel>>._, A<CancellationToken>._))
           .Returns(AsyncEnumerable.Empty<RocketModel>());
        var scopes = 0;
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped(
            _ =>
            {
                Interlocked.Increment(ref scopes);
                return mediator;
            }
        );
        // a provider that validates scopes: resolving the scoped mediator from the root provider would throw
        await using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true, });
        var worker = ActivatorUtilities.CreateInstance<BackgroundWorker>(provider);

        await worker.StartAsync(TestContext.CancellationToken);
        await WaitUntil(() => Volatile.Read(ref scopes) >= 1);
        await worker.StopAsync(TestContext.CancellationToken);

        A.CallTo(() => mediator.CreateStream(A<ListRockets.Request>._, A<CancellationToken>._)).MustHaveHappened();
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
