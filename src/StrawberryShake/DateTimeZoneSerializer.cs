using NodaTime;
using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public class DateTimeZoneSerializer : ScalarSerializer<string, DateTimeZone>
{
    private readonly IDateTimeZoneProvider _provider;

    public DateTimeZoneSerializer(IDateTimeZoneProvider provider) : base("DateTimeZone")
    {
        _provider = provider;
    }

    public override DateTimeZone Parse(string serializedValue)
    {
        return _provider[serializedValue];
    }

    protected override string Format(DateTimeZone runtimeValue)
    {
        return runtimeValue.Id;
    }
}
