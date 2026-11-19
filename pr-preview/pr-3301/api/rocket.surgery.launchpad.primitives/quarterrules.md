# QuarterRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1253`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/Primitives/StrongTypes/Quarter.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.primitives/quarterrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
