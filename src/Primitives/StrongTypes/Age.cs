using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a person's age in whole years.
/// </summary>
/// <remarks>
///     0 to 150. The upper bound is above the oldest verified human age (122) and matches <see cref="BirthDateRules" />.
/// </remarks>
public sealed class AgeRules : IStrongTypeTemplate<int>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<int> Define() => StrongType.Define<int>().HasMinValue(0).HasMaxValue(150);

    private AgeRules() { }
}

/// <summary>A person's age in whole years, valid under <see cref="AgeRules" />.</summary>
[StrongType<int, AgeRules>]
public sealed partial class Age;
