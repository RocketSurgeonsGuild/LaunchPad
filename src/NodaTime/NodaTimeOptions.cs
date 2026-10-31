using NodaTime.TimeZones;

namespace Rocket.Surgery.LaunchPad.NodaTime;

/// <summary>
///     The options of the NodaTime support, set on the joint context to change how it behaves.
/// </summary>
[PublicAPI]
public class NodaTimeOptions
{
    /// <summary>
    ///     The NodaTime timezone source
    /// </summary>
    public IDateTimeZoneSource DateTimeZoneSource { get; set; } = TzdbDateTimeZoneSource.Default;
}
