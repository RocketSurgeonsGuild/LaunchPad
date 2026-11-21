using Hosting.Tests.Scaffolding;

namespace Hosting.Tests;

/// <summary>
///     The browser's OTLP exporter: Blazor WebAssembly exports its traces, metrics and logs (OpenTelemetry's, as Serilog's OpenTelemetry sink
///     doesn't run in the browser) over HTTP/protobuf to a path of its own origin, the route Aspire's Blazor gateway forwards to the
///     dashboard, so it has a joint of its own instead of the server's. A WebAssembly application cannot run on the machine the tests do, so the tests read
///     what was written into it and whether it builds.
/// </summary>
public class WebAssemblyTelemetryTemplateTests
{
    private const string OtlpExporterJoint = "LaunchPadWebAssemblyOtlpExporterJoint";

    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     A browser application with OpenTelemetry and the exporter, as the LaunchPad SDK gives it, gets the browser's exporter and none of
    ///     the server's, not even Serilog's sink when it references it.
    /// </summary>
    [Test]
    public async Task Should_Give_A_Blazor_WebAssembly_Application_The_Browser_Exporter()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", BrowserApplication(package));

        await tree.Build("app", Cancellation);

        await Verify(new { Host = HostModel.WebAssembly, Templates = tree.EmittedTemplates("app") });
    }

    /// <summary>
    ///     A server application that references the same packages gets the server's exporters, not the browser's.
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    public async Task Should_Not_Give_A_Server_Application_The_Browser_Exporter(HostModel host)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject(
            "app",
            AppProject
               .Standalone(host)
               .AddPackage(package.PackageId, package.Version)
               .AddPackage("Joinery.Dovetail.OpenTelemetry")
               .AddPackage("OpenTelemetry.Extensions.Hosting")
               .AddPackage("OpenTelemetry.Exporter.OpenTelemetryProtocol")
               .AddPackage(AppProject.DovetailSerilog)
               .AddPackage("Serilog.Sinks.OpenTelemetry")
        );

        await tree.Build("app", Cancellation);

        await tree.EmittedTemplates("app").Should().Contain("LaunchPadOtlpExporterJoint");
        await tree.EmittedTemplates("app").Should().NotContain(OtlpExporterJoint);
    }

    /// <summary>
    ///     The text that lands in the application, which is compiled there against whatever it references.
    /// </summary>
    [Test]
    public async Task Should_Write_The_Browser_Exporter_As_An_Internal_Embedded_Live_OpenTelemetry_Joint()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", BrowserApplication(package));

        await tree.Build("app", Cancellation);

        await Verify(tree.EmittedTemplate("app", OtlpExporterJoint), "cs");
    }

    /// <summary>
    ///     A Blazor WebAssembly application in a tree outside the repository, with the packed package, the OpenTelemetry packages the LaunchPad
    ///     SDK gives a browser application, and Serilog with its OpenTelemetry sink, which a browser doesn't get a joint for.
    /// </summary>
    private static AppProject BrowserApplication(PackedHosting package) =>
        AppProject
           .Standalone(HostModel.WebAssembly)
           .AddPackage(package.PackageId, package.Version)
           .AddPackage("Joinery.Dovetail.OpenTelemetry")
           .AddPackage("OpenTelemetry.Exporter.OpenTelemetryProtocol")
           .AddPackage("OpenTelemetry.Instrumentation.Http")
           .AddPackage(AppProject.DovetailSerilog)
           .AddPackage("Serilog.Sinks.OpenTelemetry");
}
