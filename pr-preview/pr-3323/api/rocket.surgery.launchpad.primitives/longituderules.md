# LongitudeRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1275`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.848d7df41fb82a23ce0738e64978a35c934d7558/src/Primitives/StrongTypes/Longitude.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.primitives/longituderules/methods.md#define) : `StrongTypeDefinition<double>` `static`
