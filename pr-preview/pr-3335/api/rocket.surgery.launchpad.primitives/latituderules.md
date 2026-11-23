# LatitudeRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3335-merge.Sha.164159712cc333d1205a8448b12272b1815ca0c1/src/Primitives/StrongTypes/Latitude.cs)
- Implements: `IStrongTypeTemplate<double>`

The rules of a latitude in decimal degrees (WGS 84, ISO 6709).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class LatitudeRules
    : SuperStrong.Types.IStrongTypeTemplate<double>
{
    // ...
}
```

## Remarks

A finite number from -90 (the South Pole) to 90 (the North Pole) inclusive. NaN and infinities are rejected.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.primitives/latituderules/methods.md#define) : `StrongTypeDefinition<double>` `static`
