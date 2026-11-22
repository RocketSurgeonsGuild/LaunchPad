# TemperatureRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.fb40f4ae51769f679795c3f90ed1c13a62505cf5/src/Primitives/StrongTypes/Temperature.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/temperaturerules/methods.md#define) : `StrongTypeDefinition<double>` `static`

## Fields

- [AbsoluteZero](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/temperaturerules/fields.md#absolutezero) : `double` `static` -- Absolute zero in degrees Celsius, the lowest temperature.
