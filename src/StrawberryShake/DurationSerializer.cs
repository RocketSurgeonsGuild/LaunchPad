using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// General purpose Duration Serializer
/// </summary>
public class DurationSerializer : NodaTimeStringScalarSerializer<Duration>
{
    /// <summary>
    /// Create a new DurationSerializer
    /// </summary>
    public DurationSerializer() : base(DurationPattern.CreateWithInvariantCulture("-H:mm:ss.FFFFFFFFF"), "Duration")
    {
    }
}
