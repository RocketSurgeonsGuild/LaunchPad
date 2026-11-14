using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        await logEvent.Level.Should().BeEqualTo(LogEventLevel.Information);
        await logEvent.Properties["RequestPath"].ToString().Should().BeEqualTo("\"/ping\"");
        await logEvent.Properties["StatusCode"].ToString().Should().BeEqualTo("200");
    }

    [Test]
    public async Task Should_Add_The_Request_Properties()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapGet("/ping", () => "pong").WithDisplayName("Ping"));

        await host.Client.GetAsync("/ping?name=value");

        var properties = ( await host.Sink.SingleRequestEventAsync() ).Properties;
        await properties["Host"].ToString().Should().BeEqualTo("\"localhost\"");
        await properties["Protocol"].ToString().Should().BeEqualTo("\"HTTP/1.1\"");
        await properties["Scheme"].ToString().Should().BeEqualTo("\"http\"");
        await properties["QueryString"].ToString().Should().BeEqualTo("\"?name=value\"");
        await properties["EndpointName"].ToString().Should().BeEqualTo("\"Ping\"");
        await properties.Should().ContainKey("ContentType");
    }

    [Test]
    public async Task Should_Leave_Out_An_Empty_Query_String()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapGet("/ping", () => "pong"));

        await host.Client.GetAsync("/ping");

        await ( await host.Sink.SingleRequestEventAsync() ).Properties.Should().NotContainKey("QueryString");
    }

    [Test]
    public async Task Should_Log_A_Server_Error_As_An_Error()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapGet("/fail", () => Results.StatusCode(503)));

        await host.Client.GetAsync("/fail");

        await ( await host.Sink.SingleRequestEventAsync() ).Level.Should().BeEqualTo(LogEventLevel.Error);
    }

    [Test]
    public async Task Should_Log_A_Client_Error_As_Information()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapGet("/missing", () => Results.NotFound()));

        await host.Client.GetAsync("/missing");

        await ( await host.Sink.SingleRequestEventAsync() ).Level.Should().BeEqualTo(LogEventLevel.Information);
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
        await logEvent.Level.Should().BeEqualTo(LogEventLevel.Error);
        await logEvent.Exception.Should().BeOfType(typeof(InvalidOperationException));
    }

    [Test]
    public async Task Should_Log_A_Health_Check_As_Verbose()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapHealthChecks("/health"));

        await host.Client.GetAsync("/health");

        await ( await host.Sink.SingleRequestEventAsync() ).Level.Should().BeEqualTo(LogEventLevel.Verbose);
    }

    [Test]
    public async Task Should_Add_The_Action_To_The_Request_Event_Of_An_Mvc_Action()
    {
        await using var host = await RequestLoggingHost.StartAsync(app => app.MapControllers());

        await host.Client.GetAsync("/request-logging/42");

        var properties = ( await host.Sink.SingleRequestEventAsync() ).Properties;
        await properties["ActionName"].ToString().Should().Contain(nameof(RequestLoggingController.Get));
        await properties.Should().ContainKey("ActionId");
        await properties["RouteData"].ToString().Should().Contain("RequestLogging");
        await properties["ValidationState"].ToString().Should().BeEqualTo("True");
    }
}

[ApiController]
[Route("request-logging")]
public class RequestLoggingController : ControllerBase
{
    [HttpGet("{id:int}")]
    public ActionResult<int> Get(int id) => id;
}
