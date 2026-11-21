# IMutable<T> Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.foundation.md)
- Type: [IMutable<T>](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.foundation/imutable-1.md)
- Kind: `Methods`
- Members: `1`

A mutable copy of an immutable `T`, such as the view model [MutableAttribute](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generates for a request: a form binds to it a property at a time, and it makes the `T` to send.

<a id="toimmutable"></a>

## ToImmutable

- Name: `ToImmutable`
- Modifiers: `abstract`
- Returns: `T`

Makes the `T` from the current values of the mutable copy.

```csharp
public interface IMutable<T>
{
    public abstract T ToImmutable()
    {
        // ...
    }
}
```

## Returns

`T` -- The immutable value.
