using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents a <see cref="LocalTime" /> in Hot Chocolate
/// </summary>
public class LocalTimeType : StringToStructBaseType<LocalTime>
{
    /// <summary>
    ///     The constructor
    /// </summary>
    public LocalTimeType() : base("LocalTime")
    {
        Description = "LocalTime is an immutable struct representing a time of day, with no reference to a particular calendar, time zone or date.";
    }

    /// <inheritdoc />
    protected override string Serialize(LocalTime baseValue)
    {
        return LocalTimePattern.ExtendedIso
                               .WithCulture(CultureInfo.InvariantCulture)
                               .Format(baseValue);
    }

    /// <inheritdoc />
    protected override bool TryDeserialize(string str, [NotNullWhen(true)] out LocalTime? output)
    {
        return LocalTimePattern.ExtendedIso
                               .WithCulture(CultureInfo.InvariantCulture)
                               .TryParse(str, out output);
    }
}
