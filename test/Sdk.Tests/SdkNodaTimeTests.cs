using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     A project that references NodaTime itself gets <c>Rocket.Surgery.LaunchPad.NodaTime</c>, and the NodaTime integration package of
///     every partner library it also references itself. Like every detection of the SDK, it looks at the project's direct package
///     references only.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkNodaTimeTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     NodaTime alone brings the core package and nothing else.
    /// </summary>
    [Test]
    public async Task Should_Add_LaunchPad_NodaTime_For_A_Direct_NodaTime_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemPackageReference("NodaTime", PinnedVersion("NodaTime")));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A partner library the project references itself, next to NodaTime, brings the NodaTime integration package for it. An evaluation
    ///     restores nothing, so the partner is referenced without a version.
    /// </summary>
    /// <param name="partner">The partner library.</param>
    [Test]
    [Arguments("HotChocolate")]
    [Arguments("HotChocolate.AspNetCore")]
    [Arguments("StrawberryShake.Core")]
    [Arguments("StrawberryShake.Server")]
    [Arguments("StrawberryShake.Blazor")]
    [Arguments("StrawberryShake.Maui")]
    public async Task Should_Add_The_NodaTime_Integration_Package_Of_A_Partner_Library(string partner)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.GenericHost,
            HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemPackageReference("NodaTime", PinnedVersion("NodaTime")).ItemPackageReference(partner)
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A partner library without NodaTime brings nothing of NodaTime.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_A_NodaTime_Integration_Package_Without_NodaTime()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemPackageReference("HotChocolate"));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that only receives NodaTime through a library it references does not get the packages: the library chose
    ///     NodaTime, not the application. The library references NodaTime itself, so it gets them.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_LaunchPad_NodaTime_For_A_Transitive_NodaTime_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("library", HostModel.None, HostModelShape.Project(HostModel.None, targetFramework).ItemPackageReference("NodaTime", PinnedVersion("NodaTime")));
        tree.AddProject(
            "app",
            HostModel.GenericHost,
            HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemProjectReference("../library/library.csproj").ItemPackageReference("HotChocolate")
        );

        await Verify(new { Application = await tree.Evaluate("app", Cancellation), Library = await tree.Evaluate("library", Cancellation) });
    }

    /// <summary>
    ///     <c>LaunchPadEnableNodaTime</c> set to <c>false</c> in the project leaves out the core package and every integration package.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_The_NodaTime_Packages_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.GenericHost,
            HostModelShape
               .Project(HostModel.GenericHost, targetFramework)
               .Property("LaunchPadEnableNodaTime", "false")
               .ItemPackageReference("NodaTime", PinnedVersion("NodaTime"))
               .ItemPackageReference("HotChocolate")
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     Opting the partner technology out keeps its integration package out, and leaves the NodaTime package itself.
    /// </summary>
    /// <param name="optOut">The property set to <c>false</c>.</param>
    /// <param name="partner">The partner library the project references.</param>
    [Test]
    [Arguments("LaunchPadEnableHotChocolate", "HotChocolate")]
    [Arguments("LaunchPadEnableStrawberryShake", "StrawberryShake.Server")]
    public async Task Should_Not_Add_The_NodaTime_Integration_Package_When_The_Partner_Is_Opted_Out(string optOut, string partner)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.GenericHost,
            HostModelShape
               .Project(HostModel.GenericHost, targetFramework)
               .Property(optOut, "false")
               .ItemPackageReference("NodaTime", PinnedVersion("NodaTime"))
               .ItemPackageReference(partner)
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     The package the SDK adds is a direct reference of the application, so Dovetail applies its joints: the application that references
    ///     NodaTime gets the system clock registered without referencing anything of LaunchPad itself.
    /// </summary>
    [Test]
    public async Task Should_Apply_The_LaunchPad_NodaTime_Joints_To_The_Application()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemPackageReference("NodaTime", PinnedVersion("NodaTime")));
        await File.WriteAllTextAsync(
            Path.Combine(tree.Directory, "app", "Program.cs"),
            $$"""
            using {{HostModelShape.RootNamespace}};

            var host = await Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args).ConfigureDovetail();
            System.Console.WriteLine($"Clock:{host.Services.GetService(typeof(NodaTime.IClock))?.GetType().Name}");
            """,
            Cancellation
        );

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await Verify(output.Trim());
    }

    private string PinnedVersion(string packageId) => Repository.PackageVersion(packageId, targetFramework);
}
