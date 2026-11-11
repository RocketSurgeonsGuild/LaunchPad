# Forbidden

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1167`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net10.0`
- Inherits: [Problem](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/problem.md)
- Implements: `IEquatable<Forbidden>`

The caller may not do this. HTTP 403, gRPC `PermissionDenied`, GraphQL `FORBIDDEN`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public sealed record Forbidden
    : Rocket.Surgery.LaunchPad.Primitives.Problem,
      System.IEquatable<Rocket.Surgery.LaunchPad.Primitives.Forbidden>
{
    // ...
}
```

## Constructors

- [Forbidden(string?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden/constructors.md#constructor-string) -- The caller may not do this. HTTP 403, gRPC `PermissionDenied`, GraphQL `FORBIDDEN`.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden/methods.md#clone) : [Forbidden](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden.md)
- [Deconstruct(string?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden/methods.md#deconstruct-string)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Problem?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden/methods.md#equals-problem) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [Equals(Forbidden?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden/methods.md#equals-forbidden) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Forbidden?, Forbidden?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden/methods.md#op-equality-forbidden-forbidden) : `bool` `static`
- [op_Inequality(Forbidden?, Forbidden?)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden/methods.md#op-inequality-forbidden-forbidden) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/forbidden/methods.md#tostring) : `string` -- Returns a string that represents the current object.
