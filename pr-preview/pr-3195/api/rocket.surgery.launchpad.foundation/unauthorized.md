# Unauthorized

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Inherits: [Problem](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/problem.md)
- Implements: `IEquatable<Unauthorized>`

The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed record Unauthorized
    : Rocket.Surgery.LaunchPad.Foundation.Problem,
      System.IEquatable<Rocket.Surgery.LaunchPad.Foundation.Unauthorized>
{
    // ...
}
```

## Constructors

- [Unauthorized(string?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized/constructors.md#constructor-string) -- The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized/methods.md#clone) : [Unauthorized](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized.md)
- [Deconstruct(string?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized/methods.md#deconstruct-string)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Problem?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized/methods.md#equals-problem) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [Equals(Unauthorized?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized/methods.md#equals-unauthorized) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Unauthorized?, Unauthorized?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized/methods.md#op-equality-unauthorized-unauthorized) : `bool` `static`
- [op_Inequality(Unauthorized?, Unauthorized?)](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized/methods.md#op-inequality-unauthorized-unauthorized) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.foundation/unauthorized/methods.md#tostring) : `string` -- Returns a string that represents the current object.
