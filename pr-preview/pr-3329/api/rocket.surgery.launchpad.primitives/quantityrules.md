# QuantityRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1280`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3329-merge.Sha.a2bf3f292dae7cdf0854fdc32bf25e9839e7110d/src/Primitives/StrongTypes/Quantity.cs)
- Implements: `IStrongTypeTemplate<int>`

The rules of a quantity: a count of items.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class QuantityRules
    : SuperStrong.Types.IStrongTypeTemplate<int>
{
    // ...
}
```

## Remarks

Zero or more whole items.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.primitives/quantityrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
