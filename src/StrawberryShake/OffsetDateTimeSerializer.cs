using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class OffsetDateTimeSerializer : NodaTimeStringScalarSerializer<OffsetDateTime>
{
    public OffsetDateTimeSerializer() : base(OffsetDateTimePattern.GeneralIso, "OffsetDateTime")
    {
    }
}