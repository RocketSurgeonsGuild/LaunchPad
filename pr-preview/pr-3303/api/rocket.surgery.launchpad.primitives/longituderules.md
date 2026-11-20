# LongitudeRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1254`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3303-merge.Sha.af4f47555b5ed577aca4ba95e6332df625343e98/src/Primitives/StrongTypes/Longitude.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.primitives/longituderules/methods.md#define) : `StrongTypeDefinition<double>` `static`
