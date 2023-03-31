using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class ZonedDateTimeSerializer : NodaTimeStringScalarSerializer<ZonedDateTime>
{
    public ZonedDateTimeSerializer(IDateTimeZoneProvider provider) : base(
        ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", provider), "ZonedDateTime"
    )
    {
    }
}
