# PrimitiveCollectionExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1328`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.d457558b817469f413055201c091b86e2edbfa9a/src/Primitives/PrimitiveCollectionExtensions.cs)

Creates domain primitives from collections of values.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public static class PrimitiveCollectionExtensions
{
    // ...
}
```

## Methods

- [ToDomainPrimitiveArray(IEnumerable<TValue>)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivecollectionextensions/methods.md#todomainprimitivearray-ienumerable-tvalue) : `TPrimitive[]` `extension` -- Creates a domain primitive from every value, or throws for the first value its rules reject, naming its index.
- [ToDomainPrimitiveArray(ReadOnlySpan<TValue>)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivecollectionextensions/methods.md#todomainprimitivearray-readonlyspan-tvalue) : `TPrimitive[]` `extension` -- Creates a domain primitive from every value, or throws for the first value its rules reject, naming its index.
- [ToDomainPrimitiveList(IEnumerable<TValue>)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitivecollectionextensions/methods.md#todomainprimitivelist-ienumerable-tvalue) : `List<TPrimitive>` `extension` -- Creates a domain primitive from every value, or throws for the first value its rules reject, naming its index.
