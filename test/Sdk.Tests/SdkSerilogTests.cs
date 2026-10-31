using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     Dovetail's SDK adds <c>Joinery.Dovetail.Serilog</c> to a project that references Serilog only when it is turned on; the LaunchPad SDK
///     turns it on. Like every detection of the SDK, it looks at the project's direct package references only.
/// </summary>
public class SdkSerilogTests
{
    /// <summary>
    ///     The version of Serilog the throwaway projects reference: the repository pins none of its own, as Serilog arrives through the
    ///     packages it does pin.
    /// </summary>
    private const string SerilogVersion = "4.3.0";

    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     An application that references Serilog itself gets Dovetail's Serilog support.
    /// </summary>
    [Test]
    public async Task Should_Turn_Dovetail_Serilog_On_For_A_Direct_Serilog_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack());
        tree.AddProject("app", new SdkApp(HostModel.GenericHost).AddPackage("Serilog", SerilogVersion));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that only receives Serilog through a library it references does not: the library chose Serilog, not the
    ///     application. The library references Serilog itself, so it gets the support.
    /// </summary>
    [Test]
    public async Task Should_Not_Turn_Dovetail_Serilog_On_For_A_Transitive_Serilog_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack());
        tree.AddProject("library", new SdkApp(HostModel.None).AddPackage("Serilog", SerilogVersion));
        tree.AddProject("app", new SdkApp(HostModel.GenericHost).AddProjectReference("library"));

        await Verify(new { Application = await tree.Evaluate("app", Cancellation), Library = await tree.Evaluate("library", Cancellation) });
    }

    /// <summary>
    ///     <c>DovetailEnableSerilog</c> set to <c>false</c> in the project, Dovetail's own switch, still turns it off: the SDK's default is set
    ///     before the project is evaluated.
    /// </summary>
    [Test]
    public async Task Should_Leave_Dovetail_Serilog_Off_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack());
        tree.AddProject("app", new SdkApp(HostModel.GenericHost).AddPackage("Serilog", SerilogVersion).SetProperty("DovetailEnableSerilog", "false"));

        await Verify(await tree.Evaluate("app", Cancellation));
    }
}
