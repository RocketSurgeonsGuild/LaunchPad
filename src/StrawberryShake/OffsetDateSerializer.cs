using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// General purpose OffsetDate Serializer
/// </summary>
public class OffsetDateSerializer : NodaTimeStringScalarSerializer<OffsetDate>
{
    /// <summary>
    /// Create a new OffsetDateSerializer
    /// </summary>
    public OffsetDateSerializer() : base(OffsetDatePattern.GeneralIso, "OffsetDate")
    {
    }
}
