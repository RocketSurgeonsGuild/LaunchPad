using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;

using TUnit.Core.Interfaces;

namespace Sample.AppHost.Tests;

/// <summary>
///     Sample.AppHost, started once for the test session with the resources the tests call: Keycloak with the samples' realm, and the
///     Minimal API and GraphQL samples. The other samples are left out, as each is a build and a process of its own.
/// </summary>
public sealed class AppHostFixture : IAsyncInitializer, IAsyncDisposable
{
    private static readonly string[] Resources = ["keycloak", "minimal", "graphql"];

    private static readonly string[] LeftOut =
        ["restful", "classic-restful", "grpc", "pages", "blazor-server", "worker", "command", "blazor-wasm", "blazor-wasm-gateway"];

    private DistributedApplication? _app;

    /// <summary>The address of the Keycloak realm.</summary>
    public Uri Realm { get; private set; } = null!;

    /// <summary>A client that trusts the development certificates the AppHost's resources serve.</summary>
    public HttpClient Http { get; } =
        new(new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator, AllowAutoRedirect = false });

    public async Task InitializeAsync()
    {
        if (!Docker.IsAvailable) return;

        // Any type of the AppHost names its assembly, whose entry point Aspire's testing host runs.
        var builder = await DistributedApplicationTestingBuilder.CreateAsync(typeof(SampleResourceExtensions));
        // Only the samples that are named: a sample has resources of its own beside it (the one that builds it), which stay.
        foreach (var resource in builder.Resources.Where(r => LeftOut.Contains(r.Name, StringComparer.Ordinal)).ToArray())
        {
            builder.Resources.Remove(resource);
        }

        _app = await builder.BuildAsync();
        // Keycloak's image is pulled and each sample built before it starts.
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        await _app.StartAsync(timeout.Token);
        foreach (var resource in Resources)
        {
            await _app.ResourceNotifications.WaitForResourceHealthyAsync(resource, timeout.Token);
        }

        Realm = new(_app.GetEndpoint("keycloak", "http"), "/realms/launchpad");
    }

    /// <summary>The HTTPS address of a sample.</summary>
    /// <param name="resource">The sample's resource name.</param>
    public Uri Endpoint(string resource) => _app!.GetEndpoint(resource, "https");

    /// <summary>An access token of the realm's service client (client credentials), with its default scopes and the ones asked for.</summary>
    /// <param name="scopes">The optional scopes to ask for.</param>
    public async Task<string> ServiceToken(params string[] scopes)
    {
        var form = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = "sample-service",
            ["client_secret"] = "sample-service-local-secret",
        };
        if (scopes.Length > 0) form["scope"] = string.Join(' ', scopes);
        using var response = await Http.PostAsync(new Uri($"{Realm}/protocol/openid-connect/token"), new FormUrlEncodedContent(form));
        response.EnsureSuccessStatusCode();
        return JsonNode.Parse(await response.Content.ReadAsStringAsync())!["access_token"]!.GetValue<string>();
    }

    /// <summary>Sends a request to a sample, with a token when one is given.</summary>
    public async Task<HttpResponseMessage> Send(string resource, HttpMethod method, string path, string? token = null, HttpContent? content = null)
    {
        using var request = new HttpRequestMessage(method, new Uri(Endpoint(resource), path)) { Content = content };
        if (token is { }) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await Http.SendAsync(request);
    }

    public async ValueTask DisposeAsync()
    {
        Http.Dispose();
        if (_app is { }) await _app.DisposeAsync();
    }
}

/// <summary>Whether a container runtime answers: Keycloak runs in one.</summary>
internal static class Docker
{
    public static bool IsAvailable { get; } = Probe();

    private static bool Probe()
    {
        try
        {
            using var process = Process.Start(
                new ProcessStartInfo("docker", "info") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false }
            );
            if (process is null) return false;
            process.StandardOutput.ReadToEnd();
            return process.WaitForExit(15000) && process.ExitCode == 0;
        }
        catch (Exception exception) when (exception is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return false;
        }
    }
}

/// <summary>Skips a test where there is no container runtime to run Keycloak in.</summary>
public sealed class RequiresDockerAttribute() : SkipAttribute("Docker is not available, and Keycloak runs in a container.")
{
    public override Task<bool> ShouldSkip(TestRegisteredContext context) => Task.FromResult(!Docker.IsAvailable);
}
