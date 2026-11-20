# PercentageRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.422`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6f19e0829a4004d537d23cc908a809ddcba9219c/src/Primitives/StrongTypes/Percentage.cs)
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

- [Define](/LaunchPad/api/rocket.surgery.launchpad.primitives/percentagerules/methods.md#define) : `StrongTypeDefinition<decimal>` `static`
