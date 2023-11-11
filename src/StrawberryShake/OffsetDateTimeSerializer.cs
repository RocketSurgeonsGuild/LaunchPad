using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// Create a new OffsetDateTimeSerializer
/// </summary>
public class OffsetDateTimeSerializer : NodaTimeStringScalarSerializer<OffsetDateTime>
{
    /// <summary>
    /// Create a new OffsetDateTimeSerializer
    /// </summary>
    public OffsetDateTimeSerializer() : base(OffsetDateTimePattern.GeneralIso, "OffsetDateTime")
    {
    }
}
