# IMutable Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3322/api/rocket.surgery.launchpad.foundation.md)
- Type: [IMutable](/LaunchPad/pr-preview/pr-3322/api/rocket.surgery.launchpad.foundation/imutable.md)
- Kind: `Methods`
- Members: `1`

A mutable copy of an immutable value, such as the view model [MutableAttribute](/LaunchPad/pr-preview/pr-3322/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generates for a request, that can make the value it stands for.

<a id="toimmutable"></a>

## ToImmutable

- Name: `ToImmutable`
- Modifiers: `abstract`
- Returns: `object`

Makes the immutable value from the current values of the mutable copy.

```csharp
public interface IMutable
{
    public abstract object ToImmutable()
    {
        // ...
    }
}
```

## Returns

`object` -- The immutable value, boxed when it is a struct.
