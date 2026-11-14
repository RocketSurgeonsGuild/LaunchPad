# Invalid

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1190`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Inherits: [Problem](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/problem.md)
- Implements: `IEquatable<Invalid>`

The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed record Invalid
    : Rocket.Surgery.LaunchPad.Foundation.Problem,
      System.IEquatable<Rocket.Surgery.LaunchPad.Foundation.Invalid>
{
    // ...
}
```

## Constructors

- [Invalid(string?)](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/constructors.md#constructor-string) -- The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

## Properties

- [Errors](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/properties.md#errors) : `IReadOnlyDictionary<string, string[]>` `get; init` -- The errors, by property name.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/methods.md#clone) : [Invalid](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid.md)
- [Deconstruct(string?)](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/methods.md#deconstruct-string)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Problem?)](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/methods.md#equals-problem) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [Equals(Invalid?)](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/methods.md#equals-invalid) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Invalid?, Invalid?)](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/methods.md#op-equality-invalid-invalid) : `bool` `static`
- [op_Inequality(Invalid?, Invalid?)](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/methods.md#op-inequality-invalid-invalid) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.foundation/invalid/methods.md#tostring) : `string` -- Returns a string that represents the current object.
