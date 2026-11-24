# QuantityRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3339/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1286`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3339-merge.Sha.f2b0430cc034379f71251d40ca4b57fa907c264f/src/Primitives/StrongTypes/Quantity.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3339/api/rocket.surgery.launchpad.primitives/quantityrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
