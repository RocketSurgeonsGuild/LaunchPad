# Assigned<T> Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.foundation.md)
- Type: [Assigned<T>](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.foundation/assigned-1.md)
- Kind: `Constructors`
- Members: `1`

The Assigned type is used to differentiate between not set and set input values.

<a id="constructor"></a>
<a id="constructor-t"></a>

## Assigned(T?)

- Name: `Constructor(T?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.022b9cee91d8927e916725e8a1c1717558619950/src/Foundation/Assigned.cs#L71-L75)

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
