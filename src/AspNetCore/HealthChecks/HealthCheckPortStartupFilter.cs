using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

/// <summary>
///     Keeps the health check port to the health check endpoints, and the health check endpoints to their port, when
///     <see cref="LaunchPadHealthCheckOptions.Port" /> is set.
/// </summary>
/// <remarks>
///     It runs at the very front of the pipeline, before any of the application's middleware. A request with no local port (the in-memory test
///     server) is let through, so an application with a health port can still be tested with <c>WebApplicationFactory</c>.
/// </remarks>
internal sealed class HealthCheckPortStartupFilter(IOptions<LaunchPadHealthCheckOptions> options) : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) =>
        app =>
        {
            if (options.Value is { Port: { } port, Path: var path })
            {
                app.Use(
                    (context, nextMiddleware) =>
                    {
                        var localPort = context.Connection.LocalPort;
                        var onHealthPort = localPort == port;
                        var forHealthChecks = context.Request.Path.StartsWithSegments(path);
                        if (localPort == 0 || onHealthPort == forHealthChecks) return nextMiddleware(context);

                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return Task.CompletedTask;
                    }
                );
            }

            next(app);
        };
}
