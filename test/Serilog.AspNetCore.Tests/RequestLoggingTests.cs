using System.Collections.Concurrent;

using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

using Serilog.AspNetCore.Tests.Joints;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.AspNetCore.Tests;

public class RequestLoggingTests
{
    [Test]
    public async Task Should_Write_The_Request_Event_To_The_Serilog_Logger_In_The_Container()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapGet("/ping", () => "pong"));

        await host.Client.GetAsync("/ping");

        var logEvent = await host.Sink.SingleRequestEventAsync();
        logEvent.Level.ShouldBe(LogEventLevel.Information);
        logEvent.Properties["RequestPath"].ToString().ShouldBe("\"/ping\"");
        logEvent.Properties["StatusCode"].ToString().ShouldBe("200");
    }

    [Test]
    public async Task Should_Add_The_Request_Properties()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapGet("/ping", () => "pong").WithDisplayName("Ping"));

        await host.Client.GetAsync("/ping?name=value");

        var properties = ( await host.Sink.SingleRequestEventAsync() ).Properties;
        properties["Host"].ToString().ShouldBe("\"localhost\"");
        properties["Protocol"].ToString().ShouldBe("\"HTTP/1.1\"");
        properties["Scheme"].ToString().ShouldBe("\"http\"");
        properties["QueryString"].ToString().ShouldBe("\"?name=value\"");
        properties["EndpointName"].ToString().ShouldBe("\"Ping\"");
        properties.ShouldContainKey("ContentType");
    }

    [Test]
    public async Task Should_Leave_Out_An_Empty_Query_String()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapGet("/ping", () => "pong"));

        await host.Client.GetAsync("/ping");

        ( await host.Sink.SingleRequestEventAsync() ).Properties.ShouldNotContainKey("QueryString");
    }

    [Test]
    public async Task Should_Log_A_Server_Error_As_An_Error()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapGet("/fail", () => Results.StatusCode(503)));

        await host.Client.GetAsync("/fail");

        ( await host.Sink.SingleRequestEventAsync() ).Level.ShouldBe(LogEventLevel.Error);
    }

    [Test]
    public async Task Should_Log_A_Client_Error_As_Information()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapGet("/missing", () => Results.NotFound()));

        await host.Client.GetAsync("/missing");

        ( await host.Sink.SingleRequestEventAsync() ).Level.ShouldBe(LogEventLevel.Information);
    }

    [Test]
    public async Task Should_Log_An_Unhandled_Exception_As_An_Error()
    {
        await using var host = await RequestLoggingHost.StartAsync(
            app => app.MapGet("/throw", string () => throw new InvalidOperationException("boom"))
        );

        try
        {
            await host.Client.GetAsync("/throw");
        }
        catch (InvalidOperationException)
        {
            // the test server rethrows what the application did not handle
        }

        var logEvent = await host.Sink.SingleRequestEventAsync();
        logEvent.Level.ShouldBe(LogEventLevel.Error);
        logEvent.Exception.ShouldBeOfType<InvalidOperationException>();
    }

    [Test]
    public async Task Should_Log_A_Health_Check_As_Verbose()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapHealthChecks("/health"));

        await host.Client.GetAsync("/health");

        ( await host.Sink.SingleRequestEventAsync() ).Level.ShouldBe(LogEventLevel.Verbose);
    }

    [Test]
    public async Task Should_Add_The_Action_To_The_Request_Event_Of_An_Mvc_Action()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapControllers());

        await host.Client.GetAsync("/request-logging/42");

        var properties = ( await host.Sink.SingleRequestEventAsync() ).Properties;
        properties["ActionName"].ToString().ShouldContain(nameof(RequestLoggingController.Get));
        properties.ShouldContainKey("ActionId");
        properties["RouteData"].ToString().ShouldContain("RequestLogging");
        properties["ValidationState"].ToString().ShouldBe("True");
    }
}

[ApiController]
[Route("request-logging")]
public class RequestLoggingController : ControllerBase
{
    [HttpGet("{id:int}")]
    public ActionResult<int> Get(int id) => id;
}

/// <summary>
///     An ASP.NET Core application on the test server, with a Serilog logger in the container that writes to a sink the test reads, and the
///     <see cref="SerilogRequestLoggingJoint" /> applied.
/// </summary>
internal sealed class RequestLoggingHost(WebApplication app, CapturingSink sink) : IAsyncDisposable
{
    public static async Task<RequestLoggingHost> StartAsync(Action<WebApplication> map)
    {
        var sink = new CapturingSink();
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();

        var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(sink).CreateLogger();
        builder.Services.AddSingleton(logger).AddSingleton<ILogger>(logger);
        builder.Services.AddRouting().AddHealthChecks();
        builder.Services.AddControllers().AddApplicationPart(typeof(RequestLoggingController).Assembly);
        await builder.Services.ApplyService(await JointTestContext.CreateBuilder().AppendJoint<SerilogRequestLoggingJoint>().CreateAsync());

        var app = builder.Build();
        app.UseLaunchPadRequestLogging();
        map(app);
        await app.StartAsync();
        return new(app, sink);
    }

    public HttpClient Client { get; } = app.GetTestClient();
    public CapturingSink Sink => sink;

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await app.StopAsync();
        await app.DisposeAsync();
    }
}

internal sealed class CapturingSink : ILogEventSink
{
    public void Emit(LogEvent logEvent) => _events.Enqueue(logEvent);

    /// <summary>
    ///     The event the request logging middleware wrote. It is written once the response has completed, which can be after the client has
    ///     the response, so this waits a little for it.
    /// </summary>
    public async Task<LogEvent> SingleRequestEventAsync()
    {
        for (var attempt = 0; attempt < 50; attempt++)
        {
            var events = _events.Where(x => x.MessageTemplate.Text.StartsWith("HTTP ", StringComparison.Ordinal)).ToArray();
            if (events.Length > 0) return events.ShouldHaveSingleItem();
            await Task.Delay(20);
        }

        throw new ShouldAssertException("The request logging middleware wrote no event to the logger in the container.");
    }

    private readonly ConcurrentQueue<LogEvent> _events = new();
}
