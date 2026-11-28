# ValueObjectMemberDescription

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.464`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Descriptions`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.be289a136371fe2ae7f777d7d311fefdb5490824/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)
- Implements: `IEquatable<ValueObjectMemberDescription>`

One value a value object holds.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Descriptions;

public sealed record ValueObjectMemberDescription
    : System.IEquatable<Rocket.Surgery.LaunchPad.Primitives.Descriptions.ValueObjectMemberDescription>
{
    // ...
}
```

## Constructors

- [ValueObjectMemberDescription(string, Type, bool)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/constructors.md#constructor-string-type-bool) -- One value a value object holds.

## Properties

- [IsNullable](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/properties.md#isnullable) : `bool` `get; init` -- Whether the value may be missing (a nullable reference or value type).
- [Name](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/properties.md#name) : `string` `get; init` -- The name of its property.
- [Type](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/properties.md#type) : `Type` `get; init` -- The type of the value.

## Methods

- [<Clone>$](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#clone) : [ValueObjectMemberDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md)
- [Deconstruct(string, Type, bool)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#deconstruct-string-type-bool)
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(ValueObjectMemberDescription?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#equals-valueobjectmemberdescription) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(ValueObjectMemberDescription?, ValueObjectMemberDescription?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#op-equality-valueobjectmemberdescription-valueobjectmemberdescription) : `bool` `static`
- [op_Inequality(ValueObjectMemberDescription?, ValueObjectMemberDescription?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#op-inequality-valueobjectmemberdescription-valueobjectmemberdescription) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#tostring) : `string` -- Returns a string that represents the current object.
