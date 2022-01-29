using System.Globalization;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents a <see cref="Duration" /> in Hot Chocolate
/// </summary>
public class DurationType : StringToStructBaseType<Duration>
{
    /// <summary>
    ///     The constructor
    /// </summary>
    public DurationType() : base("Duration")
    {
        Description = "Represents a fixed (and calendar-independent) length of time.";
    }

    /// <inheritdoc />
    protected override string Serialize(Duration baseValue)
    {
        return DurationPattern.Roundtrip
                              .WithCulture(CultureInfo.InvariantCulture)
                              .Format(baseValue);
    }

    /// <inheritdoc />
    protected override bool TryDeserialize(string str, [NotNullWhen(true)] out Duration? output)
    {
        return DurationPattern.Roundtrip
                              .WithCulture(CultureInfo.InvariantCulture)
                              .TryParse(str, out output);
    }
}
