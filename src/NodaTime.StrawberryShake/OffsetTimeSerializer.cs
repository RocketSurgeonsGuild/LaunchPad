using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// General purpose OffsetDateTime Serializer
/// </summary>
public class OffsetTimeSerializer : NodaTimeStringScalarSerializer<OffsetTime>
{
    /// <summary>
    /// Create a new OffsetDateTimeSerializer
    /// </summary>
    public OffsetTimeSerializer() : base(OffsetTimePattern.GeneralIso, "OffsetTime")
    {
    }
}
