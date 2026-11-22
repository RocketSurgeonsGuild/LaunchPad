# StringToStructBaseType<TRuntimeType>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1291`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Helpers`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/HotChocolate/Helpers/StringToStructBaseType.cs)
- Inherits: `ScalarType<TRuntimeType, StringValueNode>`

String to struct base type

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

public abstract class StringToStructBaseType<TRuntimeType>
    : HotChocolate.Types.ScalarType<TRuntimeType, HotChocolate.Language.StringValueNode>
    where TRuntimeType : struct
{
    // ...
}
```

## Remarks

Create the base type
