# Problem

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1169`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Implements: [IProblemDetailsData](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/iproblemdetailsdata.md), `IEquatable<Problem>`

The base of LaunchPad's standard error cases: what a handler returns, as a case of its result union, instead of throwing. Each transport turns a case into its own error shape (HTTP ProblemDetails, a gRPC status, a GraphQL error) from [IProblemDetailsData](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/iproblemdetailsdata.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public abstract record Problem
    : Rocket.Surgery.LaunchPad.Foundation.IProblemDetailsData,
      System.IEquatable<Rocket.Surgery.LaunchPad.Foundation.Problem>
{
    // ...
}
```

## Remarks

A union may list the derived cases it can return ( `union GetRocketResult(RocketModel, NotFound)`), or [Problem](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem.md) itself to return any of them. See `docs/design/union-types.md`.

## Properties

- [Detail](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/properties.md#detail) : `string?` `get; init` -- A human-readable explanation specific to this occurrence of the problem.
- [Instance](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/properties.md#instance) : `string?` `get; init` -- The instance for the request
- [Link](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/properties.md#link) : `string?` `get; init` -- Request Type
- [Properties](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/properties.md#properties) : `IDictionary<string, object?>` `get; init` -- Additional properties
- [Title](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/properties.md#title) : `string?` `get; init` -- Request title

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/methods.md#clone) : [Problem](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem.md) `abstract`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Problem?)](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/methods.md#equals-problem) : `bool` `virtual` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Problem?, Problem?)](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/methods.md#op-equality-problem-problem) : `bool` `static`
- [op_Inequality(Problem?, Problem?)](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/methods.md#op-inequality-problem-problem) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.foundation/problem/methods.md#tostring) : `string` -- Returns a string that represents the current object.
