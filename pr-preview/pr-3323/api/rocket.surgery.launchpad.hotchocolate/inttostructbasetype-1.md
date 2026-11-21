# IntToStructBaseType<TRuntimeType>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1275`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Helpers`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.848d7df41fb82a23ce0738e64978a35c934d7558/src/HotChocolate/Helpers/IntToStructBaseType.cs)
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
