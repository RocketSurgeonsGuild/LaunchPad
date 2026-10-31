using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     An application applies <c>Rocket.Surgery.LaunchPad.Sdk</c> in its <c>Directory.Build.props</c> and pins it in <c>global.json</c>, and
///     that is all its LaunchPad wiring: the SDK chains Dovetail's SDK, whose host detection finds the host model, and adds
///     <c>Rocket.Surgery.LaunchPad.Hosting</c> for it. Modelled on the SDK tests of Dovetail, which build throwaway projects the same way.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkHostModelTests(string targetFramework)
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
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", host);

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A library has no host model, so it gets Dovetail (it can export joints) and nothing of LaunchPad.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_A_Library_LaunchPad_Hosting()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("library", HostModel.None);

        await Verify(await tree.Evaluate("library", Cancellation));
    }

    /// <summary>
    ///     <c>LaunchPadEnableHosting</c> set to <c>false</c> keeps LaunchPad.Hosting out of an application, and Dovetail's own packages in.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_An_Application_That_Opts_Out_LaunchPad_Hosting()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework).Property("LaunchPadEnableHosting", "false"));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     The package the SDK adds is a direct reference of the application, so LaunchPad.Hosting's templates are written into it and its
    ///     joints run: the application that references the health checks gets them registered (ASP.NET Core has them in its framework).
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    public async Task Should_Apply_The_LaunchPad_Hosting_Joints_To_The_Application(HostModel host)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        var project = HostModelShape.Project(host, targetFramework);
        if (host == HostModel.GenericHost)
        {
            project.ItemPackageReference(HostModelShape.HealthChecks, Repository.PackageVersion(HostModelShape.HealthChecks, targetFramework));
        }

        tree.AddProject("app", host, project);

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await Verify(new { Host = host, Output = output.Trim() });
    }

    /// <summary>
    ///     Most applications manage their package versions centrally. The packages the SDK adds carry their own versions and are marked
    ///     implicitly defined, which central package management accepts (it rejects a version on any other reference), so the application
    ///     restores, builds and runs the joints the same way.
    /// </summary>
    [Test]
    public async Task Should_Apply_The_LaunchPad_Hosting_Joints_To_An_Application_With_Central_Package_Management()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        var packages = ProjectCreator.Create().Property("ManagePackageVersionsCentrally", "true");
        foreach (var package in HostModelShape.Of(HostModel.GenericHost).Packages.Append(HostModelShape.HealthChecks))
        {
            packages.ItemInclude("PackageVersion", package, metadata: new Dictionary<string, string?> { ["Version"] = Repository.PackageVersion(package, targetFramework), });
        }

        tree.AddFile("Directory.Packages.props", packages);
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework, centralVersions: true).ItemPackageReference(HostModelShape.HealthChecks));

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await Verify(new { Evaluation = await tree.Evaluate("app", Cancellation), Output = output.Trim() });
    }
}
