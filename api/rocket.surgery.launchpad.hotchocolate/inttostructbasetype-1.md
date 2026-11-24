# IntToStructBaseType<TRuntimeType>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-beta.453`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Helpers`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.3675b8bfd84082dc0cd600fe8d8fc9c14e97833a/src/HotChocolate/Helpers/IntToStructBaseType.cs)
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
