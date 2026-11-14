# Conflict

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1183`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Inherits: [Problem](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/problem.md)
- Implements: `IEquatable<Conflict>`

The request conflicts with the current state. HTTP 409, gRPC `FailedPrecondition`, GraphQL `CONFLICT`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed record Conflict
    : Rocket.Surgery.LaunchPad.Foundation.Problem,
      System.IEquatable<Rocket.Surgery.LaunchPad.Foundation.Conflict>
{
    // ...
}
```

## Constructors

- [Conflict(string?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict/constructors.md#constructor-string) -- The request conflicts with the current state. HTTP 409, gRPC `FailedPrecondition`, GraphQL `CONFLICT`.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict/methods.md#clone) : [Conflict](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict.md)
- [Deconstruct(string?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict/methods.md#deconstruct-string)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Problem?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict/methods.md#equals-problem) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [Equals(Conflict?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict/methods.md#equals-conflict) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Conflict?, Conflict?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict/methods.md#op-equality-conflict-conflict) : `bool` `static`
- [op_Inequality(Conflict?, Conflict?)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict/methods.md#op-inequality-conflict-conflict) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/conflict/methods.md#tostring) : `string` -- Returns a string that represents the current object.
