using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// General purpose ZonedDateTime Serializer
/// </summary>
public class ZonedDateTimeSerializer : NodaTimeStringScalarSerializer<ZonedDateTime>
{
    /// <summary>
    /// Construct a new ZonedDateTimeSerializer
    /// </summary>
    /// <param name="provider"></param>
    public ZonedDateTimeSerializer(IDateTimeZoneProvider provider) : base(
        ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", provider), "ZonedDateTime"
    )
    {
    }
}
