using System.Net;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

namespace AspNetCore.Tests.HealthChecks;

public class HealthCheckEndpointTests
{
    [Test]
    public async Task Should_Write_The_Health_Report_As_Json()
    {
        await using var host = await HealthCheckHost.StartAsync(configure: builder => builder.Services.AddHealthChecks().AddCheck("db", () => HealthCheckResult.Healthy("fine")));

        using var response = await host.GetAsync("/health");

        await response.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await response.Content.Headers.ContentType!.MediaType.Should().BeEqualTo("application/json");
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        await document.RootElement.GetProperty("status").GetString().Should().BeEqualTo("Healthy");
        await document.RootElement.GetProperty("entries").GetProperty("db").GetProperty("description").GetString().Should().BeEqualTo("fine");
    }

    // Liveness runs no health check: the application answering is all it says.
    [Test]
    public async Task Should_Answer_Live_Without_Running_The_Health_Checks()
    {
        await using var host = await HealthCheckHost.StartAsync(configure: builder => builder.Services.AddHealthChecks().AddCheck("db", () => HealthCheckResult.Unhealthy()));

        using var live = await host.GetAsync("/health/live");
        using var ready = await host.GetAsync("/health/ready");
        using var report = await host.GetAsync("/health");

        await live.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await ( await live.Content.ReadAsStringAsync() ).Should().BeEqualTo("Healthy");
        await ready.StatusCode.Should().BeEqualTo(HttpStatusCode.ServiceUnavailable);
        await ( await ready.Content.ReadAsStringAsync() ).Should().BeEqualTo("Unhealthy");
        await report.StatusCode.Should().BeEqualTo(HttpStatusCode.ServiceUnavailable);
    }

    [Test]
    public async Task Should_Answer_Degraded_With_200_By_Default()
    {
        await using var host = await HealthCheckHost.StartAsync(configure: builder => builder.Services.AddHealthChecks().AddCheck("db", () => HealthCheckResult.Degraded()));

        using var ready = await host.GetAsync("/health/ready");

        await ready.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await ( await ready.Content.ReadAsStringAsync() ).Should().BeEqualTo("Degraded");
    }

    [Test]
    public async Task Should_Map_The_Endpoints_And_Status_Codes_From_Configuration()
    {
        await using var host = await HealthCheckHost.StartAsync(
            new Dictionary<string, string?>
            {
                ["LaunchPad:HealthChecks:Path"] = "/status",
                ["LaunchPad:HealthChecks:LivePath"] = "alive",
                ["LaunchPad:HealthChecks:ReadyPath"] = "ok",
                ["LaunchPad:HealthChecks:ResultStatusCodes:Degraded"] = "503",
            },
            builder => builder.Services.AddHealthChecks().AddCheck("db", () => HealthCheckResult.Degraded())
        );

        using var report = await host.GetAsync("/status");
        using var live = await host.GetAsync("/status/alive");
        using var ready = await host.GetAsync("/status/ok");
        using var old = await host.GetAsync("/health");

        await report.StatusCode.Should().BeEqualTo(HttpStatusCode.ServiceUnavailable);
        await live.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await ready.StatusCode.Should().BeEqualTo(HttpStatusCode.ServiceUnavailable);
        await old.StatusCode.Should().BeEqualTo(HttpStatusCode.NotFound);
    }

    // A fallback policy makes every endpoint that says nothing require an authenticated user; the health checks still answer probes.
    [Test]
    public async Task Should_Allow_Anonymous_Requests_Under_A_Fallback_Policy()
    {
        await using var host = await HealthCheckHost.StartAsync(
            configure: builder =>
            {
                builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
                builder.Services.AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
            }
        );

        using var ping = await host.GetAsync("/ping");
        using var report = await host.GetAsync("/health");
        using var live = await host.GetAsync("/health/live");
        using var ready = await host.GetAsync("/health/ready");

        await ping.StatusCode.Should().BeEqualTo(HttpStatusCode.Redirect);
        await report.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await live.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await ready.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
    }

    // The in-memory test server binds no port, so an application with a health port answers everything there, and WebApplicationFactory tests keep working.
    [Test]
    public async Task Should_Serve_Everything_On_The_Test_Server_When_A_Port_Is_Set()
    {
        await using var host = await HealthCheckHost.StartAsync(new Dictionary<string, string?> { ["LaunchPad:HealthChecks:Port"] = "8081" });

        using var ping = await host.GetAsync("/ping");
        using var live = await host.GetAsync("/health/live");

        await ping.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
        await live.StatusCode.Should().BeEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task Should_Bind_The_Port_From_Configuration()
    {
        await using var host = await HealthCheckHost.StartAsync(new Dictionary<string, string?> { ["LaunchPad:HealthChecks:Port"] = "8081" });

        await host.Services.GetRequiredService<IOptions<LaunchPadHealthCheckOptions>>().Value.Port.Should().BeEqualTo(8081);
    }

    [Test]
    [Arguments("health")]
    [Arguments("0")]
    [Arguments("70000")]
    public async Task Should_Not_Start_With_A_Port_That_Is_Not_A_Port_Number(string port)
    {
        // Optionary validates the options when the host starts, so a bad port stops the application instead of the first request.
        var exception = await Assert.ThrowsAsync<Exception>(async () => await HealthCheckHost.StartAsync(new Dictionary<string, string?> { ["LaunchPad:HealthChecks:Port"] = port }));
        await exception!.Message.Should().Contain("LaunchPad:HealthChecks:Port");
    }
}
