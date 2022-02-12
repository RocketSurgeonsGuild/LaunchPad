using System.Globalization;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents a <see cref="LocalDateTime" /> in HotChocolate
/// </summary>
public class LocalDateTimeType : StringToStructBaseType<LocalDateTime>
{
    /// <summary>
    ///     The constructor
    /// </summary>
    public LocalDateTimeType() : base("LocalDateTime")
    {
        Description = "A date and time in a particular calendar system.";
    }

    /// <inheritdoc />
    protected override string Serialize(LocalDateTime baseValue)
    {
        return LocalDateTimePattern.ExtendedIso
                                   .WithCulture(CultureInfo.InvariantCulture)
                                   .Format(baseValue);
    }

    /// <inheritdoc />
    protected override bool TryDeserialize(string str, [NotNullWhen(true)] out LocalDateTime? output)
    {
        return LocalDateTimePattern.ExtendedIso
                                   .WithCulture(CultureInfo.InvariantCulture)
                                   .TryParse(str, out output);
    }
}
