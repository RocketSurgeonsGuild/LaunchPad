using System.Globalization;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

/// <summary>
///     Binds <see cref="LaunchPadHealthCheckOptions" /> from the <see cref="LaunchPadHealthCheckOptions.SectionName" /> section by hand, so it
///     needs neither the reflection binder nor the binding source generator.
/// </summary>
internal sealed class HealthCheckOptionsSetup(IServiceProvider serviceProvider) : IConfigureOptions<LaunchPadHealthCheckOptions>
{
    public void Configure(LaunchPadHealthCheckOptions options)
    {
        if (serviceProvider.GetService<IConfiguration>()?.GetSection(LaunchPadHealthCheckOptions.SectionName) is not { } section) return;

        if (section[nameof(LaunchPadHealthCheckOptions.Port)] is { Length: > 0 } port)
        {
            options.Port = int.TryParse(port, NumberStyles.None, CultureInfo.InvariantCulture, out var value) && value is > 0 and <= 65535
                ? value
                : throw new InvalidOperationException(
                    $"{LaunchPadHealthCheckOptions.SectionName}:{nameof(LaunchPadHealthCheckOptions.Port)} must be a port number from 1 to 65535, not '{port}'."
                );
        }

        if (section[nameof(LaunchPadHealthCheckOptions.Path)] is { Length: > 0 } path) options.Path = path;
        if (section[nameof(LaunchPadHealthCheckOptions.LivePath)] is { } livePath) options.LivePath = livePath;
        if (section[nameof(LaunchPadHealthCheckOptions.ReadyPath)] is { } readyPath) options.ReadyPath = readyPath;

        foreach (var child in section.GetSection(nameof(LaunchPadHealthCheckOptions.ResultStatusCodes)).GetChildren())
        {
            if (!Enum.TryParse<HealthStatus>(child.Key, true, out var status)
             || !int.TryParse(child.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var statusCode))
            {
                throw new InvalidOperationException(
                    $"{child.Path} must name a health status (Healthy, Degraded or Unhealthy) and hold a status code, not '{child.Value}'."
                );
            }

            options.ResultStatusCodes[status] = statusCode;
        }
    }
}
