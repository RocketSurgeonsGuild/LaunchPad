using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// General purpose LocalDate Serializer
/// </summary>
public class LocalDateSerializer : NodaTimeStringScalarSerializer<LocalDate>
{
    /// <summary>
    /// Create a new LocalDateSerializer
    /// </summary>
    public LocalDateSerializer() : base(LocalDatePattern.Iso, "LocalDate")
    {
    }
}
