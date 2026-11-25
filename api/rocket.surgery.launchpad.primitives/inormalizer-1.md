# INormalizer<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.454`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.752d0f02a7c5d0592531a44cd02f98c0fc107a63/src/Primitives/INormalizer.cs)

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

- [Normalize(T)](/LaunchPad/api/rocket.surgery.launchpad.primitives/inormalizer-1/methods.md#normalize-t) : `T` `static` `abstract` -- Normalizes the value.
