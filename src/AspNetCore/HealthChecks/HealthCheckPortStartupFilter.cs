using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

/// <summary>
///     Keeps the health check port to the health check endpoints, and the health check endpoints to their port, when
///     <see cref="LaunchPadHealthCheckOptions.Port" /> is set and the server listens on it.
/// </summary>
/// <remarks>
///     <para>It runs at the very front of the pipeline, before any of the application's middleware.</para>
///     <para>
///         When the server doesn't listen on the port (a <c>Listen</c> in code that overrides <c>urls</c>, IIS, HTTP.sys or the in-memory test
///         server), nothing is kept apart: the health check endpoints answer on the application's own addresses, as they do with no port at all.
///         Whether it does is read from the server's bound addresses on the first request, once the server has started.
///     </para>
/// </remarks>
internal sealed class HealthCheckPortStartupFilter(IOptions<LaunchPadHealthCheckOptions> options) : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) =>
        app =>
        {
            if (options.Value is { Port: { } port, Path: var path })
            {
                var serverFeatures = app.ServerFeatures;
                var portBound = new Lazy<bool>(() => IsBound(serverFeatures, port), LazyThreadSafetyMode.PublicationOnly);
                app.Use(
                    (context, nextMiddleware) =>
                    {
                        if (!portBound.Value) return nextMiddleware(context);

                        var onHealthPort = context.Connection.LocalPort == port;
                        var forHealthChecks = context.Request.Path.StartsWithSegments(path);
                        if (onHealthPort == forHealthChecks) return nextMiddleware(context);

                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return Task.CompletedTask;
                    }
                );
            }

            next(app);
        };

    internal static bool IsBound(IFeatureCollection serverFeatures, int port) =>
        serverFeatures.Get<IServerAddressesFeature>()?.Addresses is { } addresses
     && addresses.Any(address => TryGetPort(address) == port);

    private static int? TryGetPort(string address)
    {
        try
        {
            return BindingAddress.Parse(address).Port;
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
