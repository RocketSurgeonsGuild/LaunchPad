# INormalizer<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1292`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3346-merge.Sha.ddf4e8df3db056914fd776aa443a00dc7536dd57/src/Primitives/INormalizer.cs)

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

- [Normalize(T)](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.primitives/inormalizer-1/methods.md#normalize-t) : `T` `static` `abstract` -- Normalizes the value.
