using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class DurationSerializer : NodaTimeStringScalarSerializer<Duration>
{
    public DurationSerializer() : base(DurationPattern.CreateWithInvariantCulture("-H:mm:ss.FFFFFFFFF"), "Duration")
    {
    }
}