using NodaTime;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class IsoDayOfWeekSerializer : EnumScalarSerializer<IsoDayOfWeek>
{
    public IsoDayOfWeekSerializer() : base("IsoDayOfWeek")
    {
    }
}