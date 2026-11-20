# MonthRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1254`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3303-merge.Sha.af4f47555b5ed577aca4ba95e6332df625343e98/src/Primitives/StrongTypes/Month.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.primitives/monthrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
