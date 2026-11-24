# StringToClassBaseType<TRuntimeType>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1330`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Helpers`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.c35dcc131245e369cef6bf89daaeca2d2d05743c/src/HotChocolate/Helpers/StringToClassBaseType.cs)
- Inherits: `ScalarType<TRuntimeType, StringValueNode>`

String class base type

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

public abstract class StringToClassBaseType<TRuntimeType>
    : HotChocolate.Types.ScalarType<TRuntimeType, HotChocolate.Language.StringValueNode>
    where TRuntimeType : class
{
    // ...
}
```

## Remarks

Create the base type
