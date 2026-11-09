# StringToStructBaseType<TRuntimeType>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3155/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1168`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Helpers`
- Target framework: `net10.0`
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

## Methods

- [ParseResult(object?)](/LaunchPad/pr-preview/pr-3155/api/rocket.surgery.launchpad.hotchocolate/stringtostructbasetype-1/methods.md#parseresult-object) : `IValueNode` -- Parses a result value of this scalar into a GraphQL value syntax representation.
- [TryDeserialize(object?, object?)](/LaunchPad/pr-preview/pr-3155/api/rocket.surgery.launchpad.hotchocolate/stringtostructbasetype-1/methods.md#trydeserialize-object-object) : `bool` -- Tries to deserializes the value from the output format to the .NET value representation.
- [TrySerialize(object?, object?)](/LaunchPad/pr-preview/pr-3155/api/rocket.surgery.launchpad.hotchocolate/stringtostructbasetype-1/methods.md#tryserialize-object-object) : `bool` -- Tries to serializes the .NET value representation to the output format.
