# VoidType

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1177`
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

- [VoidType](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.hotchocolate/voidtype/constructors.md#constructor) -- THe constructor

## Properties

- [RuntimeType](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.hotchocolate/voidtype/properties.md#runtimetype) : `Type` `get` -- The .NET type representation of this scalar.
- [SerializationType](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.hotchocolate/voidtype/properties.md#serializationtype) : `ScalarSerializationType` `get` -- Gets the possible types that this scalar can serialize to.

## Methods

- [CoerceInputLiteral(IValueNode)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#coerceinputliteral-ivaluenode) : `object` -- Coerces a GraphQL literal (AST value node) into a runtime value.
- [CoerceInputValue(JsonElement, IFeatureProvider)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#coerceinputvalue-jsonelement-ifeatureprovider) : `object` -- Coerces an external input value (deserialized JSON) into a runtime value.
- [CoerceOutputValue(object, ResultElement)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#coerceoutputvalue-object-resultelement) -- Coerces a runtime value into an external output representation and writes it to the result.
- [IsValueCompatible(IValueNode)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#isvaluecompatible-ivaluenode) : `bool` -- Checks if the value literal is compatible with this scalar type.
- [IsValueCompatible(JsonElement)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#isvaluecompatible-jsonelement) : `bool` -- Determines if the given `inputValue` is compatible with this type.
- [ValueToLiteral(object)](/LaunchPad/pr-preview/pr-3189/api/rocket.surgery.launchpad.hotchocolate/voidtype/methods.md#valuetoliteral-object) : `IValueNode` -- Converts a runtime value into a GraphQL literal (AST value node). Used for default value representation in SDL and introspection.
