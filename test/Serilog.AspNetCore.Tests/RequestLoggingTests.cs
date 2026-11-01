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
