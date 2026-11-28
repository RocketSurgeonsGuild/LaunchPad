# INormalizer<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1300`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.52b728c7d35f7e5541d5238c567b2b39c033d3c7/src/Primitives/INormalizer.cs)

Normalizes a value before a domain primitive validates it, for `[Normalize<TNormalizer>]`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public interface INormalizer<T>
{
    // ...
}
```

## Type Parameters

- `T` -- The value.

## Methods

- [Normalize(T)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/inormalizer-1/methods.md#normalize-t) : `T` `static` `abstract` -- Normalizes the value.
