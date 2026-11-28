# MutableAttribute<T> Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.foundation.md)
- Type: [MutableAttribute<T>](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.foundation/mutableattribute-1.md)
- Kind: `Constructors`
- Members: `1`

Makes the partial class it marks a mutable view model of `T`: the generator gives the class a settable property for each of its properties, `From(value)` to copy a value into it, `ToImmutable()` to make the value from it and an implicit conversion each way. `T` can be a class, a record, a record struct or a readonly record struct, in this assembly or another one. The class and the types that contain it must be partial.

<a id="constructor"></a>

## MutableAttribute

- Name: `Constructor`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3340-merge.Sha.1fd0124e0c52f2b8f0975bb6854b707a6a798292/src/Foundation/MutableAttribute.cs#L22-L27)

```csharp
public sealed class MutableAttribute<T>
{
    public MutableAttribute()
    {
        // ...
    }
}
```
