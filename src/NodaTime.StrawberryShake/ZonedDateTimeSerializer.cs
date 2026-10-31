using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// General purpose ZonedDateTime Serializer
/// </summary>
/// <param name="provider">The provider that resolves the zone id in the serialized value.</param>
public class ZonedDateTimeSerializer(IDateTimeZoneProvider provider) : NodaTimeStringScalarSerializer<ZonedDateTime>(
    ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", provider), "ZonedDateTime"
    )
{
}
