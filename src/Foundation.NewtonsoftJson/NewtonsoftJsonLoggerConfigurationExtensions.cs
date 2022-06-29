using Serilog;
using Serilog.Configuration;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Adds the Destructure.NewtonsoftJsonTypes() extension to <see cref="LoggerConfiguration" />.
/// </summary>
public static class NewtonsoftJsonLoggerConfigurationExtensions
{
    /// <summary>
    ///     Enable destructuring of JSON.NET dynamic objects.
    /// </summary>
    /// <param name="configuration">The logger configuration to apply configuration to.</param>
    /// <returns>An object allowing configuration to continue.</returns>
    public static LoggerConfiguration NewtonsoftJsonTypes(this LoggerDestructuringConfiguration configuration)
    {
        return configuration.With<NewtonsoftJsonDestructuringPolicy>();
    }
}
