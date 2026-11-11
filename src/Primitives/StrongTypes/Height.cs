using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a height, in metres (the SI unit of length).
/// </summary>
/// <remarks>
///     A finite number, zero or more. NaN and infinities are rejected. The unit is a convention of this type; the rules do not convert.
/// </remarks>
public sealed class HeightRules : IStrongTypeTemplate<double>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<double> Define() => StrongType.Define<double>().Satisfies(double.IsFinite, "Value must be a finite number.").IsPositiveOrZero();

    private HeightRules() { }
}

/// <summary>A height in metres, valid under <see cref="HeightRules" />.</summary>
[StrongType<double, HeightRules>]
public sealed partial class Height;
