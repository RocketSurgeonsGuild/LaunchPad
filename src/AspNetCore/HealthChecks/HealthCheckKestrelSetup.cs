using System.Globalization;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

/// <summary>
///     Makes Kestrel listen on the health check port as well as on the application's own addresses.
/// </summary>
/// <remarks>
///     <para>
///         Kestrel ignores the <c>urls</c> setting (and <c>ASPNETCORE_HTTP_PORTS</c>) as soon as an endpoint is declared in code or in the
///         <c>Kestrel:Endpoints</c> section, so the port cannot simply be added with <c>ListenAnyIP</c>. Instead:
///     </para>
///     <list type="bullet">
///         <item>
///             when the application declares <c>Kestrel:Endpoints</c>, its addresses already come from there, and the port is added with
///             <c>ListenAnyIP</c> next to them;
///         </item>
///         <item>
///             otherwise the port is appended to the addresses the host would bind (<c>urls</c>, else the HTTP and HTTPS ports, else Kestrel's
///             default <c>http://localhost:5000</c>), and the result is written back to the <c>urls</c> setting.
///         </item>
///     </list>
///     <para>
///         Kestrel's options are created when the server is, after the application has finished its own configuration (<c>UseUrls</c> included)
///         and before the host reads <c>urls</c> to start the server, which is the moment this runs.
///     </para>
/// </remarks>
internal sealed class HealthCheckKestrelSetup(IServiceProvider serviceProvider, IOptions<LaunchPadHealthCheckOptions> options)
    : IConfigureOptions<KestrelServerOptions>
{
    internal const string DefaultAddress = "http://localhost:5000";

    public void Configure(KestrelServerOptions kestrel)
    {
        if (options.Value.Port is not { } port) return;
        if (serviceProvider.GetService<IConfiguration>() is not { } configuration) return;

        if (configuration.GetSection("Kestrel:Endpoints").GetChildren().Any())
        {
            kestrel.ListenAnyIP(port);
            return;
        }

        configuration[WebHostDefaults.ServerUrlsKey] = AppendAddress(configuration, port);
    }

    internal static string AppendAddress(IConfiguration configuration, int port)
    {
        var addresses = ApplicationAddresses(configuration);
        var healthAddress = string.Create(CultureInfo.InvariantCulture, $"http://*:{port}");
        return addresses.Contains(healthAddress, StringComparer.OrdinalIgnoreCase)
            ? string.Join(';', addresses)
            : string.Join(';', [.. addresses, healthAddress]);
    }

    // The same order the web host reads them in when it starts the server.
    private static string[] ApplicationAddresses(IConfiguration configuration)
    {
        if (Split(configuration[WebHostDefaults.ServerUrlsKey]) is { Length: > 0 } urls) return urls;

        string[] ports =
        [
            .. Split(configuration[WebHostDefaults.HttpPortsKey]).Select(port => $"http://*:{port}"),
            .. Split(configuration[WebHostDefaults.HttpsPortsKey]).Select(port => $"https://*:{port}"),
        ];
        return ports is { Length: > 0 } ? ports : [DefaultAddress];
    }

    private static string[] Split(string? value) =>
        value?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
}
