using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

using Serilog.AspNetCore.Tests.Joints;

namespace Serilog.AspNetCore.Tests;

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
