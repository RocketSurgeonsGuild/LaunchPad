using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a business date: a day business is done on, such as a value date or a trading day.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>Monday to Friday (ISO 8601 days 1 to 5); Saturdays and Sundays are rejected.</item>
///         <item>
///             Public holidays are not checked: they depend on the country, the market and the year. An application that needs them declares
///             its own type with rules that check its calendar.
///         </item>
///     </list>
/// </remarks>
public sealed class BusinessDateRules : IStrongTypeTemplate<DateOnly>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<DateOnly> Define() => StrongType
                                                           .Define<DateOnly>()
                                                           .Satisfies(
                                                                static value => value.DayOfWeek is not ( DayOfWeek.Saturday or DayOfWeek.Sunday ),
                                                                "Value must be a weekday, Monday to Friday."
                                                            );

    private BusinessDateRules() { }
}

/// <summary>A weekday business is done on, valid under <see cref="BusinessDateRules" />.</summary>
[StrongType<DateOnly, BusinessDateRules>]
public sealed partial class BusinessDate;
