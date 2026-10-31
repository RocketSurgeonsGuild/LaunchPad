using Hosting.Tests.Scaffolding;

namespace Hosting.Tests;

/// <summary>
///     The instrumentation joints of <c>Rocket.Surgery.LaunchPad.Hosting</c>, which replace <c>LaunchPad.Telemetry</c>'s
///     <c>InstrumentationConvention</c> and the ASP.NET Core and GraphQL instrumentation joints. They are OpenTelemetry joints, so they are
///     written only into an application that turns OpenTelemetry on (it references <c>Joinery.Dovetail.OpenTelemetry</c>, which Dovetail's
///     SDK adds when OpenTelemetry is referenced) and references the instrumentation each one adds. What the application listens to when
///     it runs is what shows the joints were applied.
/// </summary>
public class InstrumentationTemplateTests
{
    private const string InstrumentationJoint = "LaunchPadInstrumentationJoint";
    private const string RuntimeInstrumentationJoint = "LaunchPadRuntimeInstrumentationJoint";
    private const string AspNetCoreInstrumentationJoint = "LaunchPadAspNetCoreInstrumentationJoint";
    private const string HotChocolateInstrumentationJoint = "LaunchPadHotChocolateInstrumentationJoint";

    private static readonly string[] InstrumentationPackages =
    [
        "OpenTelemetry.Instrumentation.Http",
        "OpenTelemetry.Instrumentation.Runtime",
        "OpenTelemetry.Instrumentation.Process",
        "HotChocolate.Diagnostics",
    ];

    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     The HTTP client traces (recording exceptions) and metrics, the runtime and process metrics, and GraphQL's traces on both hosts;
    ///     the ASP.NET Core traces (recording exceptions) and metrics on ASP.NET Core only.
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    public async Task Should_Give_An_Application_With_OpenTelemetry_The_Instrumentation_Joints(HostModel host)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, host).WithProgram(Probe(host)));

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await Verify(new { Host = host, Templates = tree.EmittedTemplates("app"), Output = output.Trim() });
    }

    /// <summary>
    ///     Blazor WebAssembly gets the HTTP client instrumentation, and not the runtime and process metrics, even when the application
    ///     references them.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_WebAssembly_The_Runtime_And_Process_Instrumentation()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, HostModel.WebAssembly));

        await tree.Build("app", Cancellation);

        await Verify(new { Host = HostModel.WebAssembly, Templates = tree.EmittedTemplates("app") });
    }

    /// <summary>
    ///     An application that references the instrumentation but does not turn OpenTelemetry on gets none of the joints (and builds
    ///     without them).
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    public async Task Should_Not_Give_An_Application_Without_OpenTelemetry_The_Instrumentation_Joints(HostModel host)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, host, openTelemetry: false));

        await tree.Build("app", Cancellation);

        await Verify(new { Host = host, Templates = tree.EmittedTemplates("app") });
    }

    /// <summary>
    ///     An application turns OpenTelemetry on but references none of the instrumentation: it gets none of the joints.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_An_Application_Without_The_Instrumentation_Packages_The_Instrumentation_Joints()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, HostModel.AspNetCore, instrumentation: false));

        await tree.Build("app", Cancellation);

        await Verify(new { Host = HostModel.AspNetCore, Templates = tree.EmittedTemplates("app") });
    }

    /// <summary>
    ///     The text that lands in the application.
    /// </summary>
    [Test]
    [Arguments(InstrumentationJoint)]
    [Arguments(RuntimeInstrumentationJoint)]
    [Arguments(AspNetCoreInstrumentationJoint)]
    [Arguments(HotChocolateInstrumentationJoint)]
    public async Task Should_Write_The_Instrumentation_Joint_As_An_Internal_Embedded_OpenTelemetry_Joint(string joint)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, HostModel.AspNetCore));

        await tree.Build("app", Cancellation);

        await Verify(tree.EmittedTemplate("app", joint), "cs");
    }

    /// <summary>
    ///     An application of <paramref name="host" /> that references the packed package and, unless told otherwise, the instrumentation
    ///     and Dovetail's OpenTelemetry joints (with OpenTelemetry.Extensions.Hosting, which the generic host needs to start them).
    /// </summary>
    private static AppProject Application(PackedHosting package, HostModel host, bool openTelemetry = true, bool instrumentation = true)
    {
        var application = AppProject.Standalone(host).AddPackage(package.PackageId, package.Version);
        if (openTelemetry)
        {
            application.AddPackage("Joinery.Dovetail.OpenTelemetry");
            if (host is not HostModel.WebAssembly) application.AddPackage("OpenTelemetry.Extensions.Hosting");
        }

        if (!instrumentation) return application;

        foreach (var id in InstrumentationPackages)
        {
            application.AddPackage(id);
        }

        if (host is HostModel.AspNetCore) application.AddPackage("OpenTelemetry.Instrumentation.AspNetCore");
        return application;
    }

    /// <summary>
    ///     A program that builds the host through Dovetail and reports which of the instrumented activity sources and meters
    ///     OpenTelemetry listens to, and whether the HTTP client and ASP.NET Core traces record exceptions. A meter is only enabled once
    ///     a reader collects it, so the program adds one.
    /// </summary>
    private static string Probe(HostModel host) =>
        $$"""
          using System;
          using System.Diagnostics;
          using System.Diagnostics.Metrics;
          using Microsoft.Extensions.DependencyInjection;
          using Microsoft.Extensions.Options;
          using OpenTelemetry;
          using OpenTelemetry.Metrics;
          using OpenTelemetry.Trace;
          using Throwaway;

          var builder = {{( host == HostModel.AspNetCore ? "Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args)" : "Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args)" )}};
          builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddReader(new BaseExportingMetricReader(new NoopExporter())));
          var host = await builder.ConfigureDovetail();
          _ = host.Services.GetService<TracerProvider>();
          _ = host.Services.GetService<MeterProvider>();

          foreach (var name in new[] { "System.Net.Http", "HotChocolate.Diagnostics" })
          {
              using var source = new ActivitySource(name);
              Console.WriteLine($"Traces {name}:{source.HasListeners()}");
          }

          foreach (var name in new[] { "System.Net.Http", "System.Runtime", "OpenTelemetry.Instrumentation.Process", "Microsoft.AspNetCore.Hosting" })
          {
              using var meter = new Meter(name);
              Console.WriteLine($"Metrics {name}:{meter.CreateCounter<long>("probe").Enabled}");
          }

          Console.WriteLine($"RecordException HttpClient:{RecordsExceptions(host.Services, "OpenTelemetry.Instrumentation.Http.HttpClientTraceInstrumentationOptions, OpenTelemetry.Instrumentation.Http")}");
          Console.WriteLine($"RecordException AspNetCore:{RecordsExceptions(host.Services, "OpenTelemetry.Instrumentation.AspNetCore.AspNetCoreTraceInstrumentationOptions, OpenTelemetry.Instrumentation.AspNetCore")}");

          // the options types are looked up by name, as the generic host does not reference the ASP.NET Core instrumentation
          static bool? RecordsExceptions(IServiceProvider services, string optionsType) =>
              Type.GetType(optionsType) is { } type
           && services.GetService(typeof(IOptionsMonitor<>).MakeGenericType(type)) is { } monitor
           && monitor.GetType().GetProperty("CurrentValue")!.GetValue(monitor) is { } options
                  ? (bool?)type.GetProperty("RecordException")!.GetValue(options)
                  : null;

          sealed class NoopExporter : BaseExporter<Metric>
          {
              public override ExportResult Export(in Batch<Metric> batch) => ExportResult.Success;
          }
          """;
}
