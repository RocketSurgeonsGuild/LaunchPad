using NodaTime.TimeZones;

namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     Common foundation options
/// </summary>
[PublicAPI]
public class PrimitiveOptions
{
    /// <summary>
    ///     The NodaTime timezone source
    /// </summary>
    public IDateTimeZoneSource DateTimeZoneSource { get; set; } = TzdbDateTimeZoneSource.Default;
}
