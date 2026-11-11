# TemperatureRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.342`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongTypeTemplate<double>`

The rules of a temperature, in degrees Celsius.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class TemperatureRules
    : SuperStrong.Types.IStrongTypeTemplate<double>
{
    // ...
}
```

## Remarks

A finite number no lower than absolute zero, -273.15 °C. NaN and infinities are rejected. The unit is a convention of this type; the rules do not convert.

## Methods

- [Define](/LaunchPad/api/rocket.surgery.launchpad.primitives/temperaturerules/methods.md#define) : `StrongTypeDefinition<double>` `static`

## Fields

- [AbsoluteZero](/LaunchPad/api/rocket.surgery.launchpad.primitives/temperaturerules/fields.md#absolutezero) : `double` `static` -- Absolute zero in degrees Celsius, the lowest temperature.
