using FakeItEasy;
using Hosting.Tests.Scaffolding;
using Joinery.Dovetail;
using Microsoft.Extensions.Hosting;

namespace Hosting.Tests;

/// <summary>
///     The application lifecycle comes from Dovetail: <c>Joinery.Dovetail.Hosting</c> emits <c>OnHostStarting</c>, <c>OnHostStarted</c>,
///     <c>OnHostStopping</c> and <c>OnHostStopped</c> into every project that references the generic host, so this project runs its own
///     copy. LaunchPad carries no lifecycle code; this is the test that used to live with the Razor Pages sample tests.
/// </summary>
public class HostLifecycleTests
{
    [Test]
    public async Task Should_Run_The_Handlers_Of_Each_Moment_Once()
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

        using var app = builder.Build();
        await app.StartAsync(TestContext.Current!.Execution.CancellationToken);
        await app.StopAsync(TestContext.Current!.Execution.CancellationToken);

        A.CallTo(onStarted).MustHaveHappenedOnceExactly();
        A.CallTo(onStarting).MustHaveHappenedOnceExactly();
        A.CallTo(onStopped).MustHaveHappenedOnceExactly();
        A.CallTo(onStopping).MustHaveHappenedOnceExactly();
    }

    /// <summary>
    ///     An application that references the packed package and Dovetail's hosting package registers its handlers on the application
    ///     builder of the generic host and of ASP.NET Core, and they run at all four moments. The application keeps the
    ///     <c>using Rocket.Surgery.LaunchPad.Hosting;</c> it had when the package held its own lifecycle extensions: it must not make
    ///     the calls ambiguous with Dovetail's.
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    public async Task Should_Run_The_Handlers_Of_An_Application_That_References_The_Package(HostModel host)
    {
        var cancellation = TestContext.Current!.Execution.CancellationToken;
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, host));

        await tree.Build("app", cancellation);
        var output = await tree.Run("app", cancellation);

        // the host also logs where it listens and its content root, which are not what the test is about
        var moments = output.Split('\n', StringSplitOptions.TrimEntries).Where(z => z.StartsWith("lifecycle:", StringComparison.Ordinal)).Select(z => z["lifecycle:".Length..]);
        await Verify(new { Host = host, Moments = moments });
    }

    private static AppProject Application(PackedHosting package, HostModel host)
    {
        var builder = host == HostModel.AspNetCore ? "Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args)" : "Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args)";
        return AppProject
              .Standalone(host)
              .AddPackage(package.PackageId, package.Version)
              .WithProgram(
                   $$"""
                   using Joinery.Dovetail;
                   using Throwaway;
                   using Rocket.Surgery.LaunchPad.Hosting;

                   var builder = {{builder}};
                   builder
                      .OnHostStarting(_ => System.Console.WriteLine("lifecycle:starting"))
                      .OnHostStarted(async _ => { await System.Threading.Tasks.Task.Yield(); System.Console.WriteLine("lifecycle:started"); })
                      .OnHostStopping(async (_, _) => { await System.Threading.Tasks.Task.Yield(); System.Console.WriteLine("lifecycle:stopping"); })
                      .OnHostStopped(_ => System.Console.WriteLine("lifecycle:stopped"));

                   using var host = await builder.ConfigureDovetail();
                   await host.StartAsync();
                   await host.StopAsync();
                   """
               );
    }
}
