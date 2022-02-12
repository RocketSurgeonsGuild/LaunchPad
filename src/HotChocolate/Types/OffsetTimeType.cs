using System.Globalization;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents a <see cref="OffsetTime" /> in Hot Chocolate
/// </summary>
public class OffsetTimeType : StringToStructBaseType<OffsetTime>
{
    /// <summary>
    ///     The constructor
    /// </summary>
    public OffsetTimeType() : base("OffsetTime")
    {
        Description =
            "A combination of a LocalTime and an Offset, " +
            "to represent a time-of-day at a specific offset from UTC " +
            "but without any date information.";
    }

    /// <inheritdoc />
    protected override string Serialize(OffsetTime baseValue)
    {
        return OffsetTimePattern.GeneralIso
                                .WithCulture(CultureInfo.InvariantCulture)
                                .Format(baseValue);
    }

    /// <inheritdoc />
    protected override bool TryDeserialize(string str, [NotNullWhen(true)] out OffsetTime? output)
    {
        return OffsetTimePattern.GeneralIso
                                .WithCulture(CultureInfo.InvariantCulture)
                                .TryParse(str, out output);
    }
}
