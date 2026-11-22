# Assigned<T> Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [Assigned<T>](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1.md)
- Kind: `Constructors`
- Members: `1`

The Assigned type is used to differentiate between not set and set input values.

<a id="constructor"></a>
<a id="constructor-t"></a>

## Assigned(T?)

- Name: `Constructor(T?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Foundation/Assigned.cs#L71-L75)

Initializes a new instance of the `Foundation.Assigned`1` struct.

```csharp
public class Assigned<T>
{
    public Assigned(
        T? value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`T?`)
  The actual value.
