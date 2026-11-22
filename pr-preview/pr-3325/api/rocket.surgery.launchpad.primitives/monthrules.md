# MonthRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1291`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/Primitives/StrongTypes/Month.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/monthrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
