using NodaTime;

using Serilog;
using Serilog.Configuration;

namespace Rocket.Surgery.LaunchPad.Serilog;

/// <summary>
///     Adds the Destructure.NodaTimeTypes() extension to <see cref="LoggerConfiguration" />.
/// </summary>
public static class NodaTimeLoggerConfigurationExtensions
{
    /// <summary>
    ///     Enable destructuring of the NodaTime types as the text of their ISO patterns.
    /// </summary>
    /// <param name="configuration">The logger configuration to apply configuration to.</param>
    /// <returns>An object allowing configuration to continue.</returns>
    public static LoggerConfiguration NodaTimeTypes(this LoggerDestructuringConfiguration configuration) =>
        configuration
           .AsScalar<Offset>()
           .Destructure.AsScalar<CalendarSystem>()
           .Destructure.With(new NodaTimeDestructuringPolicy());
}
