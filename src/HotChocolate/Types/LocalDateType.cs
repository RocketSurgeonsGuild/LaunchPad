using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents a <see cref="LocalDate" /> in Hot Chocolate
/// </summary>
public class LocalDateType : StringToStructBaseType<LocalDate>
{
    /// <summary>
    ///     The constructor
    /// </summary>
    public LocalDateType() : base("LocalDate")
    {
        Description =
            "LocalDate is an immutable struct representing a date " +
            "within the calendar, with no reference to a particular " +
            "time zone or time of day.";
    }

    /// <inheritdoc />
    protected override string Serialize(LocalDate baseValue)
    {
        return LocalDatePattern.Iso
                               .WithCulture(CultureInfo.InvariantCulture)
                               .Format(baseValue);
    }

    /// <inheritdoc />
    protected override bool TryDeserialize(string str, [NotNullWhen(true)] out LocalDate? output)
    {
        return LocalDatePattern.Iso
                               .WithCulture(CultureInfo.InvariantCulture)
                               .TryParse(str, out output);
    }
}
