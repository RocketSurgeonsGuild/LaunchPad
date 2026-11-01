using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     An ASP.NET Core application (the WebApplication host model) gets <c>Rocket.Surgery.LaunchPad.AspNetCore</c>, with its MVC, problem
///     details, OpenAPI and Serilog request logging joints. <c>LaunchPadEnableAspNetCore</c> set to <c>false</c> opts it out.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkAspNetCoreTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     Each host model gets the ASP.NET Core package only when it is an ASP.NET Core application.
    /// </summary>
    /// <param name="host">The host model.</param>
    [Test]
    [Arguments(HostModel.AspNetCore)]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.WebAssembly)]
    public async Task Should_Add_LaunchPad_AspNetCore_Only_To_An_AspNetCore_Application(HostModel host)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", host, HostModelShape.Project(host, targetFramework));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     <c>LaunchPadEnableAspNetCore</c> set to <c>false</c> leaves the package out, and the FluentValidation integration with it: the
    ///     application still gets LaunchPad.Hosting and the FluentValidation package itself.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_The_AspNetCore_Packages_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.AspNetCore,
            HostModelShape.Project(HostModel.AspNetCore, targetFramework).Property("LaunchPadEnableAspNetCore", "false").ItemPackageReference("FluentValidation")
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }
}
