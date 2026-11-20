# LongitudeRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3291-merge.Sha.13e74febbd2cc111b9b844ce764f2522bbd95912/src/Primitives/StrongTypes/Longitude.cs)
- Implements: `IStrongTypeTemplate<double>`

The rules of a longitude in decimal degrees (WGS 84, ISO 6709).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class LongitudeRules
    : SuperStrong.Types.IStrongTypeTemplate<double>
{
    // ...
}
```

## Remarks

A finite number from -180 to 180 inclusive; both ends name the antimeridian, as ISO 6709 and GeoJSON allow. NaN and infinities are rejected.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.primitives/longituderules/methods.md#define) : `StrongTypeDefinition<double>` `static`
