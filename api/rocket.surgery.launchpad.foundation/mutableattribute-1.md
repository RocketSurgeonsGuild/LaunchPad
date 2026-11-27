# MutableAttribute<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-beta.459`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.8446cfb49f81338ce4d090fef0bb3afa0711fa3f/src/Foundation/MutableAttribute.cs)
- Inherits: `Attribute`

Makes the partial class it marks a mutable view model of `T`: the generator gives the class a settable property for each of its properties, `From(value)` to copy a value into it, `ToImmutable()` to make the value from it and an implicit conversion each way. `T` can be a class, a record, a record struct or a readonly record struct, in this assembly or another one. The class and the types that contain it must be partial.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed class MutableAttribute<T>
    : System.Attribute
    where T : notnull
{
    // ...
}
```

## Type Parameters

- `T` -- The type the class is a mutable view model of.

## Remarks

A Blazor `EditForm` binds to the view model, which an immutable request can't be bound to. In a project that references ZeroAlloc.Validation the generator also writes a nested `Validator`, a `ValidatorFor` of the view model that validates the value it makes with that type's validator, and a Dovetail joint registers it as the view model's `ValidatorFor`, so LaunchPad's `ZeroAllocValidator` applies the copied type's rules to the form. A property the class declares itself is not generated, and `From` and `ToImmutable()` use it.

## Constructors

- [MutableAttribute](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute-1/constructors.md#constructor)
