using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents a <see cref="OffsetDateTime" /> in Hot Chocolate
/// </summary>
public class OffsetDateTimeType : StringToStructBaseType<OffsetDateTime>
{
    /// <summary>
    ///     The constructor
    /// </summary>
    public OffsetDateTimeType() : base("OffsetDateTime")
    {
        Description = "A local date and time in a particular calendar system, combined with an offset from UTC.";
    }

    /// <inheritdoc />
    protected override string Serialize(OffsetDateTime baseValue)
    {
        return OffsetDateTimePattern.GeneralIso
                                    .WithCulture(CultureInfo.InvariantCulture)
                                    .Format(baseValue);
    }

    /// <inheritdoc />
    protected override bool TryDeserialize(string str, [NotNullWhen(true)] out OffsetDateTime? output)
    {
        return OffsetDateTimePattern.ExtendedIso
                                    .WithCulture(CultureInfo.InvariantCulture)
                                    .TryParse(str, out output);
    }
}
