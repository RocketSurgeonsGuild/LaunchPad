# PercentageRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1280`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3329-merge.Sha.a2bf3f292dae7cdf0854fdc32bf25e9839e7110d/src/Primitives/StrongTypes/Percentage.cs)
- Implements: `IStrongTypeTemplate<decimal>`

The rules of a percentage.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class PercentageRules
    : SuperStrong.Types.IStrongTypeTemplate<decimal>
{
    // ...
}
```

## Remarks

0 to 100 inclusive, as a number of hundredths: 12.5 is 12.5 %, not 0.125.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.primitives/percentagerules/methods.md#define) : `StrongTypeDefinition<decimal>` `static`
