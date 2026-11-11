using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a rating, such as a number of stars in a review.
/// </summary>
/// <remarks>
///     A whole number from 1 to 5: the default worst and best ratings of schema.org's <c>Rating</c>. An application with another scale declares its own type with its own rules.
/// </remarks>
public sealed class RatingRules : IStrongTypeTemplate<int>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<int> Define() => StrongType.Define<int>().HasMinValue(1).HasMaxValue(5);

    private RatingRules() { }
}

/// <summary>A rating from 1 to 5, valid under <see cref="RatingRules" />.</summary>
[StrongType<int, RatingRules>]
public sealed partial class Rating;
