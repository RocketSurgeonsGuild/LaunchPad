# StringToStructBaseType<TRuntimeType>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1275`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Helpers`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3312-merge.Sha.f73ef9701b863c2f6314f4b9416fd99efc8c6135/src/HotChocolate/Helpers/StringToStructBaseType.cs)
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
