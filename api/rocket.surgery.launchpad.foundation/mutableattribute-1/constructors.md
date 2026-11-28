# MutableAttribute<T> Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [MutableAttribute<T>](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute-1.md)
- Kind: `Constructors`
- Members: `1`

Makes the partial class it marks a mutable view model of `T`: the generator gives the class a settable property for each of its properties, `From(value)` to copy a value into it, `ToImmutable()` to make the value from it and an implicit conversion each way. `T` can be a class, a record, a record struct or a readonly record struct, in this assembly or another one. The class and the types that contain it must be partial.

<a id="constructor"></a>

## MutableAttribute

- Name: `Constructor`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.8fad8144321d59e157ec1f0adfb0c30ad2256abf/src/Foundation/MutableAttribute.cs#L22-L27)

```csharp
public sealed class MutableAttribute<T>
{
    public MutableAttribute()
    {
        // ...
    }
}
```
