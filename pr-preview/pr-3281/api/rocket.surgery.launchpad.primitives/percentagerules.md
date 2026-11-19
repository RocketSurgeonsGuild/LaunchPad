# PercentageRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.65b042458886c034a574f04a60cfe2fa08ef3e1a/src/Primitives/StrongTypes/Percentage.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.primitives/percentagerules/methods.md#define) : `StrongTypeDefinition<decimal>` `static`
