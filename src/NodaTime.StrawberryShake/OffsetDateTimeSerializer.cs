using NodaTime;
using NodaTime.Text;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// Serializes <see cref="OffsetDateTime" /> as the <c>DateTime</c> scalar of HotChocolate 16
/// </summary>
/// <remarks>
/// HotChocolate 16 renamed the <c>OffsetDateTime</c> scalar to <c>DateTime</c>.
/// </remarks>
public class OffsetDateTimeSerializer : NodaTimeStringScalarSerializer<OffsetDateTime>
{
    /// <summary>
    /// Create a new OffsetDateTimeSerializer
    /// </summary>
    public OffsetDateTimeSerializer() : base(OffsetDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<Z+HH:mm>"), "DateTime")
    {
    }
}
