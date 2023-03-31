using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class OffsetDateSerializer : NodaTimeStringScalarSerializer<OffsetDate>
{
    public OffsetDateSerializer() : base(OffsetDatePattern.GeneralIso, "OffsetDate")
    {
    }
}