# Invalid

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.331`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net10.0`
- Inherits: [Problem](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem.md)
- Implements: `IEquatable<Invalid>`

The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public sealed record Invalid
    : Rocket.Surgery.LaunchPad.Primitives.Problem,
      System.IEquatable<Rocket.Surgery.LaunchPad.Primitives.Invalid>
{
    // ...
}
```

## Constructors

- [Invalid(string?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/constructors.md#constructor-string) -- The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

## Properties

- [Errors](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/properties.md#errors) : `IReadOnlyDictionary<string, string[]>` `get; init` -- The errors, by property name.

## Methods

- [<Clone>$](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/methods.md#clone) : [Invalid](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid.md)
- [Deconstruct(string?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/methods.md#deconstruct-string)
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Problem?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/methods.md#equals-problem) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [Equals(Invalid?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/methods.md#equals-invalid) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Invalid?, Invalid?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/methods.md#op-equality-invalid-invalid) : `bool` `static`
- [op_Inequality(Invalid?, Invalid?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/methods.md#op-inequality-invalid-invalid) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/invalid/methods.md#tostring) : `string` -- Returns a string that represents the current object.
