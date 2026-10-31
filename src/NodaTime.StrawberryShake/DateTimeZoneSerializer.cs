using NodaTime;
using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// Create a new DateTimeZoneSerializer
/// </summary>
/// <remarks>
/// Constructor for a new DateTimeZoneSerializer
/// </remarks>
/// <param name="provider"></param>
public class DateTimeZoneSerializer(IDateTimeZoneProvider provider) : ScalarSerializer<string, DateTimeZone>("DateTimeZone")
{
    private readonly IDateTimeZoneProvider _provider = provider;

    /// <inheritdoc />
    public override DateTimeZone Parse(string serializedValue) => _provider[serializedValue];

    /// <inheritdoc />
    protected override string Format(DateTimeZone runtimeValue) => runtimeValue.Id;
}
