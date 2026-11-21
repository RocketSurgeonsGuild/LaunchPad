# RatingRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.434`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.dfbe8776933590a8ca1efba66f9af5c4929d4975/src/Primitives/StrongTypes/Rating.cs)
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

- [Define](/LaunchPad/api/rocket.surgery.launchpad.primitives/ratingrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
