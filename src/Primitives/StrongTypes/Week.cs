using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of an ISO 8601 week of the year.
/// </summary>
/// <remarks>
///     1 to 53. Whether a year has a week 53 depends on the year (see <see cref="System.Globalization.ISOWeek.GetWeeksInYear(int)" />); a week on its own is not checked against one.
/// </remarks>
public sealed class WeekRules : IStrongTypeTemplate<int>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<int> Define() => StrongType.Define<int>().HasMinValue(1).HasMaxValue(53);

    private WeekRules() { }
}

/// <summary>An ISO 8601 week of the year, valid under <see cref="WeekRules" />.</summary>
[StrongType<int, WeekRules>]
public sealed partial class Week;
