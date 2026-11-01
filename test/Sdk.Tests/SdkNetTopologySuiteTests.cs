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
    ///     Newtonsoft.Json is the partner of the spatial package, and the other way round: a project that references both gets the GeoJSON
    ///     converters for Newtonsoft.Json, as well as what each technology adds on its own.
    /// </summary>
    [Test]
    public async Task Should_Add_Spatial_NewtonsoftJson_To_A_Project_That_References_Newtonsoft_Json()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, WithBoth(App(HostModel.GenericHost)));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     Opting either technology out keeps the package they share out: <c>LaunchPadEnableNetTopologySuite</c> or
    ///     <c>LaunchPadEnableNewtonsoftJson</c> set to <c>false</c> leaves only what the other technology adds on its own.
    /// </summary>
    /// <param name="optOut">The property set to <c>false</c>.</param>
    [Test]
    [Arguments("LaunchPadEnableNetTopologySuite")]
    [Arguments("LaunchPadEnableNewtonsoftJson")]
    public async Task Should_Not_Add_Spatial_NewtonsoftJson_When_Either_Technology_Is_Opted_Out(string optOut)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, WithBoth(App(HostModel.GenericHost)).Property(optOut, "false"));

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
        tree.AddProject("app", HostModel.GenericHost, WithStrawberryShake(App(HostModel.GenericHost)));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     <c>LaunchPadEnableNetTopologySuite</c> set to <c>false</c> keeps every spatial package out, the partner variants included.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_Anything_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, WithStrawberryShake(App(HostModel.GenericHost)).Property("LaunchPadEnableNetTopologySuite", "false"));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    private static ProjectCreator WithStrawberryShake(ProjectCreator project) =>
        project
           .ItemPackageReference(ThirdParty.NetTopologySuite, ThirdParty.NetTopologySuiteVersion)
           .ItemPackageReference(ThirdParty.StrawberryShake, ThirdParty.StrawberryShakeVersion);

    private static ProjectCreator WithBoth(ProjectCreator project) =>
        project
           .ItemPackageReference(ThirdParty.NetTopologySuite, ThirdParty.NetTopologySuiteVersion)
           .ItemPackageReference(ThirdParty.NewtonsoftJson, ThirdParty.NewtonsoftJsonVersion);

    private ProjectCreator App(HostModel host) => HostModelShape.Project(host, targetFramework);
}
