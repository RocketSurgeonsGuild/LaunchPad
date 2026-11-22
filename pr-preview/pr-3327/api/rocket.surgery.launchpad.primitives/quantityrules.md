# QuantityRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/Primitives/StrongTypes/Quantity.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.primitives/quantityrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
