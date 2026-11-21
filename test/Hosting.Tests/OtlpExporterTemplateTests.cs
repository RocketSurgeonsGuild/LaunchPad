using Hosting.Tests.Scaffolding;

namespace Hosting.Tests;

/// <summary>
///     The OTLP exporter joint of <c>Rocket.Surgery.LaunchPad.Hosting</c>. It is an OpenTelemetry joint, written into an application that
///     turns OpenTelemetry on (it references <c>Joinery.Dovetail.OpenTelemetry</c>) and references the OTLP exporter. It exports the traces
///     and metrics only when <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> is set, and only in a live application: what a listener on that endpoint
///     receives is what shows it.
/// </summary>
public class OtlpExporterTemplateTests
{
    private const string OtlpExporterJoint = "LaunchPadOtlpExporterJoint";

    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     A live application exports its traces and metrics to the endpoint its configuration names; without an endpoint it exports
    ///     nothing, and a unit test host (a build that pins the Dovetail host type, as a test project does) exports nothing either.
    /// </summary>
    [Test]
    [Arguments("Live", true)]
    [Arguments("Live", false)]
    [Arguments("UnitTest", true)]
    public async Task Should_Export_Traces_And_Metrics_Only_To_A_Configured_Endpoint_In_A_Live_Application(string hostType, bool endpoint)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package).AddProperty("DovetailHostType", hostType).WithProgram(ExportProgram));

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", endpoint ? "endpoint" : string.Empty, Cancellation);

        await Verify(new { HostType = hostType, Endpoint = endpoint, Templates = tree.EmittedTemplates("app"), Output = output.Trim() });
    }

    /// <summary>
    ///     An application that turns OpenTelemetry on but does not reference the OTLP exporter gets no exporter joint.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_An_Application_Without_The_Exporter_The_Exporter_Joint()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, exporter: false));

        await tree.Build("app", Cancellation);

        await tree.EmittedTemplates("app").Should().NotContain(OtlpExporterJoint);
    }

    /// <summary>
    ///     The text that lands in the application.
    /// </summary>
    [Test]
    public async Task Should_Write_The_Exporter_Joint_As_An_Internal_Embedded_Live_OpenTelemetry_Joint()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package));

        await tree.Build("app", Cancellation);

        await Verify(tree.EmittedTemplate("app", OtlpExporterJoint), "cs");
    }

    /// <summary>
    ///     A generic-host application that references the packed package and Dovetail's OpenTelemetry joints (with
    ///     OpenTelemetry.Extensions.Hosting, which the generic host needs to start them) and, unless told otherwise, the OTLP exporter.
    /// </summary>
    private static AppProject Application(PackedHosting package, bool exporter = true)
    {
        var application = AppProject
                         .Standalone(HostModel.GenericHost)
                         .AddPackage(package.PackageId, package.Version)
                         .AddPackage("Joinery.Dovetail.OpenTelemetry")
                         .AddPackage("OpenTelemetry.Extensions.Hosting");
        return exporter ? application.AddPackage("OpenTelemetry.Exporter.OpenTelemetryProtocol") : application;
    }

    /// <summary>
    ///     Starts a listener, points the OTLP exporter (over HTTP) at it when the program is given <c>endpoint</c>, records an activity
    ///     and a measurement, disposes the host (which flushes the exporters) and writes the paths of the requests the listener received.
    /// </summary>
    private const string ExportProgram = """
        using System;
        using System.Collections.Concurrent;
        using System.Collections.Generic;
        using System.Diagnostics;
        using System.Diagnostics.Metrics;
        using System.Linq;
        using System.Net;
        using System.Net.Sockets;
        using System.Threading.Tasks;
        using Microsoft.Extensions.Configuration;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Hosting;
        using OpenTelemetry.Metrics;
        using OpenTelemetry.Trace;
        using Throwaway;

        var free = new TcpListener(IPAddress.Loopback, 0);
        free.Start();
        var port = ((IPEndPoint)free.LocalEndpoint).Port;
        free.Stop();

        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        listener.Start();
        var paths = new ConcurrentBag<string>();
        _ = Task.Run(
            async () =>
            {
                while (true)
                {
                    HttpListenerContext request;
                    try
                    {
                        request = await listener.GetContextAsync();
                    }
                    catch
                    {
                        return;
                    }

                    // answer at once: disposing the host waits for the export to complete
                    paths.Add(request.Request.Url!.AbsolutePath);
                    request.Response.StatusCode = 200;
                    request.Response.Close();
                }
            }
        );

        var builder = Host.CreateApplicationBuilder();
        if (args is ["endpoint"])
        {
            builder.Configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["OTEL_EXPORTER_OTLP_ENDPOINT"] = $"http://127.0.0.1:{port}",
                    ["OTEL_EXPORTER_OTLP_PROTOCOL"] = "http/protobuf",
                }
            );
        }

        builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddSource("Probe"));
        builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddMeter("Probe"));
        var host = await builder.ConfigureDovetail();
        _ = host.Services.GetService<TracerProvider>();
        _ = host.Services.GetService<MeterProvider>();

        using var source = new ActivitySource("Probe");
        using var meter = new Meter("Probe");
        using (source.StartActivity("probe")) { }
        meter.CreateCounter<long>("probe").Add(1);
        host.Dispose();

        await Task.Delay(TimeSpan.FromSeconds(1));
        Console.WriteLine($"Exported:{( paths.IsEmpty ? "nothing" : string.Join(",", paths.Distinct().Order()) )}");
        """;
}
