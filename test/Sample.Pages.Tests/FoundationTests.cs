using System.Net;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.LaunchPad.Hosting;
using Sample.Pages.Tests.Helpers;

namespace Sample.Pages.Tests;

public class FoundationTests(ITestOutputHelper testOutputHelper, TestWebAppFixture factory) : WebAppFixtureTest<TestWebAppFixture>(testOutputHelper, factory)
{
    [Fact]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
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
        builder.Services.AddHostedService<ApplicationLifecycleService>();

        var app = builder.Build();
        await app.StartAsync();
        await app.StopAsync();

        A.CallTo(onStarted).MustHaveHappenedOnceExactly();
        A.CallTo(onStarting).MustHaveHappenedOnceExactly();
        A.CallTo(onStopped).MustHaveHappenedOnceExactly();
        A.CallTo(onStopping).MustHaveHappenedOnceExactly();
    }
}