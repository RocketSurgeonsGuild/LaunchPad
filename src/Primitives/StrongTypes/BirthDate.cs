using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a birth date.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             Not in the future. A birth date is a local date, so "today" is the latest date anywhere (UTC+14): someone born today in Kiribati
///             is accepted on a server running on UTC.
///         </item>
///         <item>Not more than 150 years before today, the same bound as <see cref="AgeRules" />.</item>
///         <item>Both bounds move with the clock (<see cref="TimeProvider.System" />), so a value accepted today is checked again on read.</item>
///     </list>
/// </remarks>
public sealed class BirthDateRules : IStrongTypeTemplate<DateOnly>
{
    /// <summary>The most years a birth date may be before today.</summary>
    public const int MaxAgeInYears = 150;

    /// <inheritdoc />
    public static StrongTypeDefinition<DateOnly> Define() => StrongType
                                                           .Define<DateOnly>()
                                                           .Satisfies(static value => value <= LatestToday(), "Value must not be in the future.")
                                                           .Satisfies(
                                                                static value => value >= EarliestToday().AddYears(-MaxAgeInYears),
                                                                $"Value must not be more than {MaxAgeInYears} years ago."
                                                            );

    // The calendar date in the first time zone to reach it (UTC+14) and the last (UTC-12).
    private static DateOnly LatestToday() => DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime.AddHours(14));
    private static DateOnly EarliestToday() => DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime.AddHours(-12));

    private BirthDateRules() { }
}

/// <summary>A person's date of birth, valid under <see cref="BirthDateRules" />.</summary>
[StrongType<DateOnly, BirthDateRules>]
public sealed partial class BirthDate;
