using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class LocalTimeSerializer : NodaTimeStringScalarSerializer<LocalTime>
{
    public LocalTimeSerializer() : base(LocalTimePattern.GeneralIso, "LocalTime")
    {
    }
}