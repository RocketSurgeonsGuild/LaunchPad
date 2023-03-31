using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class OffsetTimeSerializer : NodaTimeStringScalarSerializer<OffsetTime>
{
    public OffsetTimeSerializer() : base(OffsetTimePattern.GeneralIso, "OffsetTime")
    {
    }
}