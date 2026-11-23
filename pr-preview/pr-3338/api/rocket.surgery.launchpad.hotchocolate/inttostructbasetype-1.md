# IntToStructBaseType<TRuntimeType>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3338/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1285`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Helpers`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3338-merge.Sha.3cb04ef79b1e670b9474115cbea11b9f679be3f4/src/HotChocolate/Helpers/IntToStructBaseType.cs)
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
