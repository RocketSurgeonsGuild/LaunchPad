# MutableAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-beta.423`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.bc6e38a474db052319ef3b044454738f3a433de2/src/Foundation/MutableAttribute.cs)
- Inherits: `Attribute`

Generates a mutable view model of the type, a class next to it named `{Prefix}{Name}{Suffix}` ( `RequestViewModel` by default): a settable property for each of the type's properties, `From(value)` to copy a value into it and `ToImmutable()` to make the value from it ( `Foundation.IMutable`1`). The type can be a class, a record, a record struct or a readonly record struct, and the types that contain it must be partial.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed class MutableAttribute
    : System.Attribute
{
    // ...
}
```

## Remarks

A Blazor `EditForm` binds to the view model, which an immutable request can't be bound to. LaunchPad's `ZeroAllocValidator` validates the view model with the validator of the type it makes.

## Constructors

- [MutableAttribute(string, string)](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute/constructors.md#constructor-string-string) -- Generates a mutable view model of the type, a class next to it named `{Prefix}{Name}{Suffix}` ( `RequestViewModel` by default): a settable property for each of the type's properties, `From(value)` to copy a value into it and `ToImmutable()` to make the value from it ( `Foundation.IMutable`1`). The type can be a class, a record, a record struct or a readonly record struct, and the types that contain it must be partial.

## Properties

- [Prefix](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute/properties.md#prefix) : `string` `get` -- What goes before the type's name in the view model's name.
- [Suffix](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute/properties.md#suffix) : `string` `get` -- What goes after the type's name in the view model's name.
