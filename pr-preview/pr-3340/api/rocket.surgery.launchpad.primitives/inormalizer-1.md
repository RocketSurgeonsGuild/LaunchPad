# INormalizer<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1300`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3340-merge.Sha.1fd0124e0c52f2b8f0975bb6854b707a6a798292/src/Primitives/INormalizer.cs)

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

- [Normalize(T)](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.primitives/inormalizer-1/methods.md#normalize-t) : `T` `static` `abstract` -- Normalizes the value.
