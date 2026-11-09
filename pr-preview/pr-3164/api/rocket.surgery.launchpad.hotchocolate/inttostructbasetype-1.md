# IntToStructBaseType<TRuntimeType>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1170`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Helpers`
- Target framework: `net10.0`
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

## Methods

- [ParseResult(object?)](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.hotchocolate/inttostructbasetype-1/methods.md#parseresult-object) : `IValueNode` -- Parses a result value of this scalar into a GraphQL value syntax representation.
- [TryDeserialize(object?, object?)](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.hotchocolate/inttostructbasetype-1/methods.md#trydeserialize-object-object) : `bool` -- Tries to deserializes the value from the output format to the .NET value representation.
- [TrySerialize(object?, object?)](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.hotchocolate/inttostructbasetype-1/methods.md#tryserialize-object-object) : `bool` -- Tries to serializes the .NET value representation to the output format.
