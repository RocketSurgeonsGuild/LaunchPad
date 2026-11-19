# QuarterRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3292/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1245`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3292-merge.Sha.a3b1ee2389c613a51b34add57c7d3d20cf877d5b/src/Primitives/StrongTypes/Quarter.cs)
- Implements: `IStrongTypeTemplate<int>`

The rules of a quarter of the year.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class QuarterRules
    : SuperStrong.Types.IStrongTypeTemplate<int>
{
    // ...
}
```

## Remarks

1 to 4: the first quarter is January to March.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3292/api/rocket.surgery.launchpad.primitives/quarterrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
