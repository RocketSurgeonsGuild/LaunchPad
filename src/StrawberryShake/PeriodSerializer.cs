using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// General purpose Period Serializer
/// </summary>
public class PeriodSerializer : NodaTimeStringScalarSerializer<Period>
{
    /// <summary>
    /// Create a new PeriodSerializer
    /// </summary>
    public PeriodSerializer() : base(PeriodPattern.NormalizingIso, "Period")
    {
    }
}
