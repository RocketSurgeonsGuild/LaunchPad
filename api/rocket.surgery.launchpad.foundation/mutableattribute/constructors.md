# MutableAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [MutableAttribute](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute.md)
- Kind: `Constructors`
- Members: `1`

Generates a mutable view model of the type, a class next to it named `{Prefix}{Name}{Suffix}` ( `RequestViewModel` by default): a settable property for each of the type's properties, `From(value)` to copy a value into it and `ToImmutable()` to make the value from it ( `Foundation.IMutable`1`). The type can be a class, a record, a record struct or a readonly record struct, and the types that contain it must be partial.

<a id="constructor"></a>
<a id="constructor-string-string"></a>

## MutableAttribute(string, string)

- Name: `Constructor(string, string)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.d4b5f5c52aac2c2cd0e8fd565d77879e394d9792/src/Foundation/MutableAttribute.cs#L22-L33)

Generates a mutable view model of the type, a class next to it named `{Prefix}{Name}{Suffix}` ( `RequestViewModel` by default): a settable property for each of the type's properties, `From(value)` to copy a value into it and `ToImmutable()` to make the value from it ( `Foundation.IMutable`1`). The type can be a class, a record, a record struct or a readonly record struct, and the types that contain it must be partial.

```csharp
public sealed class MutableAttribute
{
    public MutableAttribute(
        string prefix = "",
        string suffix = "ViewModel")
    {
        // ...
    }
}
```

## Parameters

- `prefix` (`string`) `optional`
  What goes before the type's name in the view model's name.
- `suffix` (`string`) `optional`
  What goes after the type's name in the view model's name.

## Remarks

A Blazor `EditForm` binds to the view model, which an immutable request can't be bound to. LaunchPad's `ZeroAllocValidator` validates the view model with the validator of the type it makes.
