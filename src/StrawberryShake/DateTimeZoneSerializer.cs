using NodaTime;
using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// Create a new DateTimeZoneSerializer
/// </summary>
public class DateTimeZoneSerializer : ScalarSerializer<string, DateTimeZone>
{
    private readonly IDateTimeZoneProvider _provider;

    /// <summary>
    /// Constructor for a new DateTimeZoneSerializer
    /// </summary>
    /// <param name="provider"></param>
    public DateTimeZoneSerializer(IDateTimeZoneProvider provider) : base("DateTimeZone")
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public override DateTimeZone Parse(string serializedValue)
    {
        return _provider[serializedValue];
    }

    /// <inheritdoc />
    protected override string Format(DateTimeZone runtimeValue)
    {
        return runtimeValue.Id;
    }
}
