using NodaTime;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// General purpose IsoDayOfWeek Serializer
/// </summary>
public class IsoDayOfWeekSerializer : EnumScalarSerializer<IsoDayOfWeek>
{
    /// <summary>
    /// Create a new IsoDayOfWeekSerializer
    /// </summary>
    public IsoDayOfWeekSerializer() : base("IsoDayOfWeek")
    {
    }
}
