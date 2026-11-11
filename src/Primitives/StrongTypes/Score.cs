using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a score, such as a test or quality score.
/// </summary>
/// <remarks>
///     A whole number from 0 to 100. No standard defines a score; an application with another scale declares its own type with its own rules.
/// </remarks>
public sealed class ScoreRules : IStrongTypeTemplate<int>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<int> Define() => StrongType.Define<int>().HasMinValue(0).HasMaxValue(100);

    private ScoreRules() { }
}

/// <summary>A score from 0 to 100, valid under <see cref="ScoreRules" />.</summary>
[StrongType<int, ScoreRules>]
public sealed partial class Score;
