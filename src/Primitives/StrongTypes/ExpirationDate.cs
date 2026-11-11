using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of an expiration date: the last day something (a card, a document, a product) is valid.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             Any date except <see cref="DateOnly.MinValue" />, the default of an unset <see cref="DateOnly" />. A past date is accepted:
///             an expired card or document still has an expiration date, and must still be read back from storage. Use
///             <see cref="ExpirationDate.IsExpiredOn" /> to check it against a day.
///         </item>
///     </list>
/// </remarks>
public sealed class ExpirationDateRules : IStrongTypeTemplate<DateOnly>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<DateOnly> Define() => StrongType
                                                           .Define<DateOnly>()
                                                           .IsNot(DateOnly.MinValue);

    private ExpirationDateRules() { }
}

/// <summary>The last day something is valid, valid under <see cref="ExpirationDateRules" />.</summary>
[StrongType<DateOnly, ExpirationDateRules>]
public sealed partial class ExpirationDate
{
    /// <summary>Whether it has expired on <paramref name="date" />: the expiration date is the last valid day, so it expires the day after.</summary>
    public bool IsExpiredOn(DateOnly date) => date > AsPrimitive();
}
