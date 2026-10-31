using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// General purpose LocalDateTime Serializer
/// </summary>
public class LocalDateTimeSerializer : NodaTimeStringScalarSerializer<LocalDateTime>
{
    /// <summary>
    /// Create a new LocalDateTimeSerializer
    /// </summary>
    public LocalDateTimeSerializer() : base(LocalDateTimePattern.GeneralIso, "LocalDateTime")
    {
    }
}
