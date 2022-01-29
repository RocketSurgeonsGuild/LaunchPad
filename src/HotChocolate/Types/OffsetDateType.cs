using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents a <see cref="OffsetDate" /> in Hot Chocolate
/// </summary>
public class OffsetDateType : StringToStructBaseType<OffsetDate>
{
    /// <summary>
    ///     The constructor
    /// </summary>
    public OffsetDateType() : base("OffsetDate")
    {
        Description =
            "A combination of a LocalDate and an Offset, to represent a date " +
            "at a specific offset from UTC but without any time-of-day information.";
    }

    /// <inheritdoc />
    protected override string Serialize(OffsetDate baseValue)
    {
        return OffsetDatePattern.GeneralIso
                                .WithCulture(CultureInfo.InvariantCulture)
                                .Format(baseValue);
    }

    /// <inheritdoc />
    protected override bool TryDeserialize(string str, [NotNullWhen(true)] out OffsetDate? output)
    {
        return OffsetDatePattern.GeneralIso
                                .WithCulture(CultureInfo.InvariantCulture)
                                .TryParse(str, out output);
    }
}
