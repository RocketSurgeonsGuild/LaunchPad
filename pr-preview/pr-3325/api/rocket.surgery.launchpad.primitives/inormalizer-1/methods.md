# INormalizer<T> Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives.md)
- Type: [INormalizer<T>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/inormalizer-1.md)
- Kind: `Methods`
- Members: `1`

Normalizes a value before a domain primitive validates it, for `[Normalize<TNormalizer>]`.

<a id="normalize"></a>
<a id="normalize-t"></a>

## Normalize(T)

- Name: `Normalize(T)`
- Modifiers: `static` `abstract`
- Returns: `T`

Normalizes the value.

```csharp
public interface INormalizer<T>
{
    public static abstract T Normalize(
        T value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`T`)
  The value.

## Returns

`T` -- The normalized value.
