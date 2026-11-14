# NotFound

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1217`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Inherits: [Problem](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/problem.md)
- Implements: `IEquatable<NotFound>`

The requested entity doesn't exist. HTTP 404, gRPC `NotFound`, GraphQL `NOT_FOUND`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed record NotFound
    : Rocket.Surgery.LaunchPad.Foundation.Problem,
      System.IEquatable<Rocket.Surgery.LaunchPad.Foundation.NotFound>
{
    // ...
}
```

## Constructors

- [NotFound(string?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound/constructors.md#constructor-string) -- The requested entity doesn't exist. HTTP 404, gRPC `NotFound`, GraphQL `NOT_FOUND`.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound/methods.md#clone) : [NotFound](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound.md)
- [Deconstruct(string?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound/methods.md#deconstruct-string)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Problem?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound/methods.md#equals-problem) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [Equals(NotFound?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound/methods.md#equals-notfound) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(NotFound?, NotFound?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound/methods.md#op-equality-notfound-notfound) : `bool` `static`
- [op_Inequality(NotFound?, NotFound?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound/methods.md#op-inequality-notfound-notfound) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/notfound/methods.md#tostring) : `string` -- Returns a string that represents the current object.
