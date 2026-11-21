using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     Every application the SDK gives <c>Rocket.Surgery.LaunchPad.Hosting</c> gets OpenTelemetry, with the instrumentation of its host model
///     and the OTLP exporter, whose joints export once <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> is set: the generic host and ASP.NET Core get
///     <c>OpenTelemetry.Extensions.Hosting</c> and the runtime and process metrics, ASP.NET Core its request instrumentation, Blazor
///     WebAssembly neither. <c>LaunchPadEnableOtlpExporter</c> set to <c>false</c> leaves the exporter out, and
///     <c>LaunchPadEnableOpenTelemetry</c> (or Dovetail's <c>DovetailEnableOpenTelemetry</c>) set to <c>false</c> all of it.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkOpenTelemetryTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     Each host model gets the packages of its own telemetry.
    /// </summary>
    /// <param name="host">The host model.</param>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    [Arguments(HostModel.WebAssembly)]
    public async Task Should_Add_The_Telemetry_Of_The_Host_Model(HostModel host)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", host, HostModelShape.Project(host, targetFramework));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     An application that logs through Serilog also gets Serilog's OpenTelemetry sink, which exports its events.
    /// </summary>
    [Test]
    public async Task Should_Add_The_Serilog_Sink_To_An_Application_That_Logs_Through_Serilog()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.GenericHost,
            HostModelShape.Project(HostModel.GenericHost, targetFramework).ItemPackageReference("Serilog", SdkSerilogTests.SerilogVersion)
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     <c>LaunchPadEnableOtlpExporter</c> set to <c>false</c> leaves the exporter and Serilog's sink out, and keeps the instrumentation.
    /// </summary>
    [Test]
    public async Task Should_Leave_The_Exporter_Out_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.GenericHost,
            HostModelShape
               .Project(HostModel.GenericHost, targetFramework)
               .Property("LaunchPadEnableOtlpExporter", "false")
               .ItemPackageReference("Serilog", SdkSerilogTests.SerilogVersion)
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     LaunchPad's switch, or Dovetail's, set to <c>false</c> leaves all of OpenTelemetry out.
    /// </summary>
    /// <param name="property">The switch the project sets to <c>false</c>.</param>
    [Test]
    [Arguments("LaunchPadEnableOpenTelemetry")]
    [Arguments("DovetailEnableOpenTelemetry")]
    public async Task Should_Leave_OpenTelemetry_Out_When_Opted_Out(string property)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, HostModelShape.Project(HostModel.AspNetCore, targetFramework).Property(property, "false"));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     A project that isn't an application LaunchPad.Hosting is added to gets none of it: a library has no host model.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_OpenTelemetry_To_A_Library()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("library", HostModel.None, HostModelShape.Project(HostModel.None, targetFramework));

        await Verify(await tree.Evaluate("library", Cancellation));
    }
}
