# Unauthorized

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1165`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net10.0`
- Inherits: [Problem](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/problem.md)
- Implements: `IEquatable<Unauthorized>`

The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public sealed record Unauthorized
    : Rocket.Surgery.LaunchPad.Primitives.Problem,
      System.IEquatable<Rocket.Surgery.LaunchPad.Primitives.Unauthorized>
{
    // ...
}
```

## Constructors

- [Unauthorized(string?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized/constructors.md#constructor-string) -- The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized/methods.md#clone) : [Unauthorized](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized.md)
- [Deconstruct(string?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized/methods.md#deconstruct-string)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Problem?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized/methods.md#equals-problem) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [Equals(Unauthorized?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized/methods.md#equals-unauthorized) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Unauthorized?, Unauthorized?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized/methods.md#op-equality-unauthorized-unauthorized) : `bool` `static`
- [op_Inequality(Unauthorized?, Unauthorized?)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized/methods.md#op-inequality-unauthorized-unauthorized) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/unauthorized/methods.md#tostring) : `string` -- Returns a string that represents the current object.
