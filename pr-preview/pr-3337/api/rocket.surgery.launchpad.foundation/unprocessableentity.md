# UnprocessableEntity

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1331`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.8c1925d684bc0cbcfb962e8486f837c5164aa6f2/src/Foundation/Problems.cs)
- Inherits: [Problem](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/problem.md)
- Implements: `IEquatable<UnprocessableEntity>`

The request is well formed but fails validation: what a validator reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed record UnprocessableEntity
    : Rocket.Surgery.LaunchPad.Foundation.Problem,
      System.IEquatable<Rocket.Surgery.LaunchPad.Foundation.UnprocessableEntity>
{
    // ...
}
```

## Constructors

- [UnprocessableEntity(string?)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/constructors.md#constructor-string) -- The request is well formed but fails validation: what a validator reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

## Properties

- [Errors](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/properties.md#errors) : `IReadOnlyDictionary<string, string[]>` `get; init` -- The errors, by property name.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/methods.md#clone) : [UnprocessableEntity](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity.md)
- [Deconstruct(string?)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/methods.md#deconstruct-string)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(Problem?)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/methods.md#equals-problem) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [Equals(UnprocessableEntity?)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/methods.md#equals-unprocessableentity) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(UnprocessableEntity?, UnprocessableEntity?)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/methods.md#op-equality-unprocessableentity-unprocessableentity) : `bool` `static`
- [op_Inequality(UnprocessableEntity?, UnprocessableEntity?)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/methods.md#op-inequality-unprocessableentity-unprocessableentity) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unprocessableentity/methods.md#tostring) : `string` -- Returns a string that represents the current object.
