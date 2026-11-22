# RatingRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/Primitives/StrongTypes/Rating.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/ratingrules/methods.md#define) : `StrongTypeDefinition<int>` `static`
