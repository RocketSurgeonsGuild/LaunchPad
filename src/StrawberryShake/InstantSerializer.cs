using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// General purpose Instant Serializer
/// </summary>
public class InstantSerializer : NodaTimeStringScalarSerializer<Instant>
{
    /// <summary>
    /// Create a new InstantSerializer
    /// </summary>
    public InstantSerializer() : base(InstantPattern.General, "Instant")
    {
    }
}
