using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class OffsetSerializer : NodaTimeStringScalarSerializer<Offset>
{
    public OffsetSerializer() : base(OffsetPattern.GeneralInvariant, "Offset")
    {
    }
}