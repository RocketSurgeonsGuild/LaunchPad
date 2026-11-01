using Hosting.Tests.Scaffolding;

using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     The project SDK syntax: a project names <c>Rocket.Surgery.LaunchPad.Sdk</c>, with its version, as its only SDK
///     (<c>&lt;Project Sdk="Rocket.Surgery.LaunchPad.Sdk/1.2.3"&gt;</c>). The Dovetail SDK it chains brings Microsoft.NET.Sdk.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway app.</param>
[TargetFrameworks]
public class SdkProjectSdkTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     A generic host that names the SDK alone gets LaunchPad.Hosting and its health check joint, as one that applies it in a
    ///     <c>Directory.Build.props</c> does.
    /// </summary>
    [Test]
    public async Task Should_Apply_The_LaunchPad_Joints_To_A_Project_That_Names_The_Sdk_Alone()
    {
        var packages = await PackedSdk.Pack();
        using var tree = new SdkTree(packages, targetFramework);
        // a Directory.Build.props of its own, so the tree's, which applies the SDK to every project below it, doesn't reach the project
        tree
           .AddFile("projectsdk/Directory.Build.props", ProjectCreator.Create().Property("NuGetAudit", "false"))
           .AddFile(
                "projectsdk/app/app.csproj",
                ProjectCreator
                   .Templates.SdkCsproj(sdk: $"{PackedSdk.SdkPackageId}/{packages.Version}", targetFramework: targetFramework, outputType: "Exe")
                   .Property("RootNamespace", HostModelShape.RootNamespace)
                   .ItemPackageReference("Microsoft.Extensions.Hosting", PinnedVersion("Microsoft.Extensions.Hosting"))
                   .ItemPackageReference(HostModelShape.HealthChecks, PinnedVersion(HostModelShape.HealthChecks))
            );
        await File.WriteAllTextAsync(
            Path.Combine(tree.Directory, "projectsdk", "app", "Program.cs"),
            HostModelShape.Of(HostModel.GenericHost).Program,
            Cancellation
        );

        var output = await DotnetCli.Run("run --project projectsdk/app", tree.Directory, cancellationToken: Cancellation);

        output.ShouldContain("HealthChecks:True");
    }

    private string PinnedVersion(string packageId) => Repository.PackageVersion(packageId, targetFramework);
}
