# LongitudeRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1256`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3305-merge.Sha.f536a04ca4e28fbdc06da7c1e0f968bbeccd9e9a/src/Primitives/StrongTypes/Longitude.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.primitives/longituderules/methods.md#define) : `StrongTypeDefinition<double>` `static`
