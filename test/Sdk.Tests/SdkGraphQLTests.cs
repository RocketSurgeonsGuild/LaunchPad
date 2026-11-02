using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     A project that references HotChocolate itself gets <c>Rocket.Surgery.LaunchPad.HotChocolate</c>, and one that references a
///     StrawberryShake client package itself gets <c>Rocket.Surgery.LaunchPad.StrawberryShake</c>. <c>LaunchPadEnableHotChocolate</c> and
///     <c>LaunchPadEnableStrawberryShake</c> set to <c>false</c> opt a project out.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkGraphQLTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     A GraphQL server, by either of the packages an application references for it, gets LaunchPad's HotChocolate package.
    /// </summary>
    /// <param name="package">The HotChocolate package the application references.</param>
    [Test]
    [Arguments("HotChocolate")]
    [Arguments("HotChocolate.AspNetCore")]
    public async Task Should_Add_LaunchPad_HotChocolate_For_A_Direct_HotChocolate_Reference(string package)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemPackageReference(package));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A GraphQL client, by any of the StrawberryShake client packages, gets LaunchPad's StrawberryShake package.
    /// </summary>
    /// <param name="package">The StrawberryShake package the application references.</param>
    [Test]
    [Arguments("StrawberryShake.Core")]
    [Arguments("StrawberryShake.Server")]
    [Arguments("StrawberryShake.Blazor")]
    [Arguments("StrawberryShake.Maui")]
    public async Task Should_Add_LaunchPad_StrawberryShake_For_A_Direct_StrawberryShake_Reference(string package)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemPackageReference(package));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that only receives HotChocolate through a library it references does not get the package: the library chose
    ///     HotChocolate, not the application.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_LaunchPad_HotChocolate_For_A_Transitive_HotChocolate_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("library", HostModel.None, HostModelShape.Project(HostModel.None, targetFramework).ItemPackageReference("HotChocolate"));
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemProjectReference("../library/library.csproj"));

        await Verify(new { Application = await tree.Evaluate("app", Cancellation), Library = await tree.Evaluate("library", Cancellation) });
    }

    /// <summary>
    ///     Each opt-out leaves its own package out, and only its own.
    /// </summary>
    /// <param name="optOut">The property set to <c>false</c>.</param>
    [Test]
    [Arguments("LaunchPadEnableHotChocolate")]
    [Arguments("LaunchPadEnableStrawberryShake")]
    public async Task Should_Not_Add_The_Package_Of_A_Technology_That_Is_Opted_Out(string optOut)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.GenericHost,
            HostModelShape
               .Project(HostModel.GenericHost, targetFramework)
               .Property(optOut, "false")
               .ItemPackageReference("HotChocolate")
               .ItemPackageReference(ThirdParty.StrawberryShake, ThirdParty.StrawberryShakeVersion)
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }
}
