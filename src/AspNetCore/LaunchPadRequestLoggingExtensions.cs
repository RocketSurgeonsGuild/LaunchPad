using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     Adds Serilog's request logging with LaunchPad's defaults.
/// </summary>
[PublicAPI]
public static class LaunchPadRequestLoggingExtensions
{
    /// <summary>
    ///     Default request logging for serilog with launchpad
    /// </summary>
    /// <remarks>
    ///     The request event is written to the Serilog <see cref="ILogger" /> in the container (Joinery.Dovetail.Serilog registers it), or to the
    ///     static <see cref="Log.Logger" /> when there is none. Its level comes from <see cref="LaunchPadRequestLogging.DefaultGetLevel" /> and
    ///     its properties from <see cref="LaunchPadRequestLogging.DefaultEnrichDiagnosticContext" />.
    /// </remarks>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseLaunchPadRequestLogging(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseSerilogRequestLogging(
            x =>
            {
                x.GetLevel = LaunchPadRequestLogging.DefaultGetLevel;
                x.EnrichDiagnosticContext = LaunchPadRequestLogging.DefaultEnrichDiagnosticContext;
                x.Logger = app.ApplicationServices.GetService<ILogger>();
            }
        );
    }
}
