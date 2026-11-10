# Problem

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.331`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net10.0`
- Implements: [IProblemDetailsData](/LaunchPad/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata.md), `IEquatable<Problem>`

The base of LaunchPad's standard error cases: what a handler returns, as a case of its result union, instead of throwing. Each transport turns a case into its own error shape (HTTP ProblemDetails, a gRPC status, a GraphQL error) from [IProblemDetailsData](/LaunchPad/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public abstract record Problem
    : Rocket.Surgery.LaunchPad.Primitives.IProblemDetailsData,
      System.IEquatable<Rocket.Surgery.LaunchPad.Primitives.Problem>
{
    // ...
}
```

## Remarks

A union may list the derived cases it can return ( `union GetRocketResult(RocketModel, NotFound)`), or [Problem](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem.md) itself to return any of them. See `docs/design/union-types.md`.

## Properties

- [Detail](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/properties.md#detail) : `string?` `get; init` -- A human-readable explanation specific to this occurrence of the problem.
- [Instance](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/properties.md#instance) : `string?` `get; init` -- The instance for the request
- [Link](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/properties.md#link) : `string?` `get; init` -- Request Type
- [Properties](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/properties.md#properties) : `IDictionary<string, object?>` `get; init` -- Additional properties
- [Title](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/properties.md#title) : `string?` `get; init` -- Request title

## Methods

- [<Clone>$](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/methods.md#clone) : [Problem](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem.md) `abstract`
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Problem?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/methods.md#equals-problem) : `bool` `virtual` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Problem?, Problem?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/methods.md#op-equality-problem-problem) : `bool` `static`
- [op_Inequality(Problem?, Problem?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/methods.md#op-inequality-problem-problem) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/problem/methods.md#tostring) : `string` -- Returns a string that represents the current object.
