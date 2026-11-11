using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a temperature, in degrees Celsius.
/// </summary>
/// <remarks>
///     A finite number no lower than absolute zero, -273.15 °C. NaN and infinities are rejected. The unit is a convention of this type; the rules do not convert.
/// </remarks>
public sealed class TemperatureRules : IStrongTypeTemplate<double>
{
    /// <summary>Absolute zero in degrees Celsius, the lowest temperature.</summary>
    public const double AbsoluteZero = -273.15;

    /// <inheritdoc />
    public static StrongTypeDefinition<double> Define() => StrongType.Define<double>().Satisfies(double.IsFinite, "Value must be a finite number.").HasMinValue(AbsoluteZero);

    private TemperatureRules() { }
}

/// <summary>A temperature in degrees Celsius, valid under <see cref="TemperatureRules" />.</summary>
[StrongType<double, TemperatureRules>]
public sealed partial class Temperature;
