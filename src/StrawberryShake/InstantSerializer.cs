using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class InstantSerializer : NodaTimeStringScalarSerializer<Instant>
{
    public InstantSerializer() : base(InstantPattern.General, "Instant")
    {
    }
}