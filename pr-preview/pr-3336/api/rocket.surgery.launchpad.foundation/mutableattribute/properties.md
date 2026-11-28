# MutableAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation.md)
- Type: [MutableAttribute](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation/mutableattribute.md)
- Kind: `Properties`
- Members: `1`

Makes the partial class it marks a mutable view model of another type: the generator gives the class a settable property for each of that type's properties, `From(value)` to copy a value into it, `ToImmutable()` to make the value from it and an implicit conversion each way. The copied type can be a class, a record, a record struct or a readonly record struct, in this assembly or another one. The class and the types that contain it must be partial.

<a id="type"></a>

## Type

- Name: `Type`
- Modifiers: `get`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.52b728c7d35f7e5541d5238c567b2b39c033d3c7/src/Foundation/MutableAttribute.cs#L27)

The type the class is a mutable view model of.

```csharp
public Type Type { get; }
```
