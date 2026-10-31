using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// General purpose LocalTime Serializer
/// </summary>
public class LocalTimeSerializer : NodaTimeStringScalarSerializer<LocalTime>
{
    /// <summary>
    /// Create a new LocalTimeSerializer
    /// </summary>
    public LocalTimeSerializer() : base(LocalTimePattern.GeneralIso, "LocalTime")
    {
    }
}
