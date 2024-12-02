using NodaTime;
using Serilog;
using Serilog.Configuration;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Adds the Destructure.NewtonsoftJsonTypes() extension to <see cref="LoggerConfiguration" />.
/// </summary>
public static class NodaTimeLoggerConfigurationExtensions
{
    /// <summary>
    ///     Enable destructuring of JSON.NET dynamic objects.
    /// </summary>
    /// <param name="configuration">The logger configuration to apply configuration to.</param>
    /// <returns>An object allowing configuration to continue.</returns>
    public static LoggerConfiguration NodaTimeTypes(this LoggerDestructuringConfiguration configuration) =>
        configuration
           .AsScalar<Offset>()
           .Destructure.AsScalar<CalendarSystem>()
           .Destructure.With(new NodaTimeDestructuringPolicy());
}
