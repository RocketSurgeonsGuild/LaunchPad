using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class PeriodSerializer : NodaTimeStringScalarSerializer<Period>
{
    public PeriodSerializer() : base(PeriodPattern.NormalizingIso, "Period")
    {
    }
}