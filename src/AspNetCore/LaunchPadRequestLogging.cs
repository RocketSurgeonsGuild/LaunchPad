using Microsoft.AspNetCore.Http;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     The level and the properties LaunchPad gives the request log event that Serilog's request logging writes.
/// </summary>
/// <seealso cref="LaunchPadRequestLoggingExtensions.UseLaunchPadRequestLogging" />
[PublicAPI]
public static class LaunchPadRequestLogging
{
    /// <summary>
    ///     Setup the <see cref="IDiagnosticContext" /> with default values from the request
    /// </summary>
    /// <param name="diagnosticContext"></param>
    /// <param name="httpContext"></param>
    public static void DefaultEnrichDiagnosticContext(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(diagnosticContext);
        ArgumentNullException.ThrowIfNull(httpContext);

        var request = httpContext.Request;

        // Set all the common properties available for every request
        diagnosticContext.Set("Host", request.Host);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);

        // Only set it if available. You're not sending sensitive data in a querystring right?!
        if (request.QueryString.HasValue) diagnosticContext.Set("QueryString", request.QueryString.Value);

        // Set the content-type of the Response at this point
        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

        // Retrieve the IEndpointFeature selected for the request
        var endpoint = httpContext.GetEndpoint();
        if (endpoint is { }) // endpoint != null
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
    }

    /// <summary>
    ///     Gets the default logging level based on the response status code
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="_"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static LogEventLevel DefaultGetLevel(HttpContext ctx, double _, Exception? ex)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        return ex is { }
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
                ? LogEventLevel.Error
                : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                    ? LogEventLevel.Verbose  // Was a health check, use Verbose
                    : LogEventLevel.Information;
    }

    private static bool IsHealthCheckEndpoint(HttpContext ctx)
    {
        var endpoint = ctx.GetEndpoint();
        if (endpoint is { }) // same as !(endpoint is null)
            return string.Equals(endpoint.DisplayName, "Health checks", StringComparison.Ordinal);

        // No endpoint, so not a health check endpoint
        return false;
    }
}
