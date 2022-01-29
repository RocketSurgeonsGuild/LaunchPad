using System.Diagnostics.CodeAnalysis;
using NodaTime;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents a <see cref="DateTimeZone" /> in Hot Chocolate
/// </summary>
public class DateTimeZoneType : StringToClassBaseType<DateTimeZone>
{
    /// <summary>
    ///     The constuctor
    /// </summary>
    public DateTimeZoneType() : base("DateTimeZone")
    {
        Description =
            "Represents a time zone - a mapping between UTC and local time.\n" +
            "A time zone maps UTC instants to local times - or, equivalently, " +
            "to the offset from UTC at any particular instant.";
    }

    /// <inheritdoc />
    protected override string Serialize(DateTimeZone val)
    {
        return val.Id;
    }

    /// <inheritdoc />
    protected override bool TryDeserialize(string str, [NotNullWhen(true)] out DateTimeZone? output)
    {
        var result = DateTimeZoneProviders.Tzdb.GetZoneOrNull(str);
        if (result == null)
        {
            output = null;
            return false;
        }

        output = result;
        return true;
    }
}
