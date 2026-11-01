using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     The LaunchPad SDK adds LaunchPad's own Serilog packages to a project that references Serilog directly, each only when its partner is
///     there too: <c>Rocket.Surgery.LaunchPad.Serilog.AspNetCore</c> (request logging) to an ASP.NET Core application.
///     <c>LaunchPadEnableSerilog</c> set to <c>false</c> opts a project out.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkSerilogPackageTests(string targetFramework)
{
    /// <summary>
    ///     The version of Serilog the throwaway projects reference: the repository pins none of its own, as Serilog arrives through the
    ///     packages it does pin.
    /// </summary>
    private const string SerilogVersion = "4.3.0";

    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     An ASP.NET Core application that references Serilog gets the request logging package.
    /// </summary>
    [Test]
    public async Task Should_Give_An_AspNetCore_Application_Serilog_Request_Logging()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, HostModelShape.Project(HostModel.AspNetCore, targetFramework).ItemPackageReference("Serilog", SerilogVersion));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A generic-host application that references Serilog does not: it has no ASP.NET Core requests to log.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_A_Generic_Host_Serilog_Request_Logging()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemPackageReference("Serilog", SerilogVersion));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An ASP.NET Core application that only receives Serilog through a library it references gets nothing: the library chose Serilog,
    ///     not the application.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_Serilog_Packages_For_A_Transitive_Serilog_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("library", HostModel.None, HostModelShape.Project(HostModel.None, targetFramework).ItemPackageReference("Serilog", SerilogVersion));
        tree.AddProject("app", HostModel.AspNetCore, HostModelShape.Project(HostModel.AspNetCore, targetFramework).ItemProjectReference("../library/library.csproj"));

        await Verify(new { Application = await tree.Evaluate("app", Cancellation), Library = await tree.Evaluate("library", Cancellation) });
    }

    /// <summary>
    ///     <c>LaunchPadEnableSerilog</c> set to <c>false</c> in the project leaves LaunchPad's Serilog packages out. Dovetail's own Serilog
    ///     support has its own switch, so it stays on.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_Serilog_Packages_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.AspNetCore,
            HostModelShape.Project(HostModel.AspNetCore, targetFramework).Property("LaunchPadEnableSerilog", "false").ItemPackageReference("Serilog", SerilogVersion)
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }
}
