# RatingRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1286`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3333-merge.Sha.cfa1160619bf896bca17572106e5b103a6adcde3/src/Primitives/StrongTypes/Rating.cs)
- Implements: `IStrongTypeTemplate<int>`

The rules of a rating, such as a number of stars in a review.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class RatingRules
    : SuperStrong.Types.IStrongTypeTemplate<int>
{
    // ...
}
```

## Remarks

A whole number from 1 to 5: the default worst and best ratings of schema.org's `Rating`. An application with another scale declares its own type with its own rules.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.primitives/ratingrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
