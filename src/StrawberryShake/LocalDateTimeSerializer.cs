using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class LocalDateTimeSerializer : NodaTimeStringScalarSerializer<LocalDateTime>
{
    public LocalDateTimeSerializer() : base(LocalDateTimePattern.GeneralIso, "LocalDateTime")
    {
    }
}