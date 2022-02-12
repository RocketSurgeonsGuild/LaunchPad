using System.Globalization;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;
using Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

/// <summary>
///     Represents an <see cref="Offset" /> in Hot Chocolate
/// </summary>
public class OffsetType : StringToStructBaseType<Offset>
{
    /// <summary>
    ///     The constructor
    /// </summary>
    public OffsetType() : base("Offset")
    {
        Description =
            "An offset from UTC in seconds.\n" +
            "A positive value means that the local time is ahead of UTC (e.g. for Europe); " +
            "a negative value means that the local time is behind UTC (e.g. for America).";
    }

    /// <inheritdoc />
    protected override string Serialize(Offset baseValue)
    {
        return OffsetPattern.GeneralInvariantWithZ
                            .WithCulture(CultureInfo.InvariantCulture)
                            .Format(baseValue);
    }

    /// <inheritdoc />
    protected override bool TryDeserialize(string str, [NotNullWhen(true)] out Offset? output)
    {
        return OffsetPattern.GeneralInvariantWithZ
                            .WithCulture(CultureInfo.InvariantCulture)
                            .TryParse(str, out output);
    }
}
