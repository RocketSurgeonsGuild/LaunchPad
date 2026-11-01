using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     A project that references NetTopologySuite itself gets <c>Rocket.Surgery.LaunchPad.Spatial</c>, and the spatial variant of each
///     LaunchPad integration whose partner it also references. Like every detection of the SDK, it looks at the project's direct package
///     references only.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkNetTopologySuiteTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     An application that references NetTopologySuite gets LaunchPad's spatial package, and nothing for the partners it does not
    ///     reference.
    /// </summary>
    [Test]
    public async Task Should_Add_Spatial_For_A_Direct_NetTopologySuite_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, App(HostModel.GenericHost).ItemPackageReference(ThirdParty.NetTopologySuite, ThirdParty.NetTopologySuiteVersion));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     StrawberryShake is the partner of LaunchPad's StrawberryShake integration: a project that references a StrawberryShake client
    ///     package gets the spatial serializers for its GraphQL clients.
    /// </summary>
    [Test]
    public async Task Should_Add_StrawberryShake_Spatial_To_A_Project_That_References_StrawberryShake()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, WithNetTopologySuiteAndStrawberryShake(App(HostModel.GenericHost)));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     <c>LaunchPadEnableNetTopologySuite</c> set to <c>false</c> keeps every spatial package out, the partner variants included.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_Anything_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, WithNetTopologySuiteAndStrawberryShake(App(HostModel.GenericHost)).Property("LaunchPadEnableNetTopologySuite", "false"));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    private static ProjectCreator WithNetTopologySuiteAndStrawberryShake(ProjectCreator project) =>
        project
           .ItemPackageReference(ThirdParty.NetTopologySuite, ThirdParty.NetTopologySuiteVersion)
           .ItemPackageReference(ThirdParty.StrawberryShake, ThirdParty.StrawberryShakeVersion);

    private ProjectCreator App(HostModel host) => HostModelShape.Project(host, targetFramework);
}
