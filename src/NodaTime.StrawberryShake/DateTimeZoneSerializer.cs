using NodaTime;
using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// General purpose DateTimeZone Serializer, which writes the zone id
/// </summary>
/// <param name="provider">The provider that resolves a zone id to its zone.</param>
public class DateTimeZoneSerializer(IDateTimeZoneProvider provider) : ScalarSerializer<string, DateTimeZone>("DateTimeZone")
{
    private readonly IDateTimeZoneProvider _provider = provider;

    /// <inheritdoc />
    public override DateTimeZone Parse(string serializedValue) => _provider[serializedValue];

    /// <inheritdoc />
    protected override string Format(DateTimeZone runtimeValue) => runtimeValue.Id;
}
