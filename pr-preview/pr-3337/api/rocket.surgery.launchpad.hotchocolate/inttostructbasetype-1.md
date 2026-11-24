# IntToStructBaseType<TRuntimeType>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1328`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Helpers`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.d457558b817469f413055201c091b86e2edbfa9a/src/HotChocolate/Helpers/IntToStructBaseType.cs)
- Inherits: `ScalarType<TRuntimeType, IntValueNode>`

Additional struct types

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

public abstract class IntToStructBaseType<TRuntimeType>
    : HotChocolate.Types.ScalarType<TRuntimeType, HotChocolate.Language.IntValueNode>
    where TRuntimeType : struct
{
    // ...
}
```

## Remarks

Create the base type
