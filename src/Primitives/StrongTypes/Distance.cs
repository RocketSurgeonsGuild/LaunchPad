using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a distance, in metres (the SI unit of length).
/// </summary>
/// <remarks>
///     A finite number, zero or more. NaN and infinities are rejected. The unit is a convention of this type; the rules do not convert.
/// </remarks>
public sealed class DistanceRules : IStrongTypeTemplate<double>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<double> Define() => StrongType.Define<double>().Satisfies(double.IsFinite, "Value must be a finite number.").IsPositiveOrZero();

    private DistanceRules() { }
}

/// <summary>A distance in metres, valid under <see cref="DistanceRules" />.</summary>
[StrongType<double, DistanceRules>]
public sealed partial class Distance;
