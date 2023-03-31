using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class LocalDateSerializer : NodaTimeStringScalarSerializer<LocalDate>
{
    public LocalDateSerializer() : base(LocalDatePattern.Iso, "LocalDate")
    {
    }
}