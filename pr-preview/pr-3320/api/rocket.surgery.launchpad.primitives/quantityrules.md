# QuantityRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1272`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.bb93b2eb5e069135a2caef17855a85e8a18d22af/src/Primitives/StrongTypes/Quantity.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.primitives/quantityrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
