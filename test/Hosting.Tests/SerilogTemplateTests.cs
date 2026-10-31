using Hosting.Tests.Scaffolding;

namespace Hosting.Tests;

/// <summary>
///     The Serilog joints of <c>Rocket.Surgery.LaunchPad.Hosting</c>. Each is a template written into an application that references
///     <c>Joinery.Dovetail.Serilog</c> (the Serilog logger itself) and the packages the joint compiles against: the package brings none of
///     them. The applications run, log through <c>Microsoft.Extensions.Logging</c> and report what Serilog made of it.
/// </summary>
public class SerilogTemplateTests
{
    private const string DovetailSerilog = "Joinery.Dovetail.Serilog";

    /// <summary>
    ///     The joints that need nothing but Serilog itself.
    /// </summary>
    private static readonly string[] CoreJoints =
    [
        "LaunchPadSerilogApplicationStateJoint",
        "LaunchPadSerilogEnrichmentJoint",
        "LaunchPadSerilogSystemTextJsonJoint",
    ];

    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     An application with Serilog and nothing else gets the joints that need nothing else: the log context and the host environment
    ///     enrich every event, and a <c>System.Text.Json</c> value is logged as the structure it holds.
    /// </summary>
    [Test]
    public async Task Should_Give_An_Application_That_References_Serilog_The_Core_Serilog_Joints()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", SerilogApplication(package));

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        tree.EmittedTemplates("app").Where(IsSerilogJoint).ShouldBe(CoreJoints);
        output.ShouldContain("event: Hello { a: 1, b: [True, null] }");
        output.ShouldContain("  Pushed = \"from the log context\"");
        output.ShouldContain("  EnvironmentName = \"Production\"");
        output.ShouldContain("  ApplicationName = \"app\"");
    }

    /// <summary>
    ///     Without Joinery.Dovetail.Serilog there is no Serilog logger for the joints to configure, so an application that references every
    ///     enricher and sink package but not it gets none of them, and logs nothing through Serilog.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_An_Application_That_Does_Not_Reference_Dovetail_Serilog_Any_Serilog_Joint()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        var application = AppProject.Standalone(HostModel.GenericHost).AddPackage(package.PackageId, package.Version).WithProgram(SerilogProgram);
        foreach (var id in EveryPackage.Append("Serilog.Sinks.OpenTelemetry"))
        {
            application.AddPackage(id);
        }

        tree.AddProject("app", application);

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        tree.EmittedTemplates("app").Where(IsSerilogJoint).ShouldBeEmpty();
        output.ShouldNotContain("event:");
    }

    /// <summary>
    ///     The level the application is asked to run at wins over the minimum level of the configuration: <c>Serilog:MinimumLevel</c> lets
    ///     everything through, and <c>ApplicationState:LogLevel</c> (a <c>Microsoft.Extensions.Logging</c> level) keeps the information
    ///     event out.
    /// </summary>
    [Test]
    public async Task Should_Log_At_The_Level_Of_The_Application_State()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", SerilogApplication(package));

        await tree.Build("app", Cancellation);
        var configured = await tree.Run("app", "--Serilog:MinimumLevel:Default=Verbose", Cancellation);
        var applicationState = await tree.Run("app", "--Serilog:MinimumLevel:Default=Verbose --ApplicationState:LogLevel=Warning", Cancellation);

        configured.ShouldContain("event: Hello");
        configured.ShouldContain("event: Failed");
        applicationState.ShouldNotContain("event: Hello");
        applicationState.ShouldContain("event: Failed");
    }

    /// <summary>
    ///     Each of the other joints is written only into an application that references the packages of its enrichers or sinks, so the
    ///     application chooses them with its references.
    /// </summary>
    [Test]
    [Arguments("LaunchPadSerilogConsoleJoint", "Serilog.Sinks.Console;Serilog.Sinks.Async")]
    [Arguments("LaunchPadSerilogDebugJoint", "Serilog.Sinks.Debug;Serilog.Sinks.Async")]
    [Arguments("LaunchPadSerilogTraceJoint", "Serilog.Sinks.Trace;Serilog.Sinks.Async")]
    [Arguments("LaunchPadSerilogSpanJoint", "Serilog.Enrichers.Span")]
    [Arguments("LaunchPadSerilogExceptionsJoint", "Serilog.Exceptions;Serilog.Enrichers.Demystifier")]
    [Arguments("LaunchPadSerilogEnvironmentEnrichersJoint", "Serilog.Enrichers.Environment;Serilog.Enrichers.Process;Serilog.Enrichers.Thread;Serilog.Enrichers.AssemblyName")]
    [Arguments("LaunchPadSerilogDiagnosticContextJoint", "Serilog.Extensions.Hosting")]
    public async Task Should_Give_An_Application_A_Serilog_Joint_When_It_References_Its_Packages(string joint, string packages)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        var application = SerilogApplication(package);
        foreach (var id in packages.Split(';'))
        {
            application.AddPackage(id);
        }

        tree.AddProject("app", application);

        await tree.Build("app", Cancellation);

        tree.EmittedTemplates("app").Where(IsSerilogJoint).ShouldBe(CoreJoints.Append(joint).Order(StringComparer.Ordinal));
    }

    /// <summary>
    ///     An application with every package gets every joint, and they all take part in what it logs: the sinks write the events, the
    ///     enrichers add their properties, and the diagnostic context the request logging enriches is registered.
    /// </summary>
    [Test]
    public async Task Should_Log_Through_Every_Serilog_Joint()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", SerilogApplication(package, everything: true));

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        // the sinks write the message with the literal (lj) format, which renders a structure as JSON; the debug sink writes to the trace
        // listeners as well, and the trace listener puts its own prefix in front of the message
        output.ShouldContain("console: Hello {\"a\": 1, \"b\": [true, null]}");
        output.ShouldContain("debug: Hello {\"a\":1,\"b\":[true,null]}");
        output.ShouldContain("trace: Hello {\"a\":1,\"b\":[true,null]}");
        // the properties of the first event, as the program writes them: the name, and the value when it is the same on every machine
        var lines = output.ReplaceLineEndings("\n").Split('\n');
        lines.ShouldContain("  AssemblyName = \"app\"");
        foreach (var property in new[]
                 {
                     "AssemblyInformationalVersion", "AssemblyVersion", "EnvironmentUserName", "MachineName", "ProcessId", "ProcessName", "ThreadId",
                     "ParentId", "SpanId", "TraceId", "ExceptionDetail",
                 })
        {
            lines.ShouldContain($"  {property}");
        }

        output.ShouldContain("DiagnosticContext:True");
    }

    /// <summary>
    ///     An application that references the OpenTelemetry sink exports its logs over OTLP to the endpoint its configuration names, and a
    ///     unit test host (a build that pins the Dovetail host type, as a test project does) exports nothing: the joint is a live joint.
    /// </summary>
    [Test]
    [Arguments("Live", true)]
    [Arguments("UnitTest", false)]
    public async Task Should_Export_Logs_Through_OpenTelemetry_Only_In_A_Live_Application(string hostType, bool exported)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject(
            "app",
            AppProject
               .Standalone(HostModel.GenericHost)
               .AddPackage(package.PackageId, package.Version)
               .AddPackage(DovetailSerilog)
               .AddPackage("Serilog.Sinks.OpenTelemetry")
               .AddProperty("DovetailHostType", hostType)
               .WithProgram(OpenTelemetryProgram)
        );

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        tree.EmittedTemplates("app").ShouldContain("LaunchPadSerilogOpenTelemetryJoint");
        output.ShouldContain(exported ? "Exported:/v1/logs" : "Exported:nothing");
    }

    private static bool IsSerilogJoint(string template) => template.StartsWith("LaunchPadSerilog", StringComparison.Ordinal);

    /// <summary>
    ///     A generic-host application with the packed package and Serilog, and the program that reports what Serilog logs.
    /// </summary>
    private static AppProject SerilogApplication(PackedHosting package, bool everything = false)
    {
        var application = AppProject
                         .Standalone(HostModel.GenericHost)
                         .AddPackage(package.PackageId, package.Version)
                         .AddPackage(DovetailSerilog)
                         .WithProgram(SerilogProgram);
        if (!everything) return application;

        foreach (var id in EveryPackage)
        {
            application.AddPackage(id);
        }

        return application;
    }

    /// <summary>
    ///     The packages of every enricher and sink the joints use.
    /// </summary>
    private static readonly string[] EveryPackage =
    [
        "Serilog.Enrichers.AssemblyName",
        "Serilog.Enrichers.Demystifier",
        "Serilog.Enrichers.Environment",
        "Serilog.Enrichers.Process",
        "Serilog.Enrichers.Span",
        "Serilog.Enrichers.Thread",
        "Serilog.Exceptions",
        "Serilog.Extensions.Hosting",
        "Serilog.Sinks.Async",
        "Serilog.Sinks.Console",
        "Serilog.Sinks.Debug",
        "Serilog.Sinks.Trace",
    ];

    /// <summary>
    ///     Points the OTLP exporter (over HTTP) at a listener of its own, logs an event, disposes the host (which flushes the sink) and writes
    ///     the path of the request the listener received, if any.
    /// </summary>
    private const string OpenTelemetryProgram = """
        using System;
        using System.Collections.Generic;
        using System.Net;
        using System.Net.Sockets;
        using System.Threading.Tasks;
        using Microsoft.Extensions.Configuration;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Hosting;
        using Microsoft.Extensions.Logging;
        using Throwaway;

        var free = new TcpListener(IPAddress.Loopback, 0);
        free.Start();
        var port = ((IPEndPoint)free.LocalEndpoint).Port;
        free.Stop();

        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        listener.Start();
        var request = listener.GetContextAsync().ContinueWith(
            task =>
            {
                // answer at once: disposing the host waits for the export to complete
                var path = task.Result.Request.Url!.AbsolutePath;
                task.Result.Response.StatusCode = 200;
                task.Result.Response.Close();
                return path;
            },
            TaskScheduler.Default
        );

        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["OTEL_EXPORTER_OTLP_ENDPOINT"] = $"http://127.0.0.1:{port}",
                ["OTEL_EXPORTER_OTLP_PROTOCOL"] = "http/protobuf",
            }
        );
        var host = await builder.ConfigureDovetail();
        host.Services.GetRequiredService<ILogger<Program>>().LogInformation("Exported");
        host.Dispose();

        var exported = await Task.WhenAny(request, Task.Delay(TimeSpan.FromSeconds(2))) == request;
        Console.WriteLine($"Exported:{( exported ? request.Result : "nothing" )}");
        """;

    /// <summary>
    ///     Logs two events through <c>Microsoft.Extensions.Logging</c> inside a log context property and an activity, and once the host is
    ///     disposed (which flushes every sink) writes each event Serilog captured: its rendered message and its properties, with the value of
    ///     the ones that are the same on every machine. The sinks the joints add write with their own prefix (<c>console:</c>,
    ///     <c>trace:</c>), so the output shows which of them ran.
    /// </summary>
    private const string SerilogProgram = """"
        using System;
        using System.Collections.Generic;
        using System.Diagnostics;
        using System.Linq;
        using Joinery.Dovetail;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Hosting;
        using Microsoft.Extensions.Logging;
        using Rocket.Surgery.LaunchPad.Hosting;
        using Serilog;
        using Serilog.Context;
        using Serilog.Core;
        using Serilog.Events;
        using Throwaway;

        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Throwaway",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
        };
        ActivitySource.AddActivityListener(listener);
        using var source = new ActivitySource("Throwaway");

        var capture = new Capture();
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.Configure<LoggerConfiguration>(configuration => configuration.WriteTo.Sink(capture));
        var host = await builder.ConfigureDovetail(
            context => context.Set(
                new LaunchPadLoggingOptions
                {
                    ConsoleMessageTemplate = "console: {Message:lj}{NewLine}",
                    DebugMessageTemplate = "debug: {Message:lj}{NewLine}",
                    TraceMessageTemplate = "trace: {Message:lj}{NewLine}",
                }
            )
        );

        var logger = host.Services.GetRequiredService<ILogger<Capture>>();
        using (LogContext.PushProperty("Pushed", "from the log context"))
        using (source.StartActivity("Work"))
        {
            using var json = System.Text.Json.JsonDocument.Parse("""{"a": 1, "b": [true, null]}""");
            logger.LogInformation("Hello {@Data}", json.RootElement);
            logger.LogError(new InvalidOperationException("boom"), "Failed");
        }

        var diagnosticContext = Type.GetType("Serilog.IDiagnosticContext, Serilog.Extensions.Hosting") is { } type && host.Services.GetService(type) is not null;
        host.Dispose();

        foreach (var logEvent in capture.Events)
        {
            Console.WriteLine($"event: {logEvent.RenderMessage()}");
            foreach (var (name, value) in logEvent.Properties.OrderBy(z => z.Key, StringComparer.Ordinal))
            {
                Console.WriteLine(Capture.Stable.Contains(name) ? $"  {name} = {value}" : $"  {name}");
            }
        }

        Console.WriteLine($"DiagnosticContext:{diagnosticContext}");

        sealed class Capture : ILogEventSink
        {
            public static readonly HashSet<string> Stable = ["ApplicationName", "AssemblyName", "Data", "EnvironmentName", "Pushed", "SourceContext"];
            public List<LogEvent> Events { get; } = [];
            public void Emit(LogEvent logEvent) => Events.Add(logEvent);
        }
        """";
}
