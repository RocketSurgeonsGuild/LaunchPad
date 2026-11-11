# StrongTypeDescription<TStrongType, TPrimitive>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.342`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Inherits: [StrongTypeDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription.md)

The description of one strong type, which knows the strong type and its value as generic arguments.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class StrongTypeDescription<TStrongType, TPrimitive>
    : Rocket.Surgery.LaunchPad.StrongTypes.StrongTypeDescription
    where TStrongType : class, IStrongType<TStrongType, TPrimitive>
    where TPrimitive : notnull
{
    // ...
}
```

## Type Parameters

- `TPrimitive` -- Its value.
- `TStrongType` -- The strong type.

## Properties

- [PrimitiveType](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription-2/properties.md#primitivetype) : `Type` `get` -- The value the strong type wraps, which is what it is written as.
- [Type](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription-2/properties.md#type) : `Type` `get` -- The strong type.

## Methods

- [Accept(IStrongTypeVisitor<TResult>)](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription-2/methods.md#accept-istrongtypevisitor-tresult) : `TResult` -- Hands the strong type and its value to `visitor` as generic arguments.
