# TemperatureRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1275`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3312-merge.Sha.f73ef9701b863c2f6314f4b9416fd99efc8c6135/src/Primitives/StrongTypes/Temperature.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.primitives/temperaturerules/methods.md#define) : `StrongTypeDefinition<double>` `static`

## Fields

- [AbsoluteZero](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.primitives/temperaturerules/fields.md#absolutezero) : `double` `static` -- Absolute zero in degrees Celsius, the lowest temperature.
