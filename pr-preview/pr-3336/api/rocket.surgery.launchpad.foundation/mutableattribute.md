# MutableAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1290`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Foundation/MutableAttribute.cs)
- Inherits: `Attribute`

Makes the partial class it marks a mutable view model of another type: the generator gives the class a settable property for each of that type's properties, `From(value)` to copy a value into it, `ToImmutable()` to make the value from it and an implicit conversion each way. The copied type can be a class, a record, a record struct or a readonly record struct, in this assembly or another one. The class and the types that contain it must be partial.

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

A Blazor `EditForm` binds to the view model, which an immutable request can't be bound to. In a project that references ZeroAlloc.Validation the generator also writes a nested `Validator`, a `ValidatorFor` of the view model that validates the value it makes with that type's validator, and a Dovetail joint registers it as the view model's `ValidatorFor`, so LaunchPad's `ZeroAllocValidator` applies the copied type's rules to the form. A property the class declares itself is not generated, and `From` and `ToImmutable()` use it.

## Constructors

- [MutableAttribute(Type)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation/mutableattribute/constructors.md#constructor-type) -- Makes the partial class it marks a mutable view model of another type: the generator gives the class a settable property for each of that type's properties, `From(value)` to copy a value into it, `ToImmutable()` to make the value from it and an implicit conversion each way. The copied type can be a class, a record, a record struct or a readonly record struct, in this assembly or another one. The class and the types that contain it must be partial.

## Properties

- [Type](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation/mutableattribute/properties.md#type) : `Type` `get` -- The type the class is a mutable view model of.
