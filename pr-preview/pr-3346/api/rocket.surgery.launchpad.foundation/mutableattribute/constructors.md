# MutableAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.foundation.md)
- Type: [MutableAttribute](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.foundation/mutableattribute.md)
- Kind: `Constructors`
- Members: `1`

Makes the partial class it marks a mutable view model of another type: the generator gives the class a settable property for each of that type's properties, `From(value)` to copy a value into it, `ToImmutable()` to make the value from it and an implicit conversion each way. The copied type can be a class, a record, a record struct or a readonly record struct, in this assembly or another one. The class and the types that contain it must be partial.

<a id="constructor"></a>
<a id="constructor-type"></a>

## MutableAttribute(Type)

- Name: `Constructor(Type)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3346-merge.Sha.ddf4e8df3db056914fd776aa443a00dc7536dd57/src/Foundation/MutableAttribute.cs#L22-L27)

Makes the partial class it marks a mutable view model of another type: the generator gives the class a settable property for each of that type's properties, `From(value)` to copy a value into it, `ToImmutable()` to make the value from it and an implicit conversion each way. The copied type can be a class, a record, a record struct or a readonly record struct, in this assembly or another one. The class and the types that contain it must be partial.

```csharp
public sealed class MutableAttribute
{
    public MutableAttribute(
        Type type)
    {
        // ...
    }
}
```

## Parameters

- `type` (`Type`)
  The type the class is a mutable view model of.

## Remarks

A Blazor `EditForm` binds to the view model, which an immutable request can't be bound to. In a project that references ZeroAlloc.Validation the generator also writes a nested `Validator`, a `ValidatorFor` of the view model that validates the value it makes with that type's validator, and a Dovetail joint registers it as the view model's `ValidatorFor`, so LaunchPad's `ZeroAllocValidator` applies the copied type's rules to the form. A property the class declares itself is not generated, and `From` and `ToImmutable()` use it.
