using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     The LaunchPad SDK adds <c>Rocket.Surgery.LaunchPad.FluentValidation</c> to a project that references FluentValidation directly, and
///     each of its partner packages only when the partner is there too: ASP.NET Core (the host model), gRPC (<c>Grpc.AspNetCore</c>),
///     GraphQL (<c>HotChocolate</c>) and Blazor (the WebAssembly host model, or <c>Microsoft.AspNetCore.Components.Web</c>).
///     <c>LaunchPadEnableFluentValidation</c> set to <c>false</c> opts a project out.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkFluentValidationTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     A library that references FluentValidation gets the core package, and no partner package.
    /// </summary>
    [Test]
    public async Task Should_Give_A_Library_FluentValidation()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("library", HostModel.None, WithFluentValidation(HostModel.None));

        await Verify(await tree.Evaluate("library", Cancellation));
    }

    /// <summary>
    ///     A generic-host application gets the core package only: none of the partners is there.
    /// </summary>
    [Test]
    public async Task Should_Give_A_Generic_Host_FluentValidation()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, WithFluentValidation(HostModel.GenericHost));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An ASP.NET Core application gets the ASP.NET Core package as well.
    /// </summary>
    [Test]
    public async Task Should_Give_An_AspNetCore_Application_FluentValidation_For_AspNetCore()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, WithFluentValidation(HostModel.AspNetCore));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that references gRPC gets the gRPC package, and not the GraphQL one.
    /// </summary>
    [Test]
    public async Task Should_Give_An_Application_FluentValidation_For_Grpc()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, WithFluentValidation(HostModel.AspNetCore).ItemPackageReference("Grpc.AspNetCore", Repository.PackageVersion("Grpc.AspNetCore", targetFramework)));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that references HotChocolate gets the GraphQL package, and not the gRPC one.
    /// </summary>
    [Test]
    public async Task Should_Give_An_Application_FluentValidation_For_GraphQL()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, WithFluentValidation(HostModel.AspNetCore).ItemPackageReference("HotChocolate", Repository.PackageVersion("HotChocolate", targetFramework)));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that references only FluentValidation.DependencyInjectionExtensions, which brings FluentValidation along, has chosen
    ///     FluentValidation too.
    /// </summary>
    [Test]
    public async Task Should_Give_An_Application_That_References_The_Dependency_Injection_Extensions_FluentValidation()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.GenericHost,
            HostModelShape
               .Project(HostModel.GenericHost, targetFramework)
               .ItemPackageReference("FluentValidation.DependencyInjectionExtensions", Repository.PackageVersion("FluentValidation.DependencyInjectionExtensions", targetFramework))
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A Blazor WebAssembly application gets the Blazor package.
    /// </summary>
    [Test]
    public async Task Should_Give_A_WebAssembly_Application_FluentValidation_For_Blazor()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.WebAssembly, WithFluentValidation(HostModel.WebAssembly));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A component library, which references the Blazor components itself, gets the Blazor package.
    /// </summary>
    [Test]
    public async Task Should_Give_A_Component_Library_FluentValidation_For_Blazor()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "library",
            HostModel.None,
            WithFluentValidation(HostModel.None)
               .ItemPackageReference("Microsoft.AspNetCore.Components.Web", Repository.PackageVersion("Microsoft.AspNetCore.Components.Web", targetFramework))
        );

        await Verify(await tree.Evaluate("library", Cancellation));
    }

    /// <summary>
    ///     The partners alone add nothing: the application has not chosen FluentValidation.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_FluentValidation_Packages_Without_FluentValidation()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.AspNetCore,
            HostModelShape
               .Project(HostModel.AspNetCore, targetFramework)
               .ItemPackageReference("Grpc.AspNetCore", Repository.PackageVersion("Grpc.AspNetCore", targetFramework))
               .ItemPackageReference("HotChocolate.AspNetCore", Repository.PackageVersion("HotChocolate.AspNetCore", targetFramework))
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that only receives FluentValidation through a library it references gets nothing: the library chose
    ///     FluentValidation, not the application.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_FluentValidation_Packages_For_A_Transitive_FluentValidation_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("library", HostModel.None, WithFluentValidation(HostModel.None));
        tree.AddProject("app", HostModel.AspNetCore, HostModelShape.Project(HostModel.AspNetCore, targetFramework).ItemProjectReference("../library/library.csproj"));

        await Verify(new { Application = await tree.Evaluate("app", Cancellation), Library = await tree.Evaluate("library", Cancellation) });
    }

    /// <summary>
    ///     <c>LaunchPadEnableFluentValidation</c> set to <c>false</c> in the project leaves every FluentValidation package out.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_FluentValidation_Packages_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.AspNetCore,
            WithFluentValidation(HostModel.AspNetCore)
               .Property("LaunchPadEnableFluentValidation", "false")
               .ItemPackageReference("Grpc.AspNetCore", Repository.PackageVersion("Grpc.AspNetCore", targetFramework))
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    private ProjectCreator WithFluentValidation(HostModel host) =>
        HostModelShape.Project(host, targetFramework).ItemPackageReference("FluentValidation", Repository.PackageVersion("FluentValidation", targetFramework));
}
