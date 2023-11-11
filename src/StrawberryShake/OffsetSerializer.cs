using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// General purpose Offset Serializer
/// </summary>
public class OffsetSerializer : NodaTimeStringScalarSerializer<Offset>
{
    /// <summary>
    /// Create a new OffsetSerializer
    /// </summary>
    public OffsetSerializer() : base(OffsetPattern.GeneralInvariant, "Offset")
    {
    }
}
