using System.Diagnostics.CodeAnalysis;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents a <see cref="ZonedDateTime" /> in Hot Chocolate
/// </summary>
public class ZonedDateTimeType : StringToStructBaseType<ZonedDateTime>
{
    private static readonly string formatString = "uuuu'-'MM'-'dd'T'HH':'mm':'ss' 'z' 'o<g>";

    /// <summary>
    ///     The constructor
    /// </summary>
    public ZonedDateTimeType()
        : base("ZonedDateTime")
    {
        Description =
            "A LocalDateTime in a specific time zone and with a particular offset to " +
            "distinguish between otherwise-ambiguous instants.\n" +
            "A ZonedDateTime is global, in that it maps to a single Instant.";
    }

    /// <inheritdoc />
    protected override string Serialize(ZonedDateTime baseValue)
    {
        return ZonedDateTimePattern
              .CreateWithInvariantCulture(formatString, DateTimeZoneProviders.Tzdb)
              .Format(baseValue);
    }

    /// <inheritdoc />
    protected override bool TryDeserialize(string str, [NotNullWhen(true)] out ZonedDateTime? output)
    {
        return ZonedDateTimePattern
              .CreateWithInvariantCulture(formatString, DateTimeZoneProviders.Tzdb)
              .TryParse(str, out output);
    }
}
