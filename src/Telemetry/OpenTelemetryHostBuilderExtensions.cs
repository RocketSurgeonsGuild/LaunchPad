// ReSharper disable once CheckNamespace

using Rocket.Surgery.LaunchPad.Telemetry;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Helper method for working with <see cref="ConventionContextBuilder" />
/// </summary>
[PublicAPI]
public static class OpenTelemetryHostBuilderExtensions
{
    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetryMetrics(this ConventionContextBuilder container, OpenTelemetryConvention @delegate)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        if (@delegate == null)
        {
            throw new ArgumentNullException(nameof(@delegate));
        }

        container.AppendDelegate(@delegate);
        return container;
    }
}
