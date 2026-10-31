using System.Net;

using FakeItEasy;
using Microsoft.Extensions.Hosting;

using Rocket.Surgery.LaunchPad.Hosting;

using Sample.Pages.Tests.Helpers;

namespace Sample.Pages.Tests;

public class FoundationTests : WebAppFixtureTest
{
    [Test]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/", TestContext.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task StartingEvents()
    {
        var builder = Host.CreateApplicationBuilder();
        var onStarted = A.Fake<Func<IServiceProvider, Task>>();
        var onStarting = A.Fake<Action<IServiceProvider>>();
        var onStopping = A.Fake<Func<IServiceProvider, CancellationToken, Task>>();
        var onStopped = A.Fake<Func<IServiceProvider, Task>>();
        builder.OnHostStarted(onStarted);
        builder.OnHostStarting(onStarting);
        builder.OnHostStopped(onStopped);
        builder.OnHostStopping(onStopping);

        var app = builder.Build();
        await app.StartAsync(TestContext.CancellationToken);
        await app.StopAsync(TestContext.CancellationToken);

        A.CallTo(onStarted).MustHaveHappenedOnceExactly();
        A.CallTo(onStarting).MustHaveHappenedOnceExactly();
        A.CallTo(onStopped).MustHaveHappenedOnceExactly();
        A.CallTo(onStopping).MustHaveHappenedOnceExactly();
    }
}
