using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     The LaunchPad SDK adds the ZeroAlloc integration packages to a project that references ZeroAlloc.Validation (or
///     ZeroAlloc.Mediator.Validation) directly, each only when its partner is there too: ASP.NET Core (the host model), gRPC
///     (<c>Grpc.AspNetCore</c>), GraphQL (<c>HotChocolate</c>) and Blazor (the WebAssembly host model, or
///     <c>Microsoft.AspNetCore.Components.Web</c>). <c>LaunchPadEnableZeroAlloc</c> set to <c>false</c> opts a project out.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkZeroAllocTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     A library that references ZeroAlloc.Validation gets nothing: there is no core package, and no partner is there.
    /// </summary>
    [Test]
    public async Task Should_Give_A_Library_No_ZeroAlloc_Package()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("library", HostModel.None, WithZeroAlloc(HostModel.None));

        await Verify(await tree.Evaluate("library", Cancellation));
    }

    /// <summary>
    ///     An ASP.NET Core application gets the ASP.NET Core package.
    /// </summary>
    [Test]
    public async Task Should_Give_An_AspNetCore_Application_ZeroAlloc_For_AspNetCore()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, WithZeroAlloc(HostModel.AspNetCore));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that references ZeroAlloc.Mediator.Validation alone has chosen ZeroAlloc validation too.
    /// </summary>
    [Test]
    public async Task Should_Give_An_Application_That_References_The_Mediator_Validation_ZeroAlloc_For_AspNetCore()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.AspNetCore,
            HostModelShape
               .Project(HostModel.AspNetCore, targetFramework)
               .ItemPackageReference("ZeroAlloc.Mediator.Validation", Repository.PackageVersion("ZeroAlloc.Mediator.Validation", targetFramework))
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that references gRPC gets the gRPC package.
    /// </summary>
    [Test]
    public async Task Should_Give_An_Application_ZeroAlloc_For_Grpc()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, WithZeroAlloc(HostModel.AspNetCore).ItemPackageReference("Grpc.AspNetCore", Repository.PackageVersion("Grpc.AspNetCore", targetFramework)));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that references HotChocolate gets the GraphQL package.
    /// </summary>
    [Test]
    public async Task Should_Give_An_Application_ZeroAlloc_For_GraphQL()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, WithZeroAlloc(HostModel.AspNetCore).ItemPackageReference("HotChocolate", Repository.PackageVersion("HotChocolate", targetFramework)));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A Blazor WebAssembly application gets the Blazor package.
    /// </summary>
    [Test]
    public async Task Should_Give_A_WebAssembly_Application_ZeroAlloc_For_Blazor()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.WebAssembly, WithZeroAlloc(HostModel.WebAssembly));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     <c>LaunchPadEnableZeroAlloc</c> set to <c>false</c> in the project leaves every ZeroAlloc package out.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_ZeroAlloc_Packages_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.AspNetCore,
            WithZeroAlloc(HostModel.AspNetCore)
               .Property("LaunchPadEnableZeroAlloc", "false")
               .ItemPackageReference("Grpc.AspNetCore", Repository.PackageVersion("Grpc.AspNetCore", targetFramework))
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    private ProjectCreator WithZeroAlloc(HostModel host) =>
        HostModelShape.Project(host, targetFramework).ItemPackageReference("ZeroAlloc.Validation", Repository.PackageVersion("ZeroAlloc.Validation", targetFramework));
}
