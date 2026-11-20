# QuantityRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3291-merge.Sha.13e74febbd2cc111b9b844ce764f2522bbd95912/src/Primitives/StrongTypes/Quantity.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3291/api/rocket.surgery.launchpad.primitives/quantityrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
