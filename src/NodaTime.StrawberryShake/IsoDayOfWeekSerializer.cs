using NodaTime;

using Rocket.Surgery.LaunchPad.StrawberryShake;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

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
