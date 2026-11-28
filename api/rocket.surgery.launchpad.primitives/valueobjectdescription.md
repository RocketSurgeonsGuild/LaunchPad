# ValueObjectDescription

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.464`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Descriptions`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.be289a136371fe2ae7f777d7d311fefdb5490824/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)
- Implements: `IEquatable<ValueObjectDescription>`

What an integration needs to know about one value object: the type and the values it holds, in declaration order.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Descriptions;

public sealed record ValueObjectDescription
    : System.IEquatable<Rocket.Surgery.LaunchPad.Primitives.Descriptions.ValueObjectDescription>
{
    // ...
}
```

## Constructors

- [ValueObjectDescription(Type, IReadOnlyList<string>)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/constructors.md#constructor-type-ireadonlylist-string) -- What an integration needs to know about one value object: the type and the values it holds, in declaration order.

## Properties

- [Members](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/properties.md#members) : [IReadOnlyList<ValueObjectMemberDescription>](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md) `get; init` -- The values it holds, with their types, in declaration order: what an integration describes the value object with, as its own JSON converter writes it, so the serializer's metadata says nothing of its values.
- [Properties](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/properties.md#properties) : `IReadOnlyList<string>` `get; init` -- The names of the values it holds.
- [Type](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/properties.md#type) : `Type` `get; init` -- The value object.

## Methods

- [<Clone>$](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/methods.md#clone) : [ValueObjectDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md)
- [Deconstruct(Type, IReadOnlyList<string>)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/methods.md#deconstruct-type-ireadonlylist-string)
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(ValueObjectDescription?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/methods.md#equals-valueobjectdescription) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(ValueObjectDescription?, ValueObjectDescription?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/methods.md#op-equality-valueobjectdescription-valueobjectdescription) : `bool` `static`
- [op_Inequality(ValueObjectDescription?, ValueObjectDescription?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/methods.md#op-inequality-valueobjectdescription-valueobjectdescription) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription/methods.md#tostring) : `string` -- Returns a string that represents the current object.
