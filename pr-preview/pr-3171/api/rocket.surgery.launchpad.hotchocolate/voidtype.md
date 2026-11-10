# VoidType

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1165`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Types`
- Target framework: `net10.0`
- Inherits: `ScalarType`

Represents a void or unit type in Hot Chocolate

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Types;

public class VoidType
    : HotChocolate.Types.ScalarType
{
    // ...
}
```

## Constructors

- [VoidType](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.hotchocolate/voidtype/constructors.md#constructor) -- THe constructor

## Properties

- [RuntimeType](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.hotchocolate/voidtype/properties.md#runtimetype) : `Type` `get` -- The .NET type representation of this scalar.

## Methods

- [IsInstanceOfType(IValueNode)](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#isinstanceoftype-ivaluenode) : `bool` -- Defines if the specified `valueSyntax` can be parsed by this scalar.
- [ParseLiteral(IValueNode)](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#parseliteral-ivaluenode) : `object?` -- Parses the specified `valueSyntax` to the .NET representation of this type.
- [ParseResult(object?)](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#parseresult-object) : `IValueNode` -- Parses a result value of this scalar into a GraphQL value syntax representation.
- [ParseValue(object?)](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#parsevalue-object) : `IValueNode` -- Parses the .NET value representation to a value literal.
- [TryDeserialize(object?, object?)](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#trydeserialize-object-object) : `bool` -- Tries to deserializes the value from the output format to the .NET value representation.
- [TrySerialize(object?, object?)](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#tryserialize-object-object) : `bool` -- Tries to serializes the .NET value representation to the output format.
