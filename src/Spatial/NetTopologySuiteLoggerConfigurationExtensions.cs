using Serilog;
using Serilog.Configuration;

namespace Rocket.Surgery.LaunchPad.Spatial;

/// <summary>
///     Adds the Destructure.NetTopologySuiteTypes() extension to <see cref="LoggerConfiguration" />.
/// </summary>
public static class NetTopologySuiteLoggerConfigurationExtensions
{
    /// <summary>
    ///     Enable destructuring of JSON.NET dynamic objects.
    /// </summary>
    /// <param name="configuration">The logger configuration to apply configuration to.</param>
    /// <returns>An object allowing configuration to continue.</returns>
    public static LoggerConfiguration NetTopologySuiteTypes(this LoggerDestructuringConfiguration configuration)
    {
        return configuration.With<NetTopologySuiteDestructuringPolicy>();
    }
}
