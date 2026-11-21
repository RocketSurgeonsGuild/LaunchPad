# MutableAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [MutableAttribute](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute.md)
- Kind: `Properties`
- Members: `2`

Generates a mutable view model of the type, a class next to it named `{Prefix}{Name}{Suffix}` ( `RequestViewModel` by default): a settable property for each of the type's properties, `From(value)` to copy a value into it and `ToImmutable()` to make the value from it ( `Foundation.IMutable`1`). The type can be a class, a record, a record struct or a readonly record struct, and the types that contain it must be partial.

<a id="prefix"></a>

## Prefix

- Name: `Prefix`
- Modifiers: `get`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.995615a80088961c58f34711f6cedc47cee2f39e/src/Foundation/MutableAttribute.cs#L28)

What goes before the type's name in the view model's name.

```csharp
public string Prefix { get; }
```

<a id="suffix"></a>

## Suffix

- Name: `Suffix`
- Modifiers: `get`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.995615a80088961c58f34711f6cedc47cee2f39e/src/Foundation/MutableAttribute.cs#L33)

What goes after the type's name in the view model's name.

```csharp
public string Suffix { get; }
```
