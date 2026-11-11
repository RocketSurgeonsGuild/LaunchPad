using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a weight (strictly, a mass), in kilograms (the SI unit of mass).
/// </summary>
/// <remarks>
///     A finite number, zero or more. NaN and infinities are rejected. The unit is a convention of this type; the rules do not convert.
/// </remarks>
public sealed class WeightRules : IStrongTypeTemplate<double>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<double> Define() => StrongType.Define<double>().Satisfies(double.IsFinite, "Value must be a finite number.").IsPositiveOrZero();

    private WeightRules() { }
}

/// <summary>A weight in kilograms, valid under <see cref="WeightRules" />.</summary>
[StrongType<double, WeightRules>]
public sealed partial class Weight;
