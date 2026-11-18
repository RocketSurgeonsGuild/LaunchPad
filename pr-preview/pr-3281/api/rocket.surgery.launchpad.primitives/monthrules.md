# MonthRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1256`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.f470c0fad9a519f7357ef1c1f00f2210962f5308/src/Primitives/StrongTypes/Month.cs)
- Implements: `IStrongTypeTemplate<int>`

The rules of a month of the year.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class MonthRules
    : SuperStrong.Types.IStrongTypeTemplate<int>
{
    // ...
}
```

## Remarks

1 (January) to 12 (December), as ISO 8601 numbers them.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.primitives/monthrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
