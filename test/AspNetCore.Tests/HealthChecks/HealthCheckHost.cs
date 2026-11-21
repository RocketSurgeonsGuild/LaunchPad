using System.Net;
using System.Net.Sockets;

using AspNetCore.Tests.Joints;

using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.HealthChecks;

/// <summary>
///     An ASP.NET Core application with the <see cref="HealthChecksJoint" /> applied, LaunchPad's health check endpoints mapped and a
///     <c>/ping</c> endpoint of its own, on the in-memory test server or on Kestrel.
/// </summary>
internal sealed class HealthCheckHost(WebApplication app, HttpClient client) : IAsyncDisposable
{
    public static async Task<HealthCheckHost> StartAsync(
        IDictionary<string, string?>? configuration = null,
        Action<WebApplicationBuilder>? configure = null,
        bool kestrel = false
    )
    {
        var builder = WebApplication.CreateSlimBuilder();
        if (!kestrel) builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(configuration ?? new Dictionary<string, string?>());
        await builder.Services.ApplyService(await JointTestContext.CreateBuilder().AppendJoint<HealthChecksJoint>().CreateAsync());
        configure?.Invoke(builder);

        var app = builder.Build();
        app.MapGet("/ping", () => "pong");
        app.MapLaunchPadHealthChecks();
        await app.StartAsync();
        return new(app, kestrel ? new HttpClient() : app.GetTestClient());
    }

    /// <summary>A port nothing listens on right now.</summary>
    public static int FreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ( (IPEndPoint)listener.LocalEndpoint ).Port;
    }

    public HttpClient Client => client;
    public IServiceProvider Services => app.Services;

    public Task<HttpResponseMessage> GetAsync(string path) => client.GetAsync(new Uri(path, UriKind.RelativeOrAbsolute));

    public static async Task<HttpStatusCode> StatusAsync(HttpClient client, int port, string path)
    {
        using var response = await client.GetAsync(new Uri($"http://127.0.0.1:{port}{path}"));
        return response.StatusCode;
    }

    public async ValueTask DisposeAsync()
    {
        client.Dispose();
        await app.StopAsync();
        await app.DisposeAsync();
    }
}
