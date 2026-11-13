# PercentageRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1181`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
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

- [Define](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.primitives/percentagerules/methods.md#define) : `StrongTypeDefinition<decimal>` `static`
