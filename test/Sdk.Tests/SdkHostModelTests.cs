using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     An application applies <c>Rocket.Surgery.LaunchPad.Sdk</c> in its <c>Directory.Build.props</c> and pins it in <c>global.json</c>, and
///     that is all its LaunchPad wiring: the SDK chains Dovetail's SDK, whose host detection finds the host model, and adds
///     <c>Rocket.Surgery.LaunchPad.Hosting</c> for it. Modelled on the SDK tests of Dovetail, which build throwaway projects the same way.
/// </summary>
public class SdkHostModelTests
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     Every host model gets Dovetail's packages from the chained SDK and LaunchPad.Hosting from this one, with no other SDK entry.
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    [Arguments(HostModel.WebAssembly)]
    public async Task Should_Give_Each_Host_Model_LaunchPad_Hosting(HostModel host)
    {
        using var tree = new SdkTree(await PackedSdk.Pack());
        tree.AddProject("app", new(host));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A library has no host model, so it gets Dovetail (it can export joints) and nothing of LaunchPad.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_A_Library_LaunchPad_Hosting()
    {
        using var tree = new SdkTree(await PackedSdk.Pack());
        tree.AddProject("library", new(HostModel.None));

        await Verify(await tree.Evaluate("library", Cancellation));
    }

    /// <summary>
    ///     <c>LaunchPadEnableHosting</c> set to <c>false</c> keeps LaunchPad.Hosting out of an application, and Dovetail's own packages in.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_An_Application_That_Opts_Out_LaunchPad_Hosting()
    {
        using var tree = new SdkTree(await PackedSdk.Pack());
        tree.AddProject("app", new SdkApp(HostModel.GenericHost).SetProperty("LaunchPadEnableHosting", "false"));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     The package the SDK adds is a direct reference of the application, so LaunchPad.Hosting's templates are written into it and its
    ///     joints run: the application that references the health checks gets them registered.
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    public async Task Should_Apply_The_LaunchPad_Hosting_Joints_To_The_Application(HostModel host)
    {
        using var tree = new SdkTree(await PackedSdk.Pack());
        var app = new SdkApp(host);
        if (host == HostModel.GenericHost) app.AddPackage(SdkApp.HealthChecks);
        tree.AddProject("app", app);

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await Verify(new { Host = host, Output = output.Trim() });
    }
}
