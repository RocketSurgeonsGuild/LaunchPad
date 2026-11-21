using System.Net;

using Microsoft.Extensions.Configuration;

using Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

namespace AspNetCore.Tests.HealthChecks;

// These run the application on Kestrel, on real ports.
[NotInParallel(nameof(HealthCheckPortTests))]
public class HealthCheckPortTests
{
    [Test]
    public async Task Should_Listen_On_The_Health_Port_Next_To_The_Application_Urls()
    {
        var (appPort, healthPort) = (HealthCheckHost.FreePort(), HealthCheckHost.FreePort());
        await using var host = await HealthCheckHost.StartAsync(
            new Dictionary<string, string?> { ["urls"] = $"http://127.0.0.1:{appPort}", ["LaunchPad:HealthChecks:Port"] = $"{healthPort}" },
            kestrel: true
        );

        await AssertIsolatedAsync(host.Client, appPort, healthPort);
    }

    // ASPNETCORE_HTTP_PORTS, which the .NET container images set, is kept when the health port is added.
    [Test]
    public async Task Should_Keep_The_Http_Ports_When_Adding_The_Health_Port()
    {
        var (appPort, healthPort) = (HealthCheckHost.FreePort(), HealthCheckHost.FreePort());
        await using var host = await HealthCheckHost.StartAsync(
            new Dictionary<string, string?> { ["http_ports"] = $"{appPort}", ["LaunchPad:HealthChecks:Port"] = $"{healthPort}" },
            kestrel: true
        );

        await AssertIsolatedAsync(host.Client, appPort, healthPort);
    }

    // Kestrel ignores urls once Kestrel:Endpoints declares an endpoint, so the health port is added to Kestrel itself.
    [Test]
    public async Task Should_Listen_On_The_Health_Port_Next_To_The_Kestrel_Endpoints()
    {
        var (appPort, healthPort) = (HealthCheckHost.FreePort(), HealthCheckHost.FreePort());
        await using var host = await HealthCheckHost.StartAsync(
            new Dictionary<string, string?>
            {
                ["Kestrel:Endpoints:Http:Url"] = $"http://127.0.0.1:{appPort}", ["LaunchPad:HealthChecks:Port"] = $"{healthPort}",
            },
            kestrel: true
        );

        await AssertIsolatedAsync(host.Client, appPort, healthPort);
    }

    [Test]
    [Arguments(null, null, null, "http://localhost:5000;http://*:8081")]
    [Arguments("http://localhost:5001;https://localhost:7001", null, null, "http://localhost:5001;https://localhost:7001;http://*:8081")]
    [Arguments(null, "8080", null, "http://*:8080;http://*:8081")]
    [Arguments(null, "8080", "8443", "http://*:8080;https://*:8443;http://*:8081")]
    [Arguments("http://*:8081", null, null, "http://*:8081")]
    public async Task Should_Append_The_Health_Address_To_The_Addresses_The_Host_Binds(string? urls, string? httpPorts, string? httpsPorts, string expected)
    {
        var configuration = new ConfigurationBuilder()
                           .AddInMemoryCollection(new Dictionary<string, string?> { ["urls"] = urls, ["http_ports"] = httpPorts, ["https_ports"] = httpsPorts })
                           .Build();

        await HealthCheckKestrelSetup.AppendAddress(configuration, 8081).Should().BeEqualTo(expected);
    }

    private static async Task AssertIsolatedAsync(HttpClient client, int appPort, int healthPort)
    {
        await ( await HealthCheckHost.StatusAsync(client, healthPort, "/health") ).Should().BeEqualTo(HttpStatusCode.OK);
        await ( await HealthCheckHost.StatusAsync(client, healthPort, "/health/live") ).Should().BeEqualTo(HttpStatusCode.OK);
        await ( await HealthCheckHost.StatusAsync(client, healthPort, "/health/ready") ).Should().BeEqualTo(HttpStatusCode.OK);
        await ( await HealthCheckHost.StatusAsync(client, healthPort, "/ping") ).Should().BeEqualTo(HttpStatusCode.NotFound);
        await ( await HealthCheckHost.StatusAsync(client, appPort, "/ping") ).Should().BeEqualTo(HttpStatusCode.OK);
        await ( await HealthCheckHost.StatusAsync(client, appPort, "/health") ).Should().BeEqualTo(HttpStatusCode.NotFound);
        await ( await HealthCheckHost.StatusAsync(client, appPort, "/health/live") ).Should().BeEqualTo(HttpStatusCode.NotFound);
    }
}
