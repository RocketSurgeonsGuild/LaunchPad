using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     An ASP.NET Core application (the WebApplication host model) gets <c>Rocket.Surgery.LaunchPad.AspNetCore</c>, with its problem
///     details, OpenAPI and Serilog request logging joints, and <c>Rocket.Surgery.LaunchPad.AspNetCore.Mvc</c> with MVC's, unless it publishes
///     or is built for native AOT. <c>LaunchPadEnableAspNetCore</c> set to <c>false</c> opts out of both.
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
    ///     <c>LaunchPadEnableAspNetCore</c> set to <c>false</c> leaves the package out, and the ZeroAlloc integration with it: the
    ///     application still gets LaunchPad.Hosting and the ZeroAlloc.Validation package itself.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_The_AspNetCore_Packages_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.AspNetCore,
            HostModelShape.Project(HostModel.AspNetCore, targetFramework).Property("LaunchPadEnableAspNetCore", "false").ItemPackageReference("ZeroAlloc.Validation")
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A project that sets <c>PublishAot</c> or <c>IsAotCompatible</c> gets only the native AOT compatible package: no
    ///     <c>Rocket.Surgery.LaunchPad.AspNetCore.Mvc</c>.
    /// </summary>
    /// <param name="property">The native AOT property the project sets to <c>true</c>.</param>
    [Test]
    [Arguments("PublishAot")]
    [Arguments("IsAotCompatible")]
    public async Task Should_Not_Add_Mvc_To_A_Native_Aot_Project(string property)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, HostModelShape.Project(HostModel.AspNetCore, targetFramework).Property(property, "true"));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     <c>LaunchPadEnableAspNetCoreMvc</c> overrides the native AOT default either way: <c>true</c> adds MVC to a native AOT project, and
    ///     <c>false</c> leaves it out of any other.
    /// </summary>
    /// <param name="publishAot">The project's <c>PublishAot</c>.</param>
    /// <param name="enableMvc">The project's <c>LaunchPadEnableAspNetCoreMvc</c>.</param>
    [Test]
    [Arguments("true", "true")]
    [Arguments("false", "false")]
    public async Task Should_Let_The_Project_Choose_Mvc(string publishAot, string enableMvc)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.AspNetCore,
            HostModelShape.Project(HostModel.AspNetCore, targetFramework).Property("PublishAot", publishAot).Property("LaunchPadEnableAspNetCoreMvc", enableMvc)
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }
}
