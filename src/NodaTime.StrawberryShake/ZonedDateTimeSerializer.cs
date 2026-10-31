using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// General purpose ZonedDateTime Serializer
/// </summary>
/// <remarks>
/// Construct a new ZonedDateTimeSerializer
/// </remarks>
/// <param name="provider"></param>
public class ZonedDateTimeSerializer(IDateTimeZoneProvider provider) : NodaTimeStringScalarSerializer<ZonedDateTime>(
    ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", provider), "ZonedDateTime"
    )
{
}
